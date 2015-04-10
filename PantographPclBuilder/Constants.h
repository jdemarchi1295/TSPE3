#pragma once

#include <string>
using namespace std;

	class Constants
	{
	public:
		static const int RES_PERM_FONT_ID_BASE = 10560;
		static const int RES_FONT_ID_T_INTERF = 61;
		static const int RES_FONT_ID_T_MICRO5 = 62;
		static const int RES_FONT_ID_T_MICRO6 = 63;
		static const int RES_FONT_ID_T_SEAL = 64;

		static const string ESC;
		static const string PrintDir0;
		static const string DisableUnderline;
		static const string EraseFill;
		static const string PushCursorPos;
		static const string PopCursorPos;
		static const string SoftFontTemp;
		static const string PrintInBlack;
		static const string BlackFill;
		static const string PageSettings;
		static const string ClearHzMargin; //Clear Horizontal margins
		static const string Res600dpi;
		static const string SetPtrnRefPnt;
		static const string UserDefFill;
		static const string PtrnTransOpaque;
		static const string UserDefPattern;
		static const string HMI; //Horizontal Motion Index

		/* Logical page dimensions in pixels at 600 dpi. */
		static const int WIDTH_EXECUTIVE = 4150;
		static const int HEIGHT_EXECUTIVE = 6100;
		static const int WIDTH_LETTER = 4900;
		static const int HEIGHT_LETTER = 6400;
		static const int WIDTH_LEGAL = 4900;
		static const int HEIGHT_LEGAL = 8200;
		static const int WIDTH_JIS_EXEC = 4900;
		static const int HEIGHT_JIS_EXEC = 7600;
		static const int WIDTH_A6 = 2280;
		static const int HEIGHT_A6 = 3307;
		static const int WIDTH_A5 = 3307;
		static const int HEIGHT_A5 = 4760;
		static const int WIDTH_A4 = 4760;
		static const int HEIGHT_A4 = 6814;
		static const int WIDTH_JIS_B6 = 2843;
		static const int HEIGHT_JIS_B6 = 4114;
		static const int WIDTH_JIS_B5 = 4114;
		static const int HEIGHT_JIS_B5 = 5886;
		static const int WIDTH_HAGAKI = 2178;
		static const int HEIGHT_HAGAKI = 3312;
		static const int WIDTH_OUFUKU_HAGAKI = 3312;
		static const int HEIGHT_OUFUKU_HAGAKI = 4556;
		static const int WIDTH_MONARCH = 2124;
		static const int HEIGHT_MONARCH = 4300;
		static const int WIDTH_COM_10 = 2274;
		static const int HEIGHT_COM_10 = 5500;
		static const int WIDTH_DL = 2414;
		static const int HEIGHT_DL = 5012;
		static const int WIDTH_C5 = 3642;
		static const int HEIGHT_C5 = 5224;
		static const int WIDTH_B5 = 3972;
		static const int HEIGHT_B5 = 5720;

		static const int PAT_ID_BASE = 5000;
		static const int MAX_LIST_LENGTH = 9;
		static const int MAX_EXCL_REGIONS = 4;
		static const int MAX_MSG_STR_LEN = 40;
		static const int MAX_BORDER_STR_LEN = 40;
		static const int MAX_SIG_STR_LEN = 40;
		static const int MAX_FONT_STR_LEN = 40;
		static const int MAX_INTERF_STR_LEN = 128;
		static const int MAX_WARN_STR_LEN = 512;
		static const int MAX_PANTO_PROFILES = 10;

		static const int MAX_WIDTH = 8200;
		static const int MAX_HEIGHT = 8200;
		static const int MIN_CELL_WIDTH = 32;
		static const int MIN_CELL_HEIGHT = 32;
		static const int MIN_INTERF_STEP = 32;
		static const int MAX_INTERF_STEP = 1200;
		static const int MAX_INTERF_START = 1200;
		static const int MAX_INTERF_STOP = 1200;

		static const int CFG_ENABLE_MASK = 0x0003;
		static const int CFG_BORDER_MASK = 0x000C;
		static const int CFG_BOX_MASK = 0x0010;
		static const int CFG_SIGLINE_MASK = 0x0060;
		static const int CFG_DUP_MASK = 0x0080;
		static const int CFG_INT_MASK = 0x0100;
		static const int CFG_DOLST_MASK = 0x0200;
		static const int CFG_NOBG_MASK = 0x0400;
		static const int CFG_ONCE_MASK = 0x0800;
		static const int CFG_COPY_MASK = 0x4000;

		static const int CFG_ENABLE_CHESS = 0x0001;
		static const int CFG_ENABLE_HORIZ = 0x0002;
		static const int CFG_ENABLE_VERT = 0x0003;

		static const int CFG_SIGLINE_SHIFT = 5;
		static const int CFG_BORDER_SHIFT = 2;

		static const string SIGLINE;
		static const string BORDERLINE;

		//KLK ADDED THIS ONE.  DEFAULT WAS HARDCODED TO COPY
		static const string DEFAULT_COPY_MESSAGE;

		static const string InterferencePatterns[20];
		static const int DefaultDensityPattern[9];

		static const char PantographPattern[12][128];

	};
