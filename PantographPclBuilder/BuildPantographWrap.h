#pragma once

using namespace System;

namespace BuildPantographWrap
{
	public ref class Wrapper
	{
	public:
		int ManagedCreatePantograph(String ^ basepath);
		int ManagedCreatePantograph(String ^ basepath, String ^ configpath, String ^ datapath);
		String ^ ManagedMakeMpLine(int ptsize, String ^ MpStr, int XAnchor, int YAnchor, int Width, int Height, String ^ fontFileName);
		String ^ ManagedMakeMpLine(int ptsize, String ^ MpStr, int XAnchor, int YAnchor, int Width, int Height, String ^ fontFileName, 
			bool PrintMp, int newcharwidth, bool IncludeFonts);
	};
}
