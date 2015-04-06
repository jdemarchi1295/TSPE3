using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TroyPortMonitorService
{
    class FontConfigSet
    {
        List<FontConfig> fontList = new List<FontConfig>();

        public FontConfigSet()
        {
        }

        public bool AddFont(string fontName, string leadingPcl, string trailingPcl, string glyphMapFile, string fontType, bool deleteData, bool useForPassThrough)
        {
            try
            {
                FontConfig newFont;

                newFont = new FontConfig();
                newFont.FontName = fontName;
                newFont.LeadingPCL = leadingPcl;
                newFont.TrailingPCL = trailingPcl;
                newFont.GlyphMapFile = glyphMapFile;
                newFont.FontType = fontType;
                newFont.DeleteData = deleteData;
                newFont.UseWithPassThrough = useForPassThrough;
                if (!(glyphMapFile.EndsWith(@"\")))
                {
                    newFont.LoadGlyphToCharMap(glyphMapFile);
                }

                fontList.Add(newFont);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Add Font. " + ex.Message.ToString(), "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }
        }

        public FontConfig GetFontConfig(string fontName)
        {
            foreach (FontConfig font in fontList)
            {
                if (font.FontName == fontName)
                {
                    return font;
                }
            }
            return null;
        }

        public bool FontInList(string fontName)
        {
            bool retValue = false;
            foreach (FontConfig font in fontList)
            {
                if (font.FontName == fontName)
                {
                    retValue = true;
                }
            }
            return retValue;
        }

        public int GetLeadingPclLength(string fontName)
        {
            FontConfig font;

            if ((font = GetFontConfig(fontName)) == null)
            {
                return -1;
            }
            else
            {
                return font.LeadingPCL.Length;
            }
        }

        public int GetTrailingPclLength(string fontName)
        {
            FontConfig font;

            if ((font = GetFontConfig(fontName)) == null)
            {
                return -1;
            }
            else
            {
                return font.TrailingPCL.Length;
            }
        }


        public bool GetLeadingPCL(string fontName, ref byte[] pcl, int pclLength, ref bool deleteFont)
        {
            try
            {
                FontConfig font;
                bool slashFound = false;
                int arrayIndex = 0;

                if ((font = GetFontConfig(fontName)) == null)
                {
                    return false;
                }
                else
                {
                    if (pclLength > font.LeadingPCL.Length)
                    {
                        return false;
                    }
                    else
                    {
                        if ((font.LeadingPCL.ToUpper() == "/DELETE") || (font.DeleteData))
                        {
                            deleteFont = true;
                            return true;
                        }
                        else
                        {
                            foreach (char fontChar in font.LeadingPCL)
                            {
                                if (fontChar == '/')
                                {
                                    slashFound = true;
                                }
                                else
                                {
                                    if (slashFound)
                                    {
                                        if (fontChar == 'e')
                                        {
                                            pcl[arrayIndex] = 0x1b;
                                        }
                                        else if (fontChar == 'r')
                                        {
                                            pcl[arrayIndex] = 0x0d;
                                        }
                                        else if (fontChar == 'n')
                                        {
                                            pcl[arrayIndex] = 0x0a;
                                        }
                                        else if (fontChar == '0')
                                        {
                                            pcl[arrayIndex] = 0;
                                        }
                                        else if (fontChar == '/')
                                        {
                                            pcl[arrayIndex] = 0x2F;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Invalid character after the slash in FontConfigSet.GetLeadingPcl(). Character " + pcl[arrayIndex], "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                                            return false;
                                        }
                                        slashFound = false;
                                        arrayIndex++;
                                    }
                                    else
                                    {
                                        pcl[arrayIndex] = Convert.ToByte(fontChar);
                                        arrayIndex++;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in FontConfigSet.GetLeadingPCL(). " + ex.Message.ToString(), "TROY Port Monitor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }

        }


    }
}

