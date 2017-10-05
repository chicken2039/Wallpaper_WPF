using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    class SettingManager
    {
        /*
         * [INI FIELDS TEMPLATE]
         * Setting.ini
         * [General]
         * RunAtStartup=true/false
         * Volume=int
         * ScreenIndex=int
         * [Video]
         * Path=String
         * [Youtube]
         * ID=String
         * [GIF]
         * Path=String
         * [Morph]
         * Path=String
         * First=String
         * Second=String
         */
        #region INI_APIS
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private static string Read_ini(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int ret = GetPrivateProfileString(section, key, "", temp, 255, Setting.Path);
            return temp.ToString();
        }
        private static void Write_ini(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Setting.Path);
        }
        #endregion

        public static void Initialize()
        {
            Setting.IsStartUp = Convert.ToBoolean(Read_ini("General", "RunAtStartup"));
            Setting.Volume = Convert.ToInt32(Read_ini("General", "Volume"));
            Setting.ScreenIndex = Convert.ToInt32(Read_ini("General", "ScreenIndex"));
            Setting.VideoPath = Read_ini("Video", "Path");
            Setting.YoutubeID = Read_ini("Youtube", "ID");
            Setting.GIFPath = Read_ini("GIF", "Path");
            Setting.MorphID = Read_ini("Morph", "Path");
            Setting.First_Ment = Read_ini("Morph", "First");
            Setting.Second_Ment = Read_ini("Morph", "Second");
        }

        public static void Dispose()
        {
            Write_ini("General", "RunAtStartup", Setting.IsStartUp.ToString());
            Write_ini("General", "Volume", Setting.Volume.ToString());
            Write_ini("General", "ScreenIndex", Setting.ScreenIndex.ToString());
            Write_ini("Video", "Path", Setting.VideoPath);
            Write_ini("Youtube", "ID", Setting.YoutubeID);
            Write_ini("GIF", "Path", Setting.GIFPath);
            Write_ini("Morph", "Path", Setting.MorphID);
            Write_ini("Morph", "First", Setting.First_Ment);
            Write_ini("Morph", "Second", Setting.Second_Ment);
        }
    }
}
