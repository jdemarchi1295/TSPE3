#pragma once

#include <string>
using namespace std;

	//The Pantograph Cell. Consists of a Pattern Pair (background and foreground) and a Message.
	//  pidx - the pattern pair id.  1 thru 9 are the normal mode pattern pairs.  
	//                              -1 thru -9 are the reverse mode pattern pairs.
	//                               0 black text on white background
	//                              -0 white text on black background
	//  msg - the message that will be diplayed on the copy
	//  Note:  max number allowed cells is 9
	struct PantographCellDescriptorType
	{
		public:
		int pidx;
		string msg;
	};

	struct PantographRegionType
	{
		public:
		int left;
		int top;
		int width;
		int height;
	};


	//The region definition for the Inclusion, Exclusion, Sig Line and Pantograph Cell settings
	struct PantographRegionObjectType
	{
		public:
		int XAnchor;
		int YAnchor;
		int Width;
		int Height;
	};

	//An enumeration of the differnt Micro Print types
	enum MicroPrintType
	{
		mptBorder,
		mptSigLine
	};

	enum PantographColorType
	{
#define PCT(x,y) x = y,
#include "PantographColorType.txt"
#undef PCT
	};


	//Page types supported
	enum PageType
	{
#define PT(x,y) x = y,
#include "PageType.txt"
#undef PT
	};

	enum PageOrientationType
	{
#define POT(x,y) x = y,
#include "PageOrientationType.txt"
#undef POT
	};
