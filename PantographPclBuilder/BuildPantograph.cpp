#include "BuildPantograph.h"

using namespace std;

	BuildPantograph::BuildPantograph()
	{
	}

	int BuildPantograph::CreatePantographTest()
	{
		string pantopath = "C:\\Users\\jdemarchi.TROYSYS\\Documents\\Visual Studio 2013\\Projects\\PantographCreatorCPP\\3005";
		
		return CreatePantograph(pantopath);
	}

	int BuildPantograph::CreatePantograph(const string &basePath)
	{
		return CreatePantograph(basePath, basePath, basePath);
	}



	int BuildPantograph::CreatePantograph(const string &basePath, const string &configPath, const string &dataPath)
	{
		CMarkup xml;
		bool rc;
		string str;
		string configFn = configPath + "/TroyPantographConfiguration.xml";

		rc = xml.Load(configFn);
		if (rc == false) return 100;

		xml.ResetPos();
		xml.FindElem();
		xml.IntoElem();
		xml.FindElem();
		rc = xml.IntoElem();
		if (rc == false) return 101;

		CustomConfiguration *cc = new CustomConfiguration();
		bool foundcc = ReadCustomConfigurationTag(xml, cc);
		if (foundcc == false) return 102;
		int ccnum = 1;
		while (foundcc == true)
		{
			string errorpath("default");
			cc->WarningBoxString = str_replace(cc->WarningBoxString, "/r", "\x0D");
			cc->WarningBoxString = str_replace(cc->WarningBoxString, "/n", "\x0A");
			cc->WarningBoxString = str_replace(cc->WarningBoxString, "/s", "\x02");
			cc->WarningBoxString = str_replace(cc->WarningBoxString, "/d", "\x7F");
			cc->WarningBoxString = str_replace(cc->WarningBoxString, "/t", "\x09");

			
			
			rc = CreatePantographPcl(basePath, dataPath, string("PantographProfile" + StringConverterHelper::toString(ccnum) + "Page1.pcl"), cc);
			if (rc == false) return 103;

			foundcc = ReadCustomConfigurationTag(xml, cc);
			ccnum++;
		}
		return 0;
	}
	
	bool BuildPantograph::CreatePantographPcl(const string &basePath, const string &dataPath, const string &fileName,
		CustomConfiguration *customConfig)
	{
		BuildPantograph::seal_loaded = false;
		BuildPantograph::interf_loaded = false;
		BuildPantograph::mp5_loaded = false;
		BuildPantograph::mp6_loaded = false;
		string FQfileName = dataPath + "/" + fileName;
		//PortMonLogging *errorlog = new PortMonLogging();
		//string data = errorlogpath;
		//std::transform(data.begin(), data.end(), data.begin(), ::tolower);
		//errorlog->ErrorLogFilePath =
		//data == "default" ? basePath + "/Error Log" : data;
		//errorlog->InitializeErrorLog();

		CustomConfigurationInternal *cci = new CustomConfigurationInternal();
		int Xpos, Ypos, Size;
		bool finished;
		unsigned int patidx, pattern_id, CellListIdx, CellList1st;
		int pantoidx, xdim, ydim;
		string message;

		//KLK Trying to keep the code in this class as close to the firmware code
		//    as possible so instead of moving the configuration to another class
		//    and exposing it to the outside world, I created a public class containing
		//    the same config values.  Downside of this is I have to explicitly set
		//    each config value.  That's what this function call does.
		if (customConfig != NULLPTR)
		{
			UpdateValuesWithCustom(customConfig, cci);
			if (cci == NULLPTR || cci->PantographConfig < 1)
				return false;

			cci->fontPath = str_endswith(dataPath, "/\\") ? dataPath : dataPath + "/";
		}

		/* Quit now if there is no pantograph inclusion region. */
		if ((cci->InclRegion.width == 0) || (cci->InclRegion.height == 0))
		{
			return false;
		}

		ofstream outbuf(FQfileName.c_str(), ios::out | ios::binary);

		//TBD DO I NEED THIS??
		/* No pantograph on back of page if this configuration bit is not set. */
		//if (((PantographConfig & Constants::CFG_DUP_MASK) == 0) && (PageSide == 2))
		//{
		//    return;
		//}
		//TBD DO I NEED THIS??
		/* Inhibit multiple copies of pantograph pages unless it the software
		 application explicitly enabled it by setting the Copy bit. */
		//if (((PantographConfig & Constants::CFG_COPY_MASK) == 0) &&
		//     (NumberOfCopies > 1) && !CopyCountChanged)
		//{
		//    byte[] SetCopyToOne = new byte[5] {0x1B,0x26,0x6C,0x31,0x58};
		// 	outbuf.Write(SetCopyToOne,0,SetCopyToOne.Length);
		//    CopyCountChanged = true;  /* Remember that we changed it */
		//}
		OutputMacroCommands(outbuf, 3930, "Y"); //<ESC>&f3930Y
		OutputMacroCommands(outbuf, 0, "X"); //<ESC>&f0X
		OutputMacroCommands(outbuf, 0, "S"); //<ESC>&f0S
		OutputString(outbuf, Constants::PrintDir0);
		OutputString(outbuf, Constants::PageSettings);
		OutputString(outbuf, Constants::ClearHzMargin);
		OutputString(outbuf, Constants::Res600dpi);
		OutputCursorCoords(outbuf, 0, false, 0, false);
		OutputPantographColor(outbuf, cci->PantographColor);
		OutputString(outbuf, Constants::SetPtrnRefPnt);

		//The configuration mask can be used to disable the background.
		if ((cci->PantographConfig & Constants::CFG_NOBG_MASK) > 0)
		{
			finished = true;
		}
		else
		{
			finished = false;
			/* Set the pattern transparency mode to opaque for the pantograph
			 background.  If multiple pantographs are being printed on the same
			 page and the pantographs overlap, we want the latest pantograph in
			 the list to overprint any previously formatted pantographs. */
			OutputString(outbuf, Constants::PtrnTransOpaque);
		}

		//******************
		// BACKGROUND
		//******************

		string tempStr;
		Xpos = cci->InclRegion.left;
		Ypos = cci->InclRegion.top;
		CellListIdx = 0;
		CellList1st = 0;
		while (!finished)
		{
			//If there are not custom Cell settings, use the defaults
			if (cci->CellList.size() == 0)
			{
				pantoidx = 1;
			}
			else
			{
				pantoidx = cci->CellList[CellListIdx].pidx;
			}
			//TBD Why would pantoidx ever be -100??
			if ((pantoidx != 0) && (pantoidx != -100))
			{
				if (pantoidx > 0)
				{
					//KLK May 15, 2012
					//patidx = 0;
					patidx = cci->DarknessFactorMap[cci->DarknessFactor];
					pattern_id = 2 * (pantoidx - 1);
				}
				else
				{
					pantoidx = pantoidx * -1;
					patidx = pantoidx;
					pattern_id = (2 * pantoidx) - 1;
				}

				OutputPatternIdCommand(outbuf, pattern_id + Constants::PAT_ID_BASE); //<ESC>*c####G  -> the pattern ID

				//KLK ADDED FOR PANTOGRAPH 2
				/**********
				if ((cci->BackgroundPatternData) != 0 && pantoidx == 10)
				{
					if (!cci->Panto2BckGrdLoaded)
					{
						LoadPantograph2Pattern(outbuf,
								cci->BackgroundPatternData);
						cci->Panto2BckGrdLoaded = true;
					}
				}
				else
				{
				********/
					//KLK THE ORIGINAL (PRE PANTOGRAPH 2)
					if (!cci->pat_loaded[pattern_id])
					{
						LoadPantographPattern(outbuf, cci, pantoidx, patidx);
						cci->pat_loaded[pattern_id] = true;
					}
			}

			OutputCursorCoords(outbuf, Xpos, false, Ypos, false); //Inclusion region X and Y
			int cnt = cci->CellList.size();
			if (cnt == 0 || cnt == 1)
			{
				OutputRectangleSize(outbuf, cci->InclRegion.width,
						cci->InclRegion.height); //<ESC>c####a####B
				finished = true;
			}
			else
			{
				OutputRectangleSize(outbuf, cci->PantoCellWidth,
						cci->PantoCellHeight); //<ESC>c####a####B
			}

			if (pantoidx == 0)
			{
				OutputString(outbuf, Constants::EraseFill);
			}
			else if (pantoidx == -100)
			{
				OutputString(outbuf, Constants::BlackFill);
			}
			else
			{
				OutputString(outbuf, Constants::UserDefFill);
			}

			finished = (finished == true) ? true : DoneWithCells(cci, Xpos, Ypos, CellListIdx, CellList1st,	0);
		}
		/* Overprinting of pantograph message characters can be a problem if the
		 right edge of the inclusion region is near the logical page boundary,
		 because parts of some characters may land in that narrow region between
		 the logical page boundary and the end of the printable area, where they
		 cannot be erased by a white rectangular area fill.  Try to prevent this
		 by temporarily setting the right margin.  To set the margin we need a
		 column number.  The column width is defined by the HMI setting, and the
		 HMI is defined in units of 1/120 of an inch.  Set the column width (HMI)
		 to 1/120 inch, and divide the page width minus 2/3 inch (at 600 dpi) by
		 5 to compute the column number.  Set the right margin there.  In most
		 cases, characters that would extend into the forbidden zone will not be
		 printed.  We want to prevent overprints in the default (full page) case,
		 but also give the user maximum flexibility.  Once the user defines the
		 inclusion region, he assumes the responsibility to avoid printing past
		 the end of the logical page.  We do not need to restore the HMI after
		 doing this, because it will automatically change to match the pantograph
		 message font when we select it. */
		OutputString(outbuf, Constants::HMI);
		Size = cci->PageWidth - 400;
		int colNumber = Size / 5;
		OutputString(outbuf,
				"\x1b&a" + StringConverterHelper::toString(colNumber) + "M");

		//******************
		// FOREGROUND
		//******************

		finished = false;

		Xpos = cci->InclRegion.left + cci->PantoTextOffsetX;
		Ypos = cci->InclRegion.top + cci->PantoCellHeight
				- cci->PantoTextOffsetY;
		CellListIdx = 0;
		CellList1st = 0;
		while (!finished)
		{
			if (cci->CellList.size() == 0)
			{
				pantoidx = 1;
				message = Constants::DEFAULT_COPY_MESSAGE;
			}
			else
			{
				pantoidx = cci->CellList[CellListIdx].pidx;
				message = cci->CellList[CellListIdx].msg;
			}
			if ((message.length() > 0) && (Xpos < Size))
			{
				OutputCursorCoords(outbuf, Xpos, false, Ypos, false);
				if (pantoidx == 0)
				{
					//Updated Sept 2013
					tempStr = Constants::ESC + "*v0n1o0T";
				}
				else if (pantoidx == -100)
				{
					//Updated Sept 2013
					tempStr = Constants::ESC + "*v0n1o1T";
				}
				else
				{
					if (pantoidx > 0)
					{
						patidx = pantoidx;
						pattern_id = (2 * pantoidx) - 1;
					}
					else
					{
						/* In reverse mode, the background pattern becomes the foreground.
						 For every pantograph pattern pair, the background pattern is
						 based on pattern 0. */
						patidx = 0;
						pantoidx = -pantoidx;
						pattern_id = 2 * (pantoidx - 1);
					}
					OutputPatternIdCommand(outbuf,
							Constants::PAT_ID_BASE + pattern_id);

					//KLK ADDED FOR PANTOGRAPH 2
					/******************
					if (sizeof(cci->ForegroundPatternData) != 0 && (pantoidx == 10))
					{
						if (!cci->Panto2FrGrdLoaded)
						{
							LoadPantograph2Pattern(outbuf,
									cci->ForegroundPatternData);
							cci->Panto2FrGrdLoaded = true;
						}
					}
					else
					{
					*******/
					//KLK THE ORIGINAL (PRE PANTOGRAPH 2)
						if (!cci->pat_loaded[pattern_id])
						{
							LoadPantographPattern(outbuf, cci, pantoidx,
									patidx);
							cci->pat_loaded[pattern_id] = true;
						}
					//Changed Sept 2013
					tempStr = Constants::ESC + "*v0n1o4T";
				}
				OutputString(outbuf, tempStr);

				/* Default pantograph font is 64 point Arial. */
				/*  ESC "(8U" ESC "(s1p64v0s0b16602T */
				OutputString(outbuf, Constants::ESC + "(8U");  // Arial 64
				OutputString(outbuf, Constants::ESC + "(s1p64v0s0b16602T");

				if ((cci->PantographFontSelectString).length() > 0)
				{
					//Swap spaces with ESC
					OutputString(outbuf, cci->PantographFontSelectString);
				}

				bool DelChar = false;
				tempStr = "";
				for (unsigned int i = 0; i < message.length(); i++)
				{
					char ch = message[i];
					if (DelChar)
					{
						DelChar = false;
						switch (ch)
						{
						//Move Cursor Horizontal
						case 'X':
							tempStr += "\x1b*p+"
									+ StringConverterHelper::toString(
											cci->PantographXPlus) + "X";
							break;
						case 'x':
							tempStr += "\x1b*p-"
									+ StringConverterHelper::toString(
											cci->PantographXMinus) + "X";
							break;
						case 'Y':
							tempStr += "\x1b*p+"
									+ StringConverterHelper::toString(
											cci->PantographYPlus) + "Y";
							break;
						case 'y':
							tempStr += "\x1b*p-"
									+ StringConverterHelper::toString(
											cci->PantographYMinus) + "Y";
							break;
						case 'M':
							//KLK NOTE:  No way to do Serial Number
							break;
						case 'P':
							//KLK NOTE:  No way to do Serial Number
							break;
						default:
							break;
						}
					}
					else
					{
						switch (ch)
						{
						case '\n':
						case '\r':
							tempStr += string("\x1b*p")	+ StringConverterHelper::toString(Xpos)	+ string("x")
									+ StringConverterHelper::toString(cci->PantographYPlus) + string("Y");
							break;
						case '\x7F':
							DelChar = true;
							break;
						default:
							tempStr += ch;
							break;
						}
					}
				}
				if (tempStr.length() > 0) OutputString(outbuf, tempStr);

				/* After printing the pantograph message, disable underline and set
				 print direction to 0.  This makes it possible for the user to enable
				 underline or rotate print direction in the font selection string. */
				OutputString(outbuf, Constants::DisableUnderline);
				OutputString(outbuf, Constants::PrintDir0);
			}

			finished = DoneWithCells(cci, Xpos, Ypos, CellListIdx, CellList1st,
					cci->PantoTextOffsetX);
		}

		/* Clear horizontal margins again so that the right margin we set earlier
		 to prevent overprinting does not interfere with other stuff, e.g. the
		 MicroPrint border if there is one. */
		OutputString(outbuf, Constants::ClearHzMargin);

		if ((cci->PantographConfig & Constants::CFG_INT_MASK) > 0)
		{
			PrintInterferencePattern((cci->fontPath + cci->interfFont), outbuf,
					cci);
		}

		if (cci->include_georgia_seal)
		{
			if (!cci->incl_defined)
			{
				if (cci->georgia_x_loc < 0)
				{
					cci->georgia_x_loc = cci->InclRegion.left
							+ cci->InclRegion.width - 400; //right edge minus just over 1/2 inch
				}
				if (cci->georgia_y_loc < 0)
				{
					cci->georgia_y_loc = cci->InclRegion.top
							+ cci->InclRegion.height + 100;
				}
			}
			seal_loaded = loadSoftFont(seal_loaded,
					(cci->fontPath + cci->georgia_seal_font_file),
					(Constants::RES_PERM_FONT_ID_BASE
							+ Constants::RES_FONT_ID_T_SEAL), outbuf);
			OutputString(outbuf, Constants::PushCursorPos);
			OutputString(outbuf, Constants::ESC + "*v1o0n0T"); // {0x1B, 0x2A, 0x76, 0x31, 0x6F, 0x30, 0x6E, 0x30, 0x54};
			OutputCursorCoords(outbuf, cci->georgia_x_loc, false, cci->georgia_y_loc, false);
			OutputString(outbuf,
					"\x1b(" + StringConverterHelper::toString(Constants::RES_PERM_FONT_ID_BASE + Constants::RES_FONT_ID_T_SEAL)	+ "X");
			OutputString(outbuf, cci->georgia_seal_string);
			OutputString(outbuf, Constants::PopCursorPos);
		}

		/* Trim any overprints above the inclusion region only if the inclusion
		 region has not been defined (i.e., we are using the factory default
		 inclusion region which is fitted to the current page size).  Overprints
		 above the inclusion region can only occur if the pantograph message text
		 extends beyond the top edge of the pantograph cell. */
		if (!cci->incl_defined && (cci->InclRegion.top > 0))
		{
			OutputCursorCoords(outbuf, 0, false, 0, false);
			OutputRectangleSize(outbuf, cci->PageWidth, cci->InclRegion.top);
			OutputString(outbuf, Constants::EraseFill);
		}

		/* Trim any overprints to the right of the inclusion region only if the
		 pantograph cell width does not divide evenly into the inclusion region
		 width OR the inclusion region has not been defined (i.e., we are using
		 the factory default inclusion region which is fitted to the current
		 page size).  Overprints to the right of the inclusion region can occur
		 if the cell width does not divide evenly into the inclusion region
		 width or the pantograph message text extends beyond the right edge of
		 the pantograph cell. */
		xdim = cci->InclRegion.width % cci->PantoCellWidth;
		if (!cci->incl_defined || (xdim != 0))
		{
			OutputCursorCoords(outbuf, (cci->InclRegion.left + cci->InclRegion.width), false, cci->InclRegion.top, false);
			OutputRectangleSize(outbuf, cci->PantoCellWidth - xdim,
					cci->InclRegion.height);
			OutputString(outbuf, Constants::EraseFill);
		}

		/* Trim any overprints below the inclusion region only if the pantograph
		 cell height does not divide evenly into the inclusion region height
		 OR the inclusion region has not been defined (i.e., we are using the
		 factory default inclusion region which is fitted to the current page
		 size).  Overprints below the inclusion region can occur if the cell
		 height does not divide evenly into the inclusion region height. */
		ydim = cci->InclRegion.height % cci->PantoCellHeight;
		if (!cci->incl_defined || (ydim != 0))
		{
			OutputCursorCoords(outbuf, cci->InclRegion.left, false, (cci->InclRegion.top + cci->InclRegion.height), false);
			if (xdim == 0)
			{
				OutputRectangleSize(outbuf, cci->InclRegion.width,
						cci->PantoCellHeight - ydim);
			}
			else
			{
				OutputRectangleSize(outbuf,
						cci->InclRegion.width + cci->PantoCellWidth - xdim,
						cci->PantoCellHeight - ydim);
			}
			OutputString(outbuf, Constants::EraseFill);
		}

		//Do Exclusion Region
		for (unsigned int count = 0; count < cci->ExclRegion.size(); count++)
		{
			if ((cci->ExclRegion[count].width > 0)
					&& (cci->ExclRegion[count].height > 0))
			{
				OutputCursorCoords(outbuf, (cci->InclRegion.left + cci->ExclRegion[count].left), false, (cci->InclRegion.top + cci->ExclRegion[count].top), false);
				OutputRectangleSize(outbuf, cci->ExclRegion[count].width,
						cci->ExclRegion[count].height);
				OutputString(outbuf, Constants::EraseFill);
			}
		}

		/* Print signature line AFTER any exclusion region(s) if the designated
		 PantographConfig bits are nonzero. */
		Size = cci->PantographConfig & Constants::CFG_SIGLINE_MASK;
		if (Size != 0)
		{
			Size >>= Constants::CFG_SIGLINE_SHIFT;
			OutputCursorCoords(outbuf, (cci->InclRegion.left + cci->SigOffsetX), false, (cci->InclRegion.top + cci->SigOffsetY), false);
			PrintMicroLine(cci->SigString, outbuf, cci, Size, cci->SigLength,
					mptSigLine);
		}

		Size = cci->PantographConfig & Constants::CFG_BORDER_MASK;
		if (Size != 0)
		{
			Size >>= Constants::CFG_BORDER_SHIFT;
			if (cci->InclRegion.left >= 16)
			{
				Xpos = cci->InclRegion.left - 8;
			}
			else
			{
				Xpos = 8;
			}
			if (cci->InclRegion.top >= 16)
			{
				Ypos = cci->InclRegion.top - 8;
			}
			else
			{
				Ypos = 8;
			}

			OutputCursorCoords(outbuf, cci->InclRegion.left, false, Ypos, false);
			if (cci->MicroPrintCharWidth < 1)
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.width, mptBorder);
			}
			else
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.width, mptBorder,
						cci->MicroPrintCharWidth);
			}

			OutputCursorCoords(outbuf, Xpos, false, (cci->InclRegion.top + cci->InclRegion.height), false);
			OutputPrintDirection(outbuf, 90);
			if (cci->MicroPrintCharWidth < 1)
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.height, mptBorder);
			}
			else
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.height, mptBorder,
						cci->MicroPrintCharWidth);
			}

			OutputPrintDirection(outbuf, 0);
			OutputCursorCoords(outbuf, (cci->InclRegion.left + cci->InclRegion.width), false, (cci->InclRegion.top + cci->InclRegion.height + 8), false);
			OutputPrintDirection(outbuf, 180);
			if (cci->MicroPrintCharWidth < 1)
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.width, mptBorder);
			}
			else
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.width, mptBorder,
						cci->MicroPrintCharWidth);
			}

			OutputPrintDirection(outbuf, 0);
			OutputCursorCoords(outbuf, (cci->InclRegion.left + cci->InclRegion.width + 8), false, cci->InclRegion.top, false);
			OutputPrintDirection(outbuf, 270);
			if (cci->MicroPrintCharWidth < 1)
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.height, mptBorder);
			}
			else
			{
				PrintMicroLine(cci->BorderString, outbuf, cci, Size,
						cci->InclRegion.height, mptBorder,
						cci->MicroPrintCharWidth);
			}

			OutputPrintDirection(outbuf, 0);
		}

		if ((cci->PantographConfig & Constants::CFG_BOX_MASK) > 0)
		{
			WarningBox(outbuf, true, cci);
		}

		if ((cci->PantographConfig & Constants::CFG_ONCE_MASK) > 0)
		{
			cci->PantographConfig = 0;
		}

		OutputMacroCommands(outbuf, 1, "S");
		OutputMacroCommands(outbuf, 1, "X");
		OutputMacroCommands(outbuf, 9, "X");
		OutputMacroCommands(outbuf, 3, "X");
		OutputMacroCommands(outbuf, 8, "X");
		if (outbuf.is_open()) outbuf.close();
		return true;
	}

	bool BuildPantograph::ReadCustomConfigurationTag(CMarkup &xml, CustomConfiguration *cc)
	{
	bool rc = xml.FindElem("CustomConfiguration");
	if (rc == false) return false;

	rc = xml.IntoElem();

	// now we are inside <CustomConfiguration>

	if (xml.FindElem("PageOrientation"))
	{
		string str = xml.GetData();
		pair<string, int> pairPageOrientationType[] = {
#define POT(x,y) make_pair(qstr(x),y),
#define qstr(x) #x
#include "PageOrientationType.txt"
#undef POT
		};
		map<string, int> mapPageOrientationType(pairPageOrientationType,
			pairPageOrientationType + sizeof pairPageOrientationType / sizeof pairPageOrientationType[0]);
		map<string, int>::iterator it = mapPageOrientationType.find(str);
		if (it != mapPageOrientationType.end())
			cc->PageOrientation = (PageOrientationType)it->second;

	}

	if (xml.FindElem("PageType"))
	{
		string str = xml.GetData();
		pair<string, int> pairPageType[] = {
#define PT(x,y) make_pair(qstr(x),y),
#define qstr(x) #x
#include "PageType.txt"
#undef PT
		};
		map<string, int> mapPageType(pairPageType,
			pairPageType + sizeof pairPageType / sizeof pairPageType[0]);
		map<string, int>::iterator it = mapPageType.find(str);
		if (it != mapPageType.end())
			cc->PageSize = (PageType)it->second;
	}

	if (xml.FindElem("PantographColor"))
	{
		string str = xml.GetData();
		pair<string, int> pairPantographColorType[] = {
#define PCT(x,y) make_pair(qstr(x),y),
#define qstr(x) #x
#include "PantographColorType.txt"
#undef PCT
		};
		map<string, int> mapPantographColorType(pairPantographColorType,
			pairPantographColorType + sizeof pairPantographColorType / sizeof pairPantographColorType[0]);
		map<string, int>::iterator it = mapPantographColorType.find(str);
		if (it != mapPantographColorType.end())
			cc->PantographColor = (PantographColorType)it->second;
	}

	if (xml.FindElem("UseDefaultInclusionForPaperSize"))
	{
		if (toLower(xml.GetData()) == "false")
			cc->UseDefaultInclusionForPaperSize = false;
	}

	rc = xml.FindElem("InclusionRegion");
	rc = xml.IntoElem();
	if (xml.FindElem("XAnchor"))
		cc->InclusionRegion.XAnchor = atoi(xml.GetData().c_str());
	if (xml.FindElem("YAnchor"))
		cc->InclusionRegion.YAnchor = atoi(xml.GetData().c_str());
	if (xml.FindElem("Width"))
		cc->InclusionRegion.Width = atoi(xml.GetData().c_str());
	if (xml.FindElem("Height"))
		cc->InclusionRegion.Height = atoi(xml.GetData().c_str());
	rc = xml.OutOfElem();

	//vector<PantographRegionObjectType> ExclusionRegions;
	cc->ExclusionRegions.clear();
	rc = xml.FindElem("ExclusionRegions");
	rc = xml.IntoElem();
	rc = xml.FindElem("PantographRegionObjectType");
	while (rc == true)
	{
		rc = xml.IntoElem();
		PantographRegionObjectType item;
		if (xml.FindElem("XAnchor"))
			item.XAnchor = atoi(xml.GetData().c_str());
		if (xml.FindElem("YAnchor"))
			item.YAnchor = atoi(xml.GetData().c_str());
		if (xml.FindElem("Width"))
			item.Width = atoi(xml.GetData().c_str());
		if (xml.FindElem("Height"))
			item.Height = atoi(xml.GetData().c_str());

		(cc->ExclusionRegions).push_back(item);
		rc = xml.OutOfElem();
		rc = xml.FindElem("PantographRegionObjectType");
	}
	rc = xml.OutOfElem();

	if (xml.FindElem("PantographConfiguration"))
		cc->PantographConfiguration = xml.GetData();

	cc->CellList.clear();
	rc = xml.FindElem("CellList");
	rc = xml.IntoElem();
	int i = 0;
	rc = xml.FindElem("PantographCellDescriptorType");
	while (rc == true)
	{
		i++;
		rc = xml.IntoElem();
		PantographCellDescriptorType item;
		if (xml.FindElem("pidx"))
			item.pidx = atoi(xml.GetData().c_str());
		else
			item.pidx = i;
		if (xml.FindElem("msg"))
			item.msg = xml.GetData();
		else
			item.msg = "";

		(cc->CellList).push_back(item);
		rc = xml.OutOfElem();
		rc = xml.FindElem("PantographCellDescriptorType");
	}
	rc = xml.OutOfElem();

	if (xml.FindElem("InterferencePatternId"))
		cc->InterferencePatternId = atoi(xml.GetData().c_str());

	if (xml.FindElem("BgDarknessFactor"))
		cc->BgDarknessFactor = atoi(xml.GetData().c_str());

	if (xml.FindElem("SignatureLineOffsetX"))
		cc->SignatureLineOffsetX = atoi(xml.GetData().c_str());
	if (xml.FindElem("SignatureLineOffsetY"))
		cc->SignatureLineOffsetY = atoi(xml.GetData().c_str());
	if (xml.FindElem("SignatureLineLength"))
		cc->SignatureLineLength = atoi(xml.GetData().c_str());
	if (xml.FindElem("SignatureLineString"))
		cc->SignatureLineString = xml.GetData();

	if (xml.FindElem("BorderString"))
		cc->BorderString = xml.GetData();

	if (xml.FindElem("WarningBoxString"))
		cc->WarningBoxString = xml.GetData();

	if (xml.FindElem("DensityPattern1"))
		cc->DensityPattern1 = atoi(xml.GetData().c_str());
	if (xml.FindElem("DensityPattern2"))
		cc->DensityPattern2 = atoi(xml.GetData().c_str());

	if (xml.FindElem("TROYmarkOn"))
	{
		if (toLower(xml.GetData()) == "true")
			cc->TROYmarkOn = true;
	}

	if (xml.FindElem("UseDynamicTmMsg"))
	{
		if (xml.GetData() == "false")
			cc->UseDynamicTmMsg = false;
	}

	if (xml.FindElem("IncludeMp"))
	{
		if (toLower(xml.GetData()) == "false")
			cc->IncludeMp = false;
	}

	if (xml.FindElem("UseDynamicMp"))
	{
		if (toLower(xml.GetData()) == "true")
			cc->UseDynamicMp = true;
	}

	if (xml.FindElem("GeorgiaSealEnabled"))
	{
		if (toLower(xml.GetData()) == "true")
			cc->GeorgiaSealEnabled = true;
	}
	if (xml.FindElem("GeorgiaSealXLoc"))
		cc->GeorgiaSealXLoc = atoi(xml.GetData().c_str());
	if (xml.FindElem("GeorgiaSealYLoc"))
		cc->GeorgiaSealYLoc = atoi(xml.GetData().c_str());
	if (xml.FindElem("GeorgiaSealFile"))
		cc->GeorgiaSealFile = xml.GetData();
	rc = xml.OutOfElem();
	return true;
}
	string BuildPantograph::toLower(const string& str)
	{
		string lstr = str;
		for (unsigned int i = 0; i < lstr.length(); i++)
		{
			char c = lstr[i];
			lstr[i] = tolower(c);
		}
		return lstr;
	}

	char* BuildPantograph::readFileIntoBuffer(const string& fn, int& bufsize)
	{
		ifstream infile(fn.c_str(), ios::in | ios::binary);
		if (!infile.is_open())
			return NULLPTR;

		struct stat results;

		if (stat(fn.c_str(), &results) != 0)
			return NULLPTR;
		int filesize = results.st_size;
		char* buffer = new char[filesize];
		infile.read(buffer, filesize);
		bufsize = filesize;
		infile.close();
		return buffer;
	}
	bool BuildPantograph::loadSoftFont(bool loaded, const string &fn, int fontId,
			ofstream &outbuf)
	{
		if (loaded)
		{
			return true;
		}
		int bufsize = 0;
		char* buf = readFileIntoBuffer(fn, bufsize);
		OutputFontId(outbuf, fontId);
		outbuf.write(buf, bufsize);
		OutputString(outbuf, Constants::SoftFontTemp);
		OutputFontId(outbuf, 0);
		return true;
	}
	void BuildPantograph::PrintInterferencePattern(const string &filename,
			ofstream &outbuf, CustomConfigurationInternal *cci)
	{
		int Xpos, Xinc, Xleft, Xright, Ypos, Yinc, Ytop, Ybottom;
		bool customfont = false, customstring = false;
		string IntStr = "";

		//C# TO C++ CONVERTER WARNING: Since the array size is not known in this declaration, C# to C++ Converter has converted this array to a pointer.  You will need to call 'delete[]' where appropriate:
		//ORIGINAL LINE: byte[] tempBytes;
		string tempStr;
		if ((cci->CustomInterfString).length() > 0)
		{
			IntStr = cci->CustomInterfString;
			customstring = true;
			Xinc = cci->CustomInterfHStep;
			Xleft = cci->CustomInterfHStart;
			Xright = cci->CustomInterfHStop;
			Yinc = cci->CustomInterfVStep;
			Ytop = cci->CustomInterfVStart;
			Ybottom = cci->CustomInterfVStop;
			/* Quit now if "margin" settings prevent anything from being printed. */
			if ((Xleft > cci->InclRegion.width)
					|| (Xright > cci->InclRegion.height - Xleft)
					|| (Ytop > cci->InclRegion.height)
					|| (Ybottom > cci->InclRegion.height - Ytop))
			{
				return;
			}

			/* Default custom interference font is 8 cpi Letter Gothic Italic. */
			OutputString(outbuf, Constants::ESC + "(8U");
			OutputString(outbuf, Constants::ESC + "(s0p8h1s0b4102T");
			OutputString(outbuf, Constants::ESC + "*v1o0n1T");

			if ((cci->CustomInterfFontSel).length() > 0)
			{
				customfont = true;
			}
		}
		else if ((cci->InterferenceStr).length() > 0)
		{
			IntStr = cci->InterferenceStr;
			Xinc = 300;
			Xleft = 0;
			Xright = 0;
			Yinc = 300;
			Ytop = 0;
			Ybottom = 0;
			interf_loaded = loadSoftFont(interf_loaded, filename,
					(Constants::RES_PERM_FONT_ID_BASE
							+ Constants::RES_FONT_ID_T_INTERF), outbuf);
			OutputString(outbuf,
					"\x1b("
							+ StringConverterHelper::toString(
									(Constants::RES_PERM_FONT_ID_BASE
											+ Constants::RES_FONT_ID_T_INTERF))
							+ "X");
			OutputString(outbuf, Constants::ESC + "*v1o0n1T");
		}
		else
		{
			return;
		}

		IntStr = str_replace(IntStr, "/n", "\x0A");
		IntStr = str_replace(IntStr, "/r", "\x0D");

		Ypos = cci->InclRegion.top + Ytop;
		unsigned int intStrCntr = 0;
		unsigned int currLineCntr = 0;
		tempStr = "";
		while (Ypos < cci->InclRegion.top + cci->InclRegion.height - Ybottom)
		{
			Xpos = cci->InclRegion.left + Xleft;
			tempStr += string("\x1b*p")
					+ StringConverterHelper::toString(Xpos) + string("x")
					+ StringConverterHelper::toString(Ypos) + string("Y");

			while (Xpos < cci->InclRegion.left + cci->InclRegion.width - Xright)
			{
				/* Process the interference font selection string if the user
				 specified any custom interference font changes via the Select
				 Custom Interference Font command. */
				if (customfont)
				{
					/* Spaces in this string represent escape characters. */
					cci->CustomInterfFontSel = str_replace(
							cci->CustomInterfFontSel, " ", "\x1b");
					tempStr += cci->CustomInterfFontSel;
				}

				if ((IntStr[intStrCntr] != '\x0A')
						&& (IntStr[intStrCntr] != '\x0D'))
				{
					//Output a character from the interference pattern string
					char c = IntStr[intStrCntr];
					tempStr += c;
					Xpos += Xinc;
				}
				if (customfont)
				{
					/* Disable underline and set print direction to 0, in case the
					 custom font selection string enabled underline or rotated print
					 direction. */
					tempStr += Constants::DisableUnderline;
					tempStr += Constants::PrintDir0;
				}

				if (customstring)
					tempStr += "\x1b*p"
							+ StringConverterHelper::toString(Xpos) + "x"
							+ StringConverterHelper::toString(Ypos) + "Y";

				if ((intStrCntr + 1 >= IntStr.length())
						|| (IntStr[intStrCntr + 1] == '\x0A')
						|| (IntStr[intStrCntr + 1] == '\x0D'))
				{
					intStrCntr = currLineCntr;
				}
				else
				{
					intStrCntr++;
				}
			}
			intStrCntr++;
			//Moving to the next Ypos
			//Move to either the next line of text (CR or LF) or start back over from the first character
			while ((intStrCntr < IntStr.length())
					&& (IntStr[intStrCntr] != '\x0A')
					&& (IntStr[intStrCntr] != '\x0D'))
			{
				intStrCntr++;
			}

			//If CR and LF was not found or if CR or LF was found but was the last character
			if ((intStrCntr == IntStr.length())
					|| (intStrCntr + 1 >= IntStr.length()))
			{
				currLineCntr = 0;
				intStrCntr = 0;
			}
			else //CR or LF found followed by a character
			{
				//currLineCntr will hold the location of the first character of the new line
				currLineCntr = ++intStrCntr;
			}
			Ypos += Yinc;
		}
		OutputString(outbuf, tempStr);
	}
	string BuildPantograph::MakeMpLine(int ptsize, const string &MpStr, int XAnchor,
			int YAnchor, int Width, int Height, const string &fontFileName,
			bool PrintMp, int newcharwidth, bool IncludeFonts)
	{
		string retstr1 = "";
		string retstr2 = "";
		int howfar = 0, endpoint = Width, charwidth = 7;

		retstr1 += "\x1b*p" + StringConverterHelper::toString(XAnchor)
				+ string("x") + StringConverterHelper::toString(YAnchor) + "Y";

		//<ESC>*v0T
		retstr1 += "\x1b*v0T";
		//<ESC>&f0S
		retstr1 += "\x1b&f0S";
		int midclearpt = Height / 2;
		retstr1 += "\x1b*p-20x-" + StringConverterHelper::toString(midclearpt)
				+ "Y";
		retstr1 += "\x1b*c" + StringConverterHelper::toString(Width + 26)
				+ "a" + StringConverterHelper::toString(Height) + "B";
		//<ESC>*c1P
		retstr1 += "\x1b*c1P";
		retstr1 += "\x1b*p+2Y";

		if (PrintMp)
		{
			retstr2 += "\x1b*v0N";

			/* Select 'MP' font, print 'M' */
			retstr2 += "\x1b(10U\x1b(s1p5v1s3b16602TM\x1b*p-3XP";

			retstr2 += "\x1b*v0N";

			/* Reprint the 'M' to replace pixels erased by the 'P' in opaque mode. */
			retstr2 += "\x1b&f1S";
			retstr2 += "\x1b&f0S";
			//int tint = Width - 60;
			//retstr2 += "\x1b*p+" + tint + "x-10Y";
			retstr2 += "\x1b*p-20x-10Y";
			endpoint -= 60;
		}
		//OutputString(Constants::PopCursorPos);
		retstr2 += "\x1b&f1S";
		retstr2 += "\x1b*p+30X";

		switch (ptsize)
		{
		case 6:
			retstr2 += "\x1b("
					+ StringConverterHelper::toString(
							Constants::RES_PERM_FONT_ID_BASE
									+ Constants::RES_FONT_ID_T_MICRO6) + "X";
			retstr2 += "\x1b*p-5Y";
			break;
		case 8:
			retstr2 += "\x1b(10U"; //<ESC>(10U
			retstr2 += "\x1b(s0p80h.8v0s0b0T";
			break;
		}

		charwidth = 7;
		if (newcharwidth > 0)
		{
			charwidth = newcharwidth;
		}

		unsigned int strIndex = 0;
		char ch;
		while (howfar < endpoint)
		{
			if (strIndex >= MpStr.length())
			{
				strIndex = 0;
			}
			ch = MpStr[strIndex];
			/* Do not print characters less than ASCII 32 (space). */
			if ((ch >= ' ') && (ch != '!'))
			{
				ch = toupper(ch);
			}
			retstr2 += ch;
			howfar += charwidth;
			if (ch == ' ')
			{
				retstr2 += "\x1b*p-2X";
				howfar -= 2;
			}
			strIndex++;
		}

		int fontlen = 0;
		char *inputBuffer = NULLPTR;
		string fontId = "";
		string softfonttemp = "";
		if (IncludeFonts)
		{
			int bufsize = 0;
			inputBuffer = readFileIntoBuffer(fontFileName, bufsize);

			fontId = Constants::ESC + "*c10623D";

			softfonttemp = Constants::ESC + "*c4F" + Constants::ESC + "*c0D";
			fontlen = bufsize + fontId.length() + softfonttemp.length();
		}

		string returnStr;
		// copy retstr1 to returnStr
		returnStr = retstr1;
		if (fontlen > 0)
		{
			// append fontid
			returnStr += fontId;
			// append inputBuffer
			returnStr += inputBuffer;
			// append softfonttemp
			returnStr += softfonttemp;
		}
		// append retstr2
		returnStr += retstr2;
		return returnStr;
	}
	void BuildPantograph::PrintMicroLine(const string &str, ofstream &outbuf,
			CustomConfigurationInternal *cci, int font, int len,
			MicroPrintType ltype, int newcharwidth)
	{
		int howfar = 0, endpoint = len, charwidth = 7;
		bool convert = false, printtag = true;
		printtag = cci->PrintMp;

		/* If the string begins with the '!' character, do NOT print the 'MP' tag.
		 If the string consists only of '!' characters, do not print anything.  */
		if (str[0] == '!')
		{
			printtag = false;
			if (str.length() == 1)
			{
				return;
			}
		}

		//KLK ASK JC ABOUT THE < 64 THING
		if ((str.length() < 1) || (len < 64))
		{
			return;
		}

		//C# TO C++ CONVERTER WARNING: Since the array size is not known in this declaration, C# to C++ Converter has converted this array to a pointer.  You will need to call 'delete[]' where appropriate:
		//ORIGINAL LINE: byte[] tempBytes;
		string tempString;

		OutputString(outbuf, Constants::PrintInBlack);
		OutputString(outbuf, Constants::PushCursorPos);

		switch (ltype)
		{
		case mptSigLine:
			OutputCursorCoords(outbuf, -6, true, -30, true);
			OutputRectangleSize(outbuf, len + 12, 54);
			OutputString(outbuf, Constants::EraseFill);
			OutputCursorCoords(outbuf, (len - 54), true, 20, true);
			break;

		case mptBorder:
			OutputCursorCoords(outbuf, -20, true, -12, true);
			OutputRectangleSize(outbuf, len + 26, 20);
			OutputString(outbuf, Constants::EraseFill);
			OutputCursorCoords(outbuf, 0, true, 2, true);
			break;
		default:
			return;
		}

		switch (font)
		{
		case 1:
			mp5_loaded = loadSoftFont(mp5_loaded,
					(cci->fontPath + cci->mp5Font),
					(Constants::RES_PERM_FONT_ID_BASE
							+ Constants::RES_FONT_ID_T_MICRO5), outbuf);
			break;
		case 2:
			mp6_loaded = loadSoftFont(mp6_loaded,
					(cci->fontPath + cci->mp6Font),
					(Constants::RES_PERM_FONT_ID_BASE
							+ Constants::RES_FONT_ID_T_MICRO6), outbuf);
			break;
		}

		if (printtag)
		{
			//tempBytes = new byte[] { 0x1B, 0x2A, 0x76, 0x31, 0x4E }; //<ESC>*v1N
			OutputString(outbuf, Constants::ESC + "*v0N");

			/* Select 'MP' font, print 'M' */
			OutputString(outbuf, Constants::ESC + "(10U");
			OutputString(outbuf, Constants::ESC + "(s1p5v1s3b16602TM");
			OutputString(outbuf, Constants::ESC + "*p-3XP");
			OutputString(outbuf, Constants::ESC + "*v0N");

			//KLK ASK JC ABOUT THIS POP AND PUSH
			/* Reprint the 'M' to replace pixels erased by the 'P' in opaque mode. */
			OutputString(outbuf, Constants::PopCursorPos);
			OutputString(outbuf, Constants::PushCursorPos);
			if (ltype == mptSigLine)
			{
				OutputCursorCoords(outbuf, (len - 60), true, -10, true);
				endpoint -= 60;
			}
			else if (ltype == mptBorder)
			{
				OutputCursorCoords(outbuf, -20, true, -10, true);
			}
		}
		OutputString(outbuf, Constants::PopCursorPos);

		if (ltype == mptBorder)
		{
			if (printtag)
			{
				OutputCursorCoords(outbuf, 30, true, 0, true);
				howfar = 30;
			}
			else
			{
				OutputCursorCoords(outbuf, -14, true, 0, true);
				endpoint += 14;
			}
		}

		switch (font)
		{
		case 1:
			OutputString(outbuf,
					"\x1b("
							+ StringConverterHelper::toString(
									Constants::RES_PERM_FONT_ID_BASE
											+ Constants::RES_FONT_ID_T_MICRO5)
							+ "X");
			OutputCursorCoords(outbuf, 0, true, -6, true);
			charwidth = 5;
			convert = true;
			break;
		case 2:
			OutputString(outbuf,
					"\x1b("
							+ StringConverterHelper::toString(
									Constants::RES_PERM_FONT_ID_BASE
											+ Constants::RES_FONT_ID_T_MICRO6)
							+ "X");
			OutputCursorCoords(outbuf, 0, true, -5, true);
			charwidth = 7;
			convert = true;
			break;
		case 3:
			OutputString(outbuf, Constants::ESC + "(10U");
			OutputString(outbuf, Constants::ESC + "(s0p80h.8v0s0b0T");
			charwidth = 7;
			convert = false;
			break;
		}

		if (newcharwidth > 0)
		{
			charwidth = newcharwidth;
		}

		unsigned int strIndex = 0;
		char ch;
		string tempstr = "";

		while (howfar < endpoint)
		{
			if (strIndex >= str.length())
			{
				strIndex = 0;
			}
			ch = str[strIndex];
			/* Do not print characters less than ASCII 32 (space). */
			if ((ch >= ' ') && (ch != '!'))
			{
				/* Convert lowercase characters to uppercase for 0.5 pt and 0.6 pt. */
				if (((convert) & (ch >= 'a')) && (ch <= 'z'))
				{
					ch = toupper(ch);
				}
//				outbuf.Write(ch);
				tempstr += ch;
				howfar += charwidth;
				if (ch == ' ')
				{
					tempstr += ("\x1b*p-2X");  // adjust cursor position
					howfar -= 2;
				}
			}
			strIndex++;
		}
		OutputString(outbuf, tempstr);
	}

	int BuildPantograph::WarningBox(ofstream &outbuf, bool print, CustomConfigurationInternal *cci)
	{
		int sizetouse = cci->WarningBoxSize > 0 ? cci->WarningBoxSize : 0;

		int boxheight, boxwidth, startx, starty, tabstop, linespacing, fontsel =
				0;
		int fontheight, tempheight;
		bool drawbox = true, negative, warningboxbelow = true,
				boxheightspecified = false;
		int xcorner = Constants::MAX_WIDTH + 1;
		int ycorner = Constants::MAX_HEIGHT + 1;
		int boxoffset = 0;
		string holdFontStr = "";

		if (sizetouse < 1)
		{
			if (cci->InclRegion.width >= 3500)
			{
				boxheight = 390;
				boxwidth = 3400;
				startx = 38;
				starty = 78;
				tabstop = 784;
				linespacing = 66;
				fontheight = 7;
			}
			else if (cci->InclRegion.width >= 2850)
			{
				boxheight = 320;
				boxwidth = 2750;
				startx = 26;
				starty = 60;
				tabstop = 636;
				linespacing = 52;
				fontheight = 6;
			}
			else
			{
				boxheight = 280;
				boxwidth = 2100; //was 2020
				startx = 19;
				starty = 53;
				tabstop = 0;
				linespacing = 46;
				fontheight = 5;
			}
		}
		else
		{
			if (sizetouse == 1)
			{
				boxheight = 390;
				boxwidth = 3400;
				startx = 38;
				starty = 78;
				tabstop = 784;
				linespacing = 66;
				fontheight = 7;
			}
			else if (sizetouse == 2)
			{
				boxheight = 320;
				boxwidth = 2750;
				startx = 26;
				starty = 60;
				tabstop = 636;
				linespacing = 52;
				fontheight = 6;
			}
			else
			{
				boxheight = 280;
				boxwidth = 2100; //was 2020
				startx = 19;
				starty = 53;
				tabstop = 0;
				linespacing = 46;
				fontheight = 5;
			}
		}

		unsigned int strIndex = 0;
		char ch;
		while ((strIndex < (cci->CustomWarningString).length()) && ((cci->CustomWarningString[strIndex]) != 0x02))
		{
			ch = cci->CustomWarningString[strIndex];
			switch (ch)
			{
			case 'A':
				warningboxbelow = false;
				strIndex++;
				break;
			case 'N':
				drawbox = false;
				strIndex++;
				break;
			case 'P':
				xcorner = 0;
				ch = cci->CustomWarningString[++strIndex];
				negative = false;
				if (ch == '+')
				{
					ch = cci->CustomWarningString[++strIndex];
				}
				else if (ch == '-')
				{
					negative = true;
					ch = cci->CustomWarningString[++strIndex];
				}
				while ((ch >= '0') && (ch <= '9'))
				{
					if (xcorner < 1000)
					{
						xcorner = xcorner * 10 + (ch - '0');
					}
					ch = cci->CustomWarningString[++strIndex];
				}
				if (xcorner > Constants::MAX_WIDTH)
				{
					xcorner = Constants::MAX_WIDTH;
				}
				if (negative)
				{
					xcorner *= -1;
				}
				break;
			case 'Q':
				ycorner = 0;
				ch = cci->CustomWarningString[++strIndex];
				negative = false;
				if (ch == '+')
				{
					ch = cci->CustomWarningString[++strIndex];
				}
				else if (ch == '-')
				{
					negative = true;
					ch = cci->CustomWarningString[++strIndex];
				}
				while ((ch >= '0') && (ch <= '9'))
				{
					if (ycorner < 1000)
					{
						ycorner = ycorner * 10 + (ch - '0');
					}
					ch = cci->CustomWarningString[++strIndex];
				}
				if (ycorner > Constants::MAX_HEIGHT)
				{
					ycorner = Constants::MAX_HEIGHT;
				}
				if (negative)
				{
					ycorner *= -1;
				}
				break;

			case 'W':
				boxwidth = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex,
						boxwidth);
				break;
			case 'H':
				boxheight = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex,
						boxheight);
				boxheightspecified = true;
				break;
			case 'X':
				boxheight = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex, startx);
				break;
			case 'Y':
				boxheight = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex, starty);
				break;
			case 'T':
				tabstop = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex, tabstop);
				break;
			case ' ':
				linespacing = 0;
				strIndex++;
				GetWarningBoxValue(cci->CustomWarningString, strIndex,
						linespacing);
				break;
			case 'F':
				strIndex++;
				if ((ch >= '0') && (ch <= '9')
					&& (strIndex < (cci->CustomWarningString).length()))
				{
					ch = cci->CustomWarningString[strIndex];
					fontsel = static_cast<int>(ch);
					strIndex++;
				}
				break;
			case 'V':
				strIndex++;
				if ((ch >= '0') && (ch <= '9')
					&& (strIndex < (cci->CustomWarningString).length()))
				{
					ch = cci->CustomWarningString[strIndex];
					fontheight = static_cast<int>(ch);
					strIndex++;
					if ((ch >= '0') && (ch <= '9')
						&& (strIndex < (cci->CustomWarningString).length()))
					{
						ch = cci->CustomWarningString[strIndex];
						fontheight = fontheight * 10 + static_cast<int>(ch);
						strIndex++;
					}
				}
				break;
			default:
				strIndex++;
				break;
			}
		}

		if (strIndex < (cci->CustomWarningString).length())
		{
			ch = cci->CustomWarningString[strIndex];
		}
		else
		{
			ch = 0x00;
		}

		tempheight = fontheight;
		if ((ch == '\0') && (!boxheightspecified))
		{
			if ((!cci->troymark_on) && (boxheight >= linespacing))
			{
				boxheight -= linespacing;
			}
			if ((!(((cci->PantographConfig & Constants::CFG_BORDER_MASK) > 0)
					&& (cci->BorderString != ""))
					&& !(((cci->PantographConfig & Constants::CFG_SIGLINE_MASK)
							> 0) && (cci->SigString != "")))
					&& (boxheight >= linespacing))
			{
				boxheight -= linespacing;
			}
		}

		if (ycorner > Constants::MAX_HEIGHT)
		{
			/* The vertical position of the warning box has not been explicitly
			 specified; therefore we must figure out the offset for automatic
			 positioning above or below the pantograph inclusion region.  The
			 offset must be a little bigger if that is necessary to prevent it
			 from printing over a MicroPrint border line's 'MP' symbol. */
			if (((cci->PantographConfig & Constants::CFG_BORDER_MASK) != 0)
				&& ((cci->BorderString).length() > 0))
			{
				boxoffset = 58;
			}
			else
			{
				boxoffset = 30;
			}
		}

		//C# TO C++ CONVERTER WARNING: Since the array size is not known in this declaration, C# to C++ Converter has converted this array to a pointer.  You will need to call 'delete[]' where appropriate:
		//ORIGINAL LINE: byte[] tempBytes;
		string tempStr;
		if (print)
		{
			OutputString(outbuf, Constants::ESC + "(8U");
			switch (fontsel)
			{
			case 0: //Arial
				holdFontStr = "\x1b(s1p7v0s0b16602T";
				break;
			case 1: //CG Times
				holdFontStr = "\x1b(s1p7v0s0b4101T";
				break;
			case 2: // CG Omega
				holdFontStr = "\x1b(s1p7v0s0b4113T";
				break;
			case 3: //Univers
				holdFontStr = "\x1b(s1p7v0s0b4148T";
				break;
			case 4: //Garmond Antiqua
				holdFontStr = "\x1b(s1p7v0s0b4197T";
				break;
			case 5: // Times New Roman
				holdFontStr = "\x1b(s1p7v0s0b16901T";
				break;
			case 6: //Helvetia
				holdFontStr = "\x1b(s1p7v0s0b24580T";
				break;
			case 7: //Palatino Roman
				holdFontStr = "\x1b(s1p7v0s0b24591T";
				break;
			case 8:
			case 9: //New Century Schoolbook Roman
				holdFontStr = "\x1b(s1p7v0s0b24703T";
				break;
			}
			holdFontStr += "\x1b(s"
					+ StringConverterHelper::toString(fontheight) + "V";
			OutputString(outbuf, holdFontStr);

			OutputString(outbuf, Constants::PrintInBlack);

			if (xcorner > Constants::MAX_WIDTH)
			{
				xcorner = (cci->InclRegion.width - boxwidth) / 2;
			}
			xcorner += cci->InclRegion.left;
			if (xcorner < 0)
			{
				xcorner = 0;
			}
			else if (xcorner > Constants::MAX_WIDTH)
			{
				xcorner = Constants::MAX_WIDTH;
			}

			if (ycorner > Constants::MAX_HEIGHT)
			{
				if (warningboxbelow)
				{
					ycorner = cci->InclRegion.height + boxoffset;
				}
				else
				{
					ycorner = (boxheight + boxoffset) * 1;
				}
			}

			ycorner += cci->InclRegion.top;
			if (ycorner < 0)
			{
				ycorner = 0;
			}
			else if (ycorner > Constants::MAX_HEIGHT)
			{
				ycorner = Constants::MAX_HEIGHT;
			}

			OutputCursorCoords(outbuf, xcorner, false, ycorner, false);
			OutputRectangleSize(outbuf, boxwidth, boxheight);

			if (drawbox)
			{
				OutputString(outbuf, Constants::BlackFill);
				OutputCursorCoords(outbuf, 6, true, 6, true);
				OutputRectangleSize(outbuf, boxwidth - 12, boxheight - 12);
			}

			OutputString(outbuf, Constants::EraseFill);

			OutputCursorCoords(outbuf, (xcorner + startx), false, (ycorner + starty), false);

			if (ch == '\0')
			{
				OutputString(outbuf,
						"WARNING:  This document contains the following industry recognized tamper resistant security features.");
				OutputCursorCoords(outbuf, (xcorner + startx), false, (linespacing + 8), true);

				--tempheight;
				OutputString(outbuf,
						"\x1b(s" + StringConverterHelper::toString(tempheight)
								+ "V");

				OutputString(outbuf, "Copy Void Pantograph");
				if (tabstop == 0)
				{
					OutputString(outbuf, " - ");
				}
				else
				{
					OutputCursorCoords(outbuf, (xcorner + tabstop), false, 0, true);
				}
				OutputString(outbuf, "When copying is attempted on many copiers and scanners the message ");

				tempStr = "";
				for (vector<PantographCellDescriptorType>::iterator it = cci->CellList.begin(); it != cci->CellList.end(); ++it)
				{
					if (it->msg != "")
					{
						tempStr += it->msg + " ";
						break;
					}
				}
				if (tempStr == "")
				{
					tempStr = Constants::DEFAULT_COPY_MESSAGE;
				}
				OutputString(outbuf, "\"" + tempStr	+ string("\" appears in the background"));
				OutputCursorCoords(outbuf, (xcorner + startx), false, linespacing, true);

				if (cci->troymark_on)
				{
					OutputString(outbuf, "TROYmark");
					OutputString(outbuf,
							"\x1b(s"
									+ StringConverterHelper::toString(
											(tempheight > 5) ?
													tempheight - 3 : 2)
									+ "v3B");

					OutputCursorCoords(outbuf, 0, true, -15, true);
					OutputString(outbuf, "TM");
					OutputCursorCoords(outbuf, 0, true, 15, true);
					OutputString(outbuf,
							"\x1b(s"
									+ StringConverterHelper::toString(
											tempheight) + "v0B");
					if (tabstop == 0)
					{
						OutputString(outbuf, " - ");
					}
					else
					{
						OutputCursorCoords(outbuf, (xcorner + tabstop), false, 0, true);
					}
					if (cci->include_dynamic_msg)
						OutputString(outbuf,
								"Diagonal repeating \"watermark\" consisting of variable data from the document, located on front or back of page");
					else
						OutputString(outbuf,
								"Diagonal repeating \"watermark\" consisting of static text, located on front or back of page");
					OutputCursorCoords(outbuf, (xcorner + startx), false, linespacing, true);
				}

				if ((((cci->PantographConfig & Constants::CFG_BORDER_MASK) != 0)
					&& ((cci->BorderString).length() > 0))
						|| (((cci->PantographConfig
								& Constants::CFG_SIGLINE_MASK) != 0)
								&& ((cci->SigString).length() > 0)))
				{
					OutputString(outbuf, "Micro Print");
					if (tabstop == 0)
					{
						OutputString(outbuf, " - ");
					}
					else
					{
						OutputCursorCoords(outbuf, (xcorner + tabstop), false, 0, true);
					}
					OutputString(outbuf,
							"Area of very small print which must be read under magnification");
					if ((((cci->PantographConfig & Constants::CFG_BORDER_MASK)
							> 0) && ((cci->BorderString).length() > 0)
							&& (cci->BorderString[0] != '!'))
							|| (((cci->PantographConfig
									& Constants::CFG_SIGLINE_MASK) > 0)
									&& ((cci->SigString).length() > 0)
									&& (cci->SigString[0] != '!')))
					{
						OutputString(outbuf, " found wherever the ");
						OutputString(outbuf, "\x1b(s1p5v1s3b16602TM");
						OutputCursorCoords(outbuf, -3, true, 0, true);
						OutputString(outbuf, "P");
						OutputString(outbuf, holdFontStr);
						OutputString(outbuf, "\x1b(s" + StringConverterHelper::toString(tempheight) + "V");
						OutputString(outbuf, " symbol appears");
					}
					OutputCursorCoords(outbuf, (xcorner + startx), false, linespacing, true);
				}
				OutputString(outbuf, "Security Features Warning Box");
				if (tabstop == 0)
				{
					OutputString(outbuf, " - ");
				}
				else
				{
					OutputCursorCoords(outbuf, (xcorner + tabstop), false, 0, true);
				}
				OutputString(outbuf, "Warning Box describing the security features contained within this document");
			}
			else
			{
				tempStr = "";
				while (strIndex < (cci->CustomWarningString).length())
				{
					ch = cci->CustomWarningString[strIndex];
					if ((ch == '\n') || (ch == '\r'))
						OutputCursorCoords(outbuf, (xcorner + startx), false, linespacing, true);
					else if (ch == '\t')
					{
						if (tabstop == 0)
							tempStr += " - ";
						else
							OutputCursorCoords(outbuf, (xcorner + tabstop), false, 0, true);
					}
					else if (static_cast<char>(ch) == 0x7F)
					{
						ch = cci->CustomWarningString[++strIndex];
						switch (ch)
						{
						case 'B':
							tempStr += "\x1b(s3B";
							break;
						case 'b':
							tempStr += "\x1b(s0B";
							break;
						case 'I':
							tempStr += "\x1b(s1S";
							break;
						case 'i':
							tempStr += "\x1b(s0S";
							break;
						case 'U':
							tempStr += "\x1b&d0D";
							break;
						case 'u':
							tempStr += "\x1b&d@";
							break;
						case 'X':
							OutputCursorCoords(outbuf, 3, true, 0, true);
							break;
						case 'x':
							OutputCursorCoords(outbuf, -3, true, 0, true);
							break;
						case 'Y':
							OutputCursorCoords(outbuf, 0, true, 3, true);
							break;
						case 'y':
							OutputCursorCoords(outbuf, 0, true, -3, true);
							break;
						case 'V':
							tempStr += "\x1b(s"	+ StringConverterHelper::toString(fontheight) + "V";
							tempheight = fontheight;
							break;
						case '+':
							tempheight++;
							tempStr += "\x1b(s"
									+ StringConverterHelper::toString(tempheight) + "V";
							break;
						case '-':
							tempheight--;
							tempStr += "\x1b(s"	+ StringConverterHelper::toString(tempheight) + "V";
							break;
						case 'M':
							//Do nothing
							break;
						case 'P':
							//Do nothing
							break;
						default:
							break;
						}
					}
					else if (ch >= ' ')
					{
						tempStr += ch;
					}
					strIndex++;
				}
				OutputString(outbuf, tempStr);
			}
		}
		if (warningboxbelow)
		{
			return boxheight + boxoffset;
		}
		else
		{
			return (boxheight + boxoffset) * -1;
		}
	}

	void BuildPantograph::GetWarningBoxValue(const string &str, unsigned int &index,
			int &val)
	{
		char ch;
		int charVal;
		while (index < str.length())
		{
			ch = str[index];
			if ((ch >= '0') && (ch <= '9'))
			{
				string tmp = "";
				tmp += ch;
				charVal = StringConverterHelper::fromString<int>(tmp);
				if (val < 1000)
					val = val * 10 + charVal;
			}
			else
				break;
			index++;
		}

	}
	int BuildPantograph::SetPatternDensity(int patnum, int density)
	{
		int mod = (density - 1) % 250 + 1;
		if (mod < 1)
			mod = Constants::DefaultDensityPattern[patnum - 1];
		return mod;
	}
	bool BuildPantograph::DoneWithCells(CustomConfigurationInternal *cci, int &Xpos,
			int &Ypos, unsigned int &CellListIdx, unsigned int &CellList1st,
			int offsetX)
	{
		Xpos += cci->PantoCellWidth;
		if (Xpos < (cci->InclRegion.left + cci->InclRegion.width))
		{
			if ((cci->PantographConfig & Constants::CFG_ENABLE_MASK)
					!= Constants::CFG_ENABLE_HORIZ)
			{
				CellListIdx++;
				if (CellListIdx >= (cci->CellList).size())
					CellListIdx = 0;
			}
		}
		else
		{
			Ypos += cci->PantoCellHeight;
			if (Ypos < (cci->InclRegion.top + cci->InclRegion.height))
			{
				Xpos = cci->InclRegion.left + offsetX;
				switch (cci->PantographConfig & Constants::CFG_ENABLE_MASK)
				{
				case 0:
					//TBD Error
				case Constants::CFG_ENABLE_CHESS:
					CellList1st++;
					if (CellList1st >= (cci->CellList).size())
						CellList1st = 0;
					CellListIdx = CellList1st;
					break;
				case Constants::CFG_ENABLE_HORIZ:
					CellListIdx++;
					if (CellListIdx >= (cci->CellList).size())
						CellListIdx = 0;
					break;
				case Constants::CFG_ENABLE_VERT:
					CellListIdx = 0;
					break;
				}
			}
			else
				return true;
		}
		return false;
	}
	void BuildPantograph::UpdateValuesWithCustom(CustomConfiguration *cc, CustomConfigurationInternal *cci)
	{
		string temp_str;
		int temp_str_len;

		//Page Setup
		if (cc->PageSize !=ptNone)
			cci->PageSize = cc->PageSize;
		if (cc->PageOrientation != poNone)
			cci->PageOrientation = cc->PageOrientation;
		// convert cc->PantographColor (enum) to cci->PantographColor (int)
		cci->PantographColor = static_cast<int>(cc->PantographColor);

		//Pantograph 2 goes here

		//Georgia seal
		cci->include_georgia_seal = cc->GeorgiaSealEnabled;
		if (cci->include_georgia_seal)
		{
			cci->georgia_x_loc = BuildPantograph::IntegerOf(cc->GeorgiaSealXLoc);
			cci->georgia_y_loc = BuildPantograph::IntegerOf(cc->GeorgiaSealYLoc);
			cci->georgia_seal_font_file = cc->GeorgiaSealFile;
			cci->georgia_seal_string = cc->GeorgiaSealString;
		}

		if ((!cc->UseDefaultInclusionForPaperSize)
				&& cc->InclusionRegion.XAnchor >= 0)
		{
			cci->incl_defined = true;
			PantographRegionType prt;
			prt.top = cc->InclusionRegion.YAnchor;
			prt.left = cc->InclusionRegion.XAnchor;
			prt.width = cc->InclusionRegion.Width;
			prt.height = cc->InclusionRegion.Height;
			cci->InclRegion = prt;
			if (cci->include_georgia_seal)
			{
				if (cci->georgia_x_loc < 0)
				{
					cci->georgia_x_loc = cci->InclRegion.left
							+ cci->InclRegion.width - 400; //right edge minus just over 1/2 inch
				}
				if (cci->georgia_y_loc < 0)
				{
					cci->georgia_y_loc = cci->InclRegion.top
							+ cci->InclRegion.height + 100;
				}
			}

		}
		else
		{
			cci->incl_defined = false;
		}

		//Exclusion Region
		// List<PantographRegionObjectType> ExclusionRegions
		int size = (cc->ExclusionRegions).size();
		if (size > 0)
		{
			(cci->ExclRegion).clear();
			PantographRegionType exc;
			for (vector<PantographRegionObjectType>::iterator it = (cc->ExclusionRegions).begin(); it < (cc->ExclusionRegions).end(); ++it)
			{
				exc.top = it->YAnchor;
				exc.left = it->XAnchor;
				exc.height = it->Height;
				exc.width = it->Width;
				(cci->ExclRegion).push_back(exc);
			}
		}

		//Main Pantograph Configuration
		cci->PantographConfig = BuildPantograph::IntegerOf(cc->PantographConfiguration);
		//Pantograph Font
		if (cc->PantographFont != "")
		{
			cci->PantographFontSelectString = BuildPantograph::str_replace(cc->PantographFont,
					" ", "\x1b");
		}

		//Cell List
		if ((cc->CellList).size() > 0)
		{
			cci->CellList.clear();

			PantographCellDescriptorType item;
			for (vector<PantographCellDescriptorType>::iterator it = cc->CellList.begin(); it != cc->CellList.end(); ++it)
			{
				item.pidx = it->pidx;
				item.msg = it->msg;
				(cci->CellList).push_back(item);
			}
		}

		//Cell Region
		if (cc->PantographTextOffsetX > -1)	cci->PantoTextOffsetX = cc->PantographTextOffsetX;
		if (cc->PantographTextOffsetY > -1)	cci->PantoTextOffsetY = cc->PantographTextOffsetY;
		if (cc->PantographCellWidth > -1) cci->PantoCellWidth = cc->PantographCellWidth;
		if (cc->PantographCellHeight > -1) cci->PantoCellHeight = cc->PantographCellHeight;
		if (cc->PantographXPlus > -1) cci->PantographXPlus = cc->PantographXPlus;
		if (cc->PantographXMinus > -1) cci->PantographXMinus = cc->PantographXMinus;
		if (cc->PantographYPlus > -1) cci->PantographYPlus = cc->PantographYPlus;
		if (cc->PantographYMinus > -1) cci->PantographYMinus = cc->PantographYMinus;

		cci->troymark_on = cc->TROYmarkOn;
		cci->include_dynamic_msg = cc->UseDynamicTmMsg;

		//Interference
		if (cc->InterferenceFontFilename != "")
			cci->interfFont = cc->InterferenceFontFilename;
		if (cc->InterferenceString != "")
		{
			cci->CustomInterfString = cc->InterferenceString;
		}
		else
		{
			if ((cc->InterferencePatternId > 0) && (cc->InterferencePatternId <= 20))
				cci->InterferenceStr = Constants::InterferencePatterns[cc->InterferencePatternId - 1];
		}
		if (cc->InterferenceHStep > -1) cci->CustomInterfHStep = cc->InterferenceHStep;
		if (cc->InterferenceHStart > -1) cci->CustomInterfHStart = cc->InterferenceHStart;
		if (cc->InterferenceHStop > -1) cci->CustomInterfHStop = cc->InterferenceHStop;
		if (cc->InterferenceVStep > -1) cci->CustomInterfVStep = cc->InterferenceVStep;
		if (cc->InterferenceVStart > -1) cci->CustomInterfVStart = cc->InterferenceVStart;
		if (cc->InterferenceVStop > -1) cci->CustomInterfVStop = cc->InterferenceVStop;
		if (cc->InterferenceFontSelection != "") cci->CustomInterfFontSel = cc->InterferenceFontSelection;
		if (cc->MicroPrintCharWidth > 0) cci->MicroPrintCharWidth = cc->MicroPrintCharWidth;

		//Signature
		if (cc->SignatureLineOffsetX > -1)
			cci->SigOffsetX = cc->SignatureLineOffsetX;
		if (cc->SignatureLineOffsetY > -1)
			cci->SigOffsetY = cc->SignatureLineOffsetY;
		if (cc->SignatureLineLength > -1)
			cci->SigLength = cc->SignatureLineLength;
		temp_str = cc->SignatureLineString;
		temp_str_len = temp_str.length();
		if (temp_str != "")
		{
			if ((temp_str_len == 1) && (temp_str == "!"))
			{
				cci->SigString = "!" + Constants::SIGLINE;
			}
			else
			{
				cci->SigString = cc->SignatureLineString;
			}
		}
		//Border
		if (cc->BorderString.length() > 0)
		{
			if (((cc->BorderString).length() == 1) && (cc->BorderString == "!"))
			{
				cci->BorderString = "!" + Constants::BORDERLINE;
			}
			else
			{
				cci->BorderString = cc->BorderString;
			}
		}
		else
			cci->BorderString = Constants::BORDERLINE;
		//Warning Box
		if (cc->WarningBoxString != "")
		{
			cci->CustomWarningString = cc->WarningBoxString;
		}
		cci->WarningBoxSize = cc->WarningBoxSize;

		cci->PatternDensity[0] = BuildPantograph::SetPatternDensity(1, cc->DensityPattern1);
		cci->PatternDensity[1] = BuildPantograph::SetPatternDensity(2, cc->DensityPattern2);
		cci->PatternDensity[2] = BuildPantograph::SetPatternDensity(3, cc->DensityPattern3);
		cci->PatternDensity[3] = BuildPantograph::SetPatternDensity(4, cc->DensityPattern4);
		cci->PatternDensity[4] = BuildPantograph::SetPatternDensity(5, cc->DensityPattern5);
		cci->PatternDensity[5] = BuildPantograph::SetPatternDensity(6, cc->DensityPattern6);
		cci->PatternDensity[6] = BuildPantograph::SetPatternDensity(7, cc->DensityPattern7);
		cci->PatternDensity[7] = BuildPantograph::SetPatternDensity(8, cc->DensityPattern8);
		cci->PatternDensity[8] = BuildPantograph::SetPatternDensity(9, cc->DensityPattern9);

		cci->PrintMp = cc->IncludeMp;

		//KLK Added May 15, 2012 for Pantograph 2 Phase 1
		//    Darkness factor makes the pantographs darker

		//  C#: if ((cc.BgDarknessFactor > cci.DarknessFactorMap.Length) || (cc.BgDarknessFactor < 2))
		if (cc->BgDarknessFactor > (sizeof(cci->DarknessFactorMap) / sizeof(cci->DarknessFactorMap[0])) || (cc->BgDarknessFactor < 2))
		{
			cci->DarknessFactor = 1;
		}
		else
		{
			cci->DarknessFactor = cc->BgDarknessFactor;
		}

		BuildPantograph::SetupPantographPage(cci);
		//incl_defined is true if a custom inclusion region is used
		if (!cci->incl_defined)
		{
			/* If a MicroPrint border is to be printed, move all four sides of the
			 default inclusion region in enough to make room for it. */
			if ((cci->PantographConfig & Constants::CFG_BORDER_MASK) > 0)
			{
				cci->InclRegion.left += 20;
				cci->InclRegion.top += 50;
				if (cci->InclRegion.width > 40)
				{
					cci->InclRegion.width -= 40;
				}
				else
				{
					cci->InclRegion.width = 0;
				}
				if (cci->InclRegion.height > 100)
				{
					cci->InclRegion.height -= 100;
				}
				else
				{
					cci->InclRegion.height = 0;
				}
			}
			/* If a warning box is to be printed, move the top or bottom of the
			 default inclusion region in to make room for it. */
			if ((cci->PantographConfig & Constants::CFG_BOX_MASK) > 0)
			{
				//Call the Warning Box function to get the y dimension without actually outputting PCL
				ofstream nullbuf;
				int ydim = BuildPantograph::WarningBox(nullbuf, false, cci);
				if (ydim < 0)
				{
					ydim = -ydim;
					cci->InclRegion.top += ydim;
				}
				if (cci->InclRegion.height > static_cast<int>(ydim))
				{
					cci->InclRegion.height -= ydim;
				}
				else
				{
					cci->InclRegion.height = 0;
				}
			}
		}
	}
	bool BuildPantograph::str_endswith(string str, const string &find)
	{
		size_t i = str.find_last_of(find);
		return (i == (str.length() - 1));
	}
	string BuildPantograph::str_replace(string str, const string &find,
			const string &replace)
	{
		size_t index = 0;
		while (true)
		{
			index = str.find(find, index);
			if (index == string::npos)
				break;
			str.replace(index, find.length(), replace);
			++index;
		}
		return str;
	}
	int BuildPantograph::IntegerOf(const string &str)	{
		try
		{
			std::istringstream stream(str);
			long x;
			char test;

			if ((!(stream >> x)) || (stream >> test))
				return -1;
			else
				return x;
		}
		catch (...)
		{
			return -1;
		}
	}

	void BuildPantograph::LoadPantograph2Pattern(ofstream &outbuf, char Pattern[])
	{
		/*******
		OutputString(outbuf,
			"\x1B*c" + StringConverterHelper::toString((Pattern).length()) + "W");
		//Output the data
		outbuf.write(Pattern, (Pattern).length());
		********/
	}
	void BuildPantograph::OutputPantographColor(ofstream &outbuf, int color)
	{
		OutputString(outbuf,
				"\x1b*r3U\x1B*v" + StringConverterHelper::toString(color) + "S");
	}
	void BuildPantograph::OutputCursorCoords(ofstream &outbuf, int x, bool relx, int y, bool rely)
	{
		string xStr = relx ? SignedNumStr(x) : StringConverterHelper::toString(x);
		string yStr = rely ? SignedNumStr(y) : StringConverterHelper::toString(y);
		bool xZero = (xStr == "+0") ? true : false;
		bool yZero = (yStr == "+0") ? true : false;

		if (xZero && yZero) return;

		string tempStr;
		if (!xZero && !yZero)
			tempStr = ("\x1b*p" + xStr + "x" + yStr + "Y");
		else if (yZero)
			tempStr = ("\x1b*p" + xStr + "X");
		else
			tempStr = ("\x1b*p" + yStr + "Y");

		OutputString(outbuf, tempStr);
	}
	string BuildPantograph::SignedNumStr(int num)
	{
		string str = StringConverterHelper::toString(num);
		if (num >= 0)  // zero gets plus sign to treat it as relative, not absolute, curosor position
			return ("+" + str);
		else
			return str;
	}

	void BuildPantograph::OutputRectangleSize(ofstream &outbuf, int Horiz, int Vert)
	{
		string tempStr = "\x1b*c" + StringConverterHelper::toString(Horiz)
				+ "a" + StringConverterHelper::toString(Vert) + "B";
		OutputString(outbuf, tempStr);
	}
	void BuildPantograph::OutputPrintDirection(ofstream &outbuf, int dir)
	{
		OutputString(outbuf,
				"\x1b&a" + StringConverterHelper::toString(dir) + "P");
	}
	void BuildPantograph::OutputMacroCommands(ofstream &outbuf, int cmd,
			const string &end)
	{
		OutputString(outbuf,
				"\x1b&f" + StringConverterHelper::toString(cmd) + end);
	}
	void BuildPantograph::OutputPatternIdCommand(ofstream &outbuf, int ptrn)
	{
		OutputString(outbuf,
				"\x1b*c" + StringConverterHelper::toString(ptrn) + "G");
	}
	void BuildPantograph::OutputFontId(ofstream &outbuf, int id)
	{
		OutputString(outbuf,
				"\x1b*c" + StringConverterHelper::toString(id) + "D");
	}
	void BuildPantograph::OutputString(ofstream &outbuf, const string &str)
	{
		const char *a = &str[0];
		outbuf.write(a, str.size());
	}

		/*************************
				void LoadPantographPattern(int PatternPairID, int PatternID)
		{
			int byteidx, density, noise;
			byte patbyte;

			byte[] UserDefPattern = new byte[7] { 0x1B, 0x2A, 0x63, 0x31, 0x34, 0x30, 0x57 }; //<ESC>*c140W
			byte[] Pattern = new byte[12] { 0x14, 0x00, 0x01, 0x00, 0x00, 0x20, 0x00, 0x20, 0x02, 0x58, 0x02, 0x58 }; //
			outbuf.Write(UserDefPattern, 0, UserDefPattern.Length);
			outbuf.Write(Pattern, 0, Pattern.Length);

			byte[] PgPattern = PantographPattern[PatternID];
			//If the Pattern Pair is >0 then the PatternId is set to 0
			if (PatternID > 0 && PatternID != 11)
			{
				outbuf.Write(PantographPattern[PatternID], 0, PantographPattern[PatternID].Length);
			}
			else
			{
				density = GetPatternDensity(PatternPairID) - 1;
				noise = density / 50;
				density %= 50;

				for (byteidx = 0; byteidx < 128; byteidx++)
				{
					if (density < 13)
					{
						if ((byteidx & 4) > 0)
						{
							patbyte = 0;
						}
						else
						{
							patbyte = PgPattern[byteidx];
							if ((density + 32) < BgWeight[byteidx])
							{
								patbyte = 0;
							}
						}
					}
					else
					{
						patbyte = PgPattern[byteidx];
						if (density < BgWeight[byteidx])
						{
							patbyte = 0;
						}
						else if (density > 45)
						{
							if ((byteidx == 4) || (byteidx == 70))
							{
								patbyte = 0x84;
							}
							if (density > 46 && ((byteidx == 2) || (byteidx == 64)))
							{
								patbyte = 0xC0;
							}
							if (density > 47 && ((byteidx == 34) || (byteidx == 96)))
							{
								patbyte = 0x81;
							}
							if (density > 48 && ((byteidx == 29) || (byteidx == 95)))
							{
								patbyte = 0x90;
							}
						}
					}
					switch (noise)
					{
						case 1:
							switch (byteidx)
							{
								case 70:
									patbyte |= 0xC0;
									break;
								case 74:
									patbyte |= 0xE0;
									break;
								default:
									break;
							}
							break;
						case 2:
							switch (byteidx)
							{
								case 66:
								case 70:
								case 74:
									patbyte |= 0xE0;
									break;
								default:
									break;
							}
							break;
						case 3:
							switch (byteidx)
							{
								case 4:
								case 70:
									patbyte |= 0xC0;
									break;
								case 8:
								case 74:
									patbyte |= 0xE0;
									break;
								default:
									break;
							}
							break;
						case 4:
							switch (byteidx)
							{
								case 0:
								case 4:
								case 8:
								case 66:
								case 70:
								case 74:
									patbyte |= 0xE0;
									break;
								default:
									break;
							}
							break;    
					}
					outbuf.Write(patbyte);
				}    
			}
		}
*********************************/


void BuildPantograph::LoadPantographPattern(ofstream &outbuf,
	CustomConfigurationInternal *cci, int PatternPairID, int PatternID)
{
	int byteidx, density, noise;
		char patbyte;
		char patbytes[128];
		string str(Constants::UserDefPattern);
		char *a = &str[0];
		outbuf.write(a, str.size());
		char myPattern[12] =
				{ 0x14, 0x00, 0x01, 0x00, 0x00, 0x20, 0x00, 0x20, 0x02, 0x58,
						0x02, 0x58 };
		outbuf.write(myPattern, 12);

		char *PgPattern = (char *)Constants::PantographPattern[PatternID];
		//If the Pattern Pair is >0 then the PatternId is set to 0
		if (PatternID > 0 && PatternID != 11)
		{
			char* ptr1 = (char *)Constants::PantographPattern[PatternID];
			outbuf.write(ptr1, sizeof(Constants::PantographPattern[PatternID]) / sizeof(Constants::PantographPattern[PatternID][0]));
		}
		else
		{
			if ((PatternPairID < 1) || (PatternPairID > 9))
				density = 1000;
			else
				density = cci->PatternDensity[PatternPairID-1] - 1;
			noise = density / 50;
			density %= 50;

			int pos = 0;
			for (byteidx = 0; byteidx < 128; byteidx++)
			{
				if (density < 13)
				{
					if ((byteidx & 4) > 0)
					{
						patbyte = 0;
					}
					else
					{
						patbyte = PgPattern[byteidx];
						if ((density + 32) < cci->BgWeight[byteidx])
						{
							patbyte = 0;
						}
					}
				}
				else
				{
					patbyte = PgPattern[byteidx];
					if (density < cci->BgWeight[byteidx])
					{
						patbyte = 0;
					}
					else if (density > 45)
					{
						if ((byteidx == 4) || (byteidx == 70))
						{
							patbyte = 0x84;
						}
						if (density > 46 && ((byteidx == 2) || (byteidx == 64)))
						{
							patbyte = 0xC0;
						}
						if (density > 47
								&& ((byteidx == 34) || (byteidx == 96)))
						{
							patbyte = 0x81;
						}
						if (density > 48
								&& ((byteidx == 29) || (byteidx == 95)))
						{
							patbyte = 0x90;
						}
					}
				}
				switch (noise)
				{
				case 1:
					switch (byteidx)
					{
					case 70:
						patbyte |= 0xC0;
						break;
					case 74:
						patbyte |= 0xE0;
						break;
					default:
						break;
					}
					break;
				case 2:
					switch (byteidx)
					{
					case 66:
					case 70:
					case 74:
						patbyte |= 0xE0;
						break;
					default:
						break;
					}
					break;
				case 3:
					switch (byteidx)
					{
					case 4:
					case 70:
						patbyte |= 0xC0;
						break;
					case 8:
					case 74:
						patbyte |= 0xE0;
						break;
					default:
						break;
					}
					break;
				case 4:
					switch (byteidx)
					{
					case 0:
					case 4:
					case 8:
					case 66:
					case 70:
					case 74:
						patbyte |= 0xE0;
						break;
					default:
						break;
					}
					break;
				}

				patbytes[pos] = patbyte;
				pos++;
			}
			outbuf.write(patbytes, pos);
		}
	}
	void BuildPantograph::SetupPantographPage(CustomConfigurationInternal *cci)
	{
		if ((cci->PageOrientation == poPortrait) ||
			(cci->PageOrientation == poReversePortrait))
		{
			switch (cci->PageSize)
			{
			case ptExecutive: // Executive
				cci->PageWidth = Constants::WIDTH_EXECUTIVE - 100;
				cci->PageHeight = Constants::HEIGHT_EXECUTIVE;
				break;
			case ptLetter: // Letter
				cci->PageWidth = Constants::WIDTH_LETTER - 100;
				/* cci->PageHeight = HEIGHT_LETTER - 200; */
				/* JCTEST -- for Ken's demo */
				cci->PageHeight = Constants::HEIGHT_LETTER;
				break;
			case ptLegal: // Legal
				cci->PageWidth = Constants::WIDTH_LEGAL - 100;
				cci->PageHeight = Constants::HEIGHT_LEGAL;
				break;
			case ptExecutive_JIS: // Executive JIS
				cci->PageWidth = Constants::WIDTH_JIS_EXEC - 100;
				cci->PageHeight = Constants::HEIGHT_JIS_EXEC;
				break;
			case ptA6: // A6
				cci->PageWidth = Constants::WIDTH_A6 - 100;
				cci->PageHeight = Constants::HEIGHT_A6;
				break;
			case ptA5: // A5
				cci->PageWidth = Constants::WIDTH_A5 - 100;
				cci->PageHeight = Constants::HEIGHT_A5;
				break;
			case ptA4: // A4
				cci->PageWidth = Constants::WIDTH_A4 - 100;
				cci->PageHeight = Constants::HEIGHT_A4;
				break;
			case ptJIS_B6: // JIS B6
				cci->PageWidth = Constants::WIDTH_JIS_B6 - 100;
				cci->PageHeight = Constants::HEIGHT_JIS_B6;
				break;
			case ptJIS_B5: // JIS B5
				cci->PageWidth = Constants::WIDTH_JIS_B5 - 100;
				cci->PageHeight = Constants::HEIGHT_JIS_B5;
				break;
			case ptHagaki_Postcard: // Hagaki Postcard
				cci->PageWidth = Constants::WIDTH_HAGAKI - 100;
				cci->PageHeight = Constants::HEIGHT_HAGAKI;
				break;
			case ptOufuku_Hagaki_Postcard: // Oufuku-Hagaki Postcard
				cci->PageWidth = Constants::WIDTH_OUFUKU_HAGAKI - 100;
				cci->PageHeight = Constants::HEIGHT_OUFUKU_HAGAKI;
				break;
			case ptMonarch_Envelope: // Monarch Envelope
				cci->PageWidth = Constants::WIDTH_MONARCH - 100;
				cci->PageHeight = Constants::HEIGHT_MONARCH;
				break;
			case ptCOM10_Envelope: // COM 10 Envelope
				cci->PageWidth = Constants::WIDTH_COM_10 - 100;
				cci->PageHeight = Constants::HEIGHT_COM_10;
				break;
			case ptDL_Envelope: // DL Envelope
				cci->PageWidth = Constants::WIDTH_DL - 100;
				cci->PageHeight = Constants::HEIGHT_DL;
				break;
			case ptC5_Envelope: // C5 Envelope
				cci->PageWidth = Constants::WIDTH_C5 - 100;
				cci->PageHeight = Constants::HEIGHT_C5;
				break;
			case ptB5_Envelope: // B5 Envelope
				cci->PageWidth = Constants::WIDTH_B5 - 100;
				cci->PageHeight = Constants::HEIGHT_B5;
				break;
			default:
				break;
			}
			if (!cci->incl_defined)
			{
				cci->InclRegion.left = 0;
				cci->InclRegion.top = 170;
				cci->InclRegion.width = cci->PageWidth;
				cci->InclRegion.height = cci->PageHeight - 180;
			}
		}
		else
		{
			switch (cci->PageSize)
			{
			case ptExecutive: // Executive
				cci->PageWidth = Constants::HEIGHT_EXECUTIVE - 40;
				cci->PageHeight = Constants::WIDTH_EXECUTIVE;
				break;
			case ptLetter: // Letter
				cci->PageWidth = Constants::HEIGHT_LETTER - 40;
				cci->PageHeight = Constants::WIDTH_LETTER;
				break;
			case ptLegal: // Legal
				cci->PageWidth = Constants::HEIGHT_LEGAL - 40;
				cci->PageHeight = Constants::WIDTH_LEGAL;
				break;
			case ptExecutive_JIS: // Executive JIS
				cci->PageWidth = Constants::HEIGHT_JIS_EXEC - 40;
				cci->PageHeight = Constants::WIDTH_JIS_EXEC;
				break;
			case ptA6: // A6
				cci->PageWidth = Constants::HEIGHT_A6 - 40;
				cci->PageHeight = Constants::WIDTH_A6;
				break;
			case ptA5: // A5
				cci->PageWidth = Constants::HEIGHT_A5 - 40;
				cci->PageHeight = Constants::WIDTH_A5;
				break;
			case ptA4: // A4
				cci->PageWidth = Constants::HEIGHT_A4 - 40;
				cci->PageHeight = Constants::WIDTH_A4;
				break;
			case ptJIS_B6: // JIS B6
				cci->PageWidth = Constants::HEIGHT_JIS_B6 - 40;
				cci->PageHeight = Constants::WIDTH_JIS_B6;
				break;
			case ptJIS_B5: // JIS B5
				cci->PageWidth = Constants::HEIGHT_JIS_B5 - 40;
				cci->PageHeight = Constants::WIDTH_JIS_B5;
				break;
			case ptHagaki_Postcard: // Hagaki Postcard
				cci->PageWidth = Constants::HEIGHT_HAGAKI - 40;
				cci->PageHeight = Constants::WIDTH_HAGAKI;
				break;
			case ptOufuku_Hagaki_Postcard: // Oufuku-Hagaki Postcard
				cci->PageWidth = Constants::HEIGHT_OUFUKU_HAGAKI - 40;
				cci->PageHeight = Constants::WIDTH_OUFUKU_HAGAKI;
				break;
			case ptMonarch_Envelope: // Monarch Envelope
				cci->PageWidth = Constants::HEIGHT_MONARCH - 40;
				cci->PageHeight = Constants::WIDTH_MONARCH;
				break;
			case ptCOM10_Envelope: // COM 10 Envelope
				cci->PageWidth = Constants::HEIGHT_COM_10 - 40;
				cci->PageHeight = Constants::WIDTH_COM_10;
				break;
			case ptDL_Envelope: // DL Envelope
				cci->PageWidth = Constants::HEIGHT_DL - 40;
				cci->PageHeight = Constants::WIDTH_DL;
				break;
			case ptC5_Envelope: // C5 Envelope
				cci->PageWidth = Constants::HEIGHT_C5 - 40;
				cci->PageHeight = Constants::WIDTH_C5;
				break;
			case ptB5_Envelope: // B5 Envelope
				cci->PageWidth = Constants::HEIGHT_B5 - 40;
				cci->PageHeight = Constants::WIDTH_B5;
				break;
			default:
				break;
			}
			if (!cci->incl_defined)
			{
				cci->InclRegion.left = 70;
				cci->InclRegion.top = 120;
				cci->InclRegion.width = cci->PageWidth - 120;
				cci->InclRegion.height = cci->PageHeight - 40;
			}
		}
	}
