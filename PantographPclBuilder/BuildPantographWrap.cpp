// main DLL
#include "BuildPantograph.h"
#include "BuildPantographWrap.h"

#include <string>
#include <iostream>
using namespace std;
using namespace System;

namespace BuildPantographWrap
{
	void MarshalString(String ^ s, string& os)
	{
		using namespace Runtime::InteropServices;
		const char* chars =
			(const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}

	void MarshalString(const string& os, String ^ s)
	{
		s = gcnew String(os.c_str());
	}

	int Wrapper::ManagedCreatePantograph(String ^ basepath)
	{
		string basepathCPP;
		MarshalString(basepath, basepathCPP);
		BuildPantograph bp;
		return bp.CreatePantograph(basepathCPP);
	};

	int Wrapper::ManagedCreatePantograph(String ^ basepath, String ^ configpath, String ^ datapath)
	{
		string basepathCPP;
		MarshalString(basepath, basepathCPP);
		string configpathCPP;
		MarshalString(configpath, configpathCPP);
		string datapathCPP;
		MarshalString(datapath, datapathCPP);

		BuildPantograph bp;
		return bp.CreatePantograph(basepathCPP, configpathCPP, datapathCPP);
	};

	String ^ Wrapper::ManagedMakeMpLine(int ptsize, String ^ MpStr, int XAnchor, int YAnchor, int Width, int Height, String ^ fontFileName)
	{
		return Wrapper::ManagedMakeMpLine(ptsize, MpStr, XAnchor, YAnchor, Width, Height, fontFileName, true, 0, false);
	}

	String ^ Wrapper::ManagedMakeMpLine(int ptsize, String ^ MpStr, int XAnchor, int YAnchor, int Width, int Height, String ^ fontFileName, bool PrintMp,
		int newcharwidth, bool IncludeFonts)
	{
		string MpStrCPP;
		MarshalString(MpStr, MpStrCPP);
		string fontFileNameCPP;
		MarshalString(fontFileName, fontFileNameCPP);

		BuildPantograph bp;
		string retStrCPP = bp.MakeMpLine(ptsize, MpStrCPP, XAnchor, YAnchor, Width, Height, fontFileNameCPP, PrintMp, newcharwidth, IncludeFonts);
		String ^ retStr;
		MarshalString(retStrCPP, retStr);
		return retStr;
	}
}

