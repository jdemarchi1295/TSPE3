#pragma once

#include <string>
#include <vector>
#include "Globals.h"
using namespace std;

	class CustomConfigurationInternal
	{
		//*** PAPER SIZE VARIABLES ***
	public:
		PageType PageSize; //The paper size
		int PageWidth; //Set in SetupPantographPage based on the PageSize
		int PageHeight; //Set in SetupPantographPage based on the PageSize
		PageOrientationType PageOrientation;

		//*** PANTOGRAPH COLOR ***
		int PantographColor;

		//*** PANTOGRAPH CONFIGURATION VARIABLES ***
		int PantographConfig; //Defaulting to everything on and .6 micro prints
		string PantographFontSelectString;
		PantographRegionType InclRegion;
		vector<PantographRegionType> ExclRegion;
		//PantographRegionType[] ExclRegion = new PantographRegionType[Constants.MAX_EXCL_REGIONS];
		//int NumExclRegions = 0;
		//RegionType RegionSelect = RegionType.rtNone;


		//*** Cell Settings ***
		int PantoTextOffsetX; //Distance from the left side of the cell to the first character
		int PantoTextOffsetY; //Distance from the bottom edge of the cell to the pantograph message
		int PantoCellWidth;
		int PantoCellHeight;
		//PantographCellDescriptorType[] CellList = new PantographCellDescriptorType[Constants.MAX_LIST_LENGTH];
		vector<PantographCellDescriptorType> CellList;
		//int CellListSize = 0;
		int PantographXPlus;
		int PantographXMinus;
		int PantographYPlus;
		int PantographYMinus;

		string BorderString;

		//*** Sig Line Settings ***
		int SigOffsetX;
		int SigOffsetY;
		int SigLength;
		string SigString;

		//*** Interference Settings ***
		int CustomInterfHStep;
		int CustomInterfHStart;
		int CustomInterfHStop;
		int CustomInterfVStep;
		int CustomInterfVStart;
		int CustomInterfVStop;
		string CustomInterfFontSel;
		string CustomInterfString;
		string InterferenceStr;

		//*** Warning Box Settings ***
		string CustomWarningString;
		int WarningBoxSize;

		bool PrintMp;
		string fontPath;


		unsigned char BgWeight[128];

		//KLK May 15, 2012 This array maps the users entered darkness value to the pattern array id.  Options are 1 or 2 (maps to 0 or 11) for this initial release
		int DarknessFactor;
		int DarknessFactorMap[3];

		//***** FLAGS *****
		bool pat_loaded[22];
		bool Panto2BckGrdLoaded;
		bool Panto2FrGrdLoaded;
		bool incl_defined;
		bool troymark_on;
		bool include_dynamic_msg;

		bool include_georgia_seal;
		int georgia_x_loc;
		int georgia_y_loc;
		string georgia_seal_font_file;
		string georgia_seal_string;

		string mp5Font;
		string mp6Font;
		string interfFont;

		int PatternDensity[9];

		int MicroPrintCharWidth;

	private:
		void InitializeInstanceFields();

public:
		CustomConfigurationInternal()
		{
			InitializeInstanceFields();
		}
	};

	class CustomConfiguration
	{
	public:
		PageOrientationType PageOrientation;
		PageType PageSize;
		PantographColorType PantographColor;

		//Regions
		bool UseDefaultInclusionForPaperSize;
		PantographRegionObjectType InclusionRegion;
		vector<PantographRegionObjectType> ExclusionRegions;

		//Pantograph/Cell
		string PantographConfiguration; //Note: using string instead of int, leave blank if not using.
		string PantographFont;
		int PantographTextOffsetX;
		int PantographTextOffsetY;
		int PantographCellWidth;
		int PantographCellHeight;
		int PantographXPlus;
		int PantographXMinus;
		int PantographYPlus;
		int PantographYMinus;
		vector<PantographCellDescriptorType> CellList;

		//Interference Pattern
		int InterferencePatternId;
		int InterferenceHStep;
		int InterferenceHStart;
		int InterferenceHStop;
		int InterferenceVStep;
		int InterferenceVStart;
		int InterferenceVStop;
		string InterferenceFontSelection;
		string InterferenceString;
		string InterferenceFontFilename;

		int BgDarknessFactor;

		//Signature Line
		int SignatureLineOffsetX;
		int SignatureLineOffsetY;
		int SignatureLineLength;
		string SignatureLineString;

		//Border
		string BorderString;
		int MicroPrintCharWidth;

		//Warning Box
		string WarningBoxString;
		int WarningBoxSize;

		//Density
		int DensityPattern1;
		int DensityPattern2;
		int DensityPattern3;
		int DensityPattern4;
		int DensityPattern5;
		int DensityPattern6;
		int DensityPattern7;
		int DensityPattern8;
		int DensityPattern9;

		//TROYmark On
		bool TROYmarkOn;
		bool UseDynamicTmMsg;

		bool IncludeMp;
		bool UseDynamicMp;

		//GA Seal
		bool GeorgiaSealEnabled;
		string GeorgiaSealXLoc;
		string GeorgiaSealYLoc;
		string GeorgiaSealFile;
		string GeorgiaSealString;

	private:
		void InitializeInstanceFields();

public:
		CustomConfiguration()
		{
			InitializeInstanceFields();
		}
	};
