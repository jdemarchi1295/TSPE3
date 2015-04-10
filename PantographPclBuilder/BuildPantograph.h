#pragma once

#define NULLPTR 0
#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <string.h>
#include <time.h>
#include <math.h>
#include <algorithm>
#include <string>
#include <stdexcept>
#include <map>
#include <vector>
#include "Globals.h"
#include "Constants.h"
#include "Markup.h"
#include "CustomConfiguration.h"
#include "stringconverter.h"

using namespace std;

class BuildPantograph
{
public:
	BuildPantograph();
	bool seal_loaded, interf_loaded, mp5_loaded, mp6_loaded;
	CustomConfiguration *cc;
public:
	int CreatePantographTest();
	int CreatePantograph(const string &basePath);
	int CreatePantograph(const string &basePath, const string &configPath, const string &dataPath);
	string MakeMpLine(int ptsize, const string &MpStr, int XAnchor, int YAnchor, int Width, int Height, const string &fontFileName, bool PrintMp = true,
		int newcharwidth = 0, bool IncludeFonts = false);
private:
bool CreatePantographPcl(const string &basePath, const string &dataPath, const string &fileName,
	CustomConfiguration *customConfig);
bool ReadCustomConfigurationTag(CMarkup &xml, CustomConfiguration *cc);
string toLower(const string& str);
char* readFileIntoBuffer(const string& fn, int& bufsize);
bool loadSoftFont(bool loaded, const string &fn, int fontId,	ofstream &outbuf);
void PrintInterferencePattern(const string &filename, ofstream &outbuf, CustomConfigurationInternal *cci);
void PrintMicroLine(const string &str, ofstream &outbuf, CustomConfigurationInternal *cci, int font, int len, MicroPrintType ltype, int newcharwidth = 0);
int WarningBox(ofstream &outbuf, bool print, CustomConfigurationInternal *cci);
void GetWarningBoxValue(const string &str, unsigned int &index, int &val);
int SetPatternDensity(int patnum, int density);
bool DoneWithCells(CustomConfigurationInternal *cci, int &Xpos,
			int &Ypos, unsigned int &CellListIdx, unsigned int &CellList1st, int offsetX);
void UpdateValuesWithCustom(CustomConfiguration *cc, CustomConfigurationInternal *cci);
bool str_endswith(string str, const string &find);
string str_replace(string str, const string &find,
			const string &replace);
int IntegerOf(const string &str);
void LoadPantograph2Pattern(ofstream &outbuf, char Pattern[]);
void OutputPantographColor(ofstream &outbuf, int color);
void OutputCursorCoords(ofstream &outbuf, int x, bool relx, int y, bool rely);
string SignedNumStr(int num);
void OutputRectangleSize(ofstream &outbuf, int Horiz, int Vert);
void OutputPrintDirection(ofstream &outbuf, int dir);
void OutputMacroCommands(ofstream &outbuf, int cmd, const string &end);
void OutputPatternIdCommand(ofstream &outbuf, int ptrn);
void OutputFontId(ofstream &outbuf, int id);
void OutputString(ofstream &outbuf, const string &str);
void LoadPantographPattern(ofstream &outbuf, CustomConfigurationInternal *cci, int PatternPairID, int PatternID);
void SetupPantographPage(CustomConfigurationInternal *cci);
};
