using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1
{
    class Setting
    {
        public readonly static string Path = Application.StartupPath + @"\Setting.ini";
        public static bool IsStartUp { get; set; }
        public static int Volume { get; set; }
        public static int ScreenIndex { get; set; }
        public static string VideoPath { get; set; }
        public static string YoutubeID { get; set; }
        public static string GIFPath { get; set; }
        public static string First_Ment { get; set; }
        public static string Second_Ment { get; set; }
        public static string MorphID { get; set; }
    }
}
