using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Interop;

namespace WpfApp1
{
    /// <summary>
    /// YoutubeForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class YoutubeForm : Window
    {
        public YoutubeForm(int ownerScreenIndex = 0)
        {
            InitializeComponent();
            OwnerScreenIndex = ownerScreenIndex;
        }
        #region Variables
        private string m_videopath = "";
        public string Videopath { get { return m_videopath; } set { m_videopath = value; } }
        //################################################//
        private bool m_isFixed = false;
        public bool IsFixed
        { get { return m_isFixed; } }
        static IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;//static 붙여야 한다 해서 붙임ㅎ
        HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        //################################################//
        private int m_latestVolume = 100;
        public int Volume
        {
            get
            {
                return m_latestVolume;
            }
            set
            {
                m_latestVolume = value;

            }
        }
        //################################################//
        public Winapi.MONITORINFO OwnerScreen
        {
            get
            {
                if (OwnerScreenIndex < ScreenUtility.Screens.Length)
                    return ScreenUtility.Screens[OwnerScreenIndex];
                return new Winapi.MONITORINFO()
                {
                    rcMonitor = Screen.PrimaryScreen.Bounds,
                    rcWork = Screen.PrimaryScreen.WorkingArea,
                };
            }
        }
        //################################################//
        private int m_ownerScreenIndex = 0;
        public int OwnerScreenIndex
        {
            get { return m_ownerScreenIndex; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value >= Screen.AllScreens.Length)
                    value = 0;

                if (m_ownerScreenIndex != value)
                {
                    m_ownerScreenIndex = value;

                    PinToBackground();
                }
            }
        }
        //################################################//
        private Task m_checkParent = null;
        private bool m_onRunning = false;
        private EventWaitHandle m_waitHandle = null;
        //################################################//
        private readonly object m_lockFlag = new object();
        private bool m_needUpdate = false;
        DispatcherTimer timer = new DispatcherTimer();
        #endregion
        #region Pin_Background
        protected bool PinToBackground()
        {
            m_isFixed = BehindDesktopIcon.FixBehindDesktopIcon(mainWindowSrc.Handle);

            if (m_isFixed)
            {
                ScreenUtility.FillScreen(this, OwnerScreen);
            }


            return m_isFixed;
        }

        protected void CheckParent(object thisHandle)
        {
            IntPtr me = (IntPtr)thisHandle;


            while (m_onRunning)
            {
                bool isChildOfProgman = false;
                var progman = Winapi.FindWindow("Progman", null);
                Winapi.EnumChildWindows(progman, new Winapi.EnumWindowsProc((handle, lparam) =>
                {
                    if (handle == me)
                    {
                        isChildOfProgman = true;
                        return false;
                    }
                    return true;
                }), IntPtr.Zero);

                if (isChildOfProgman == false)
                {
                    lock (m_lockFlag)
                    {
                        m_needUpdate = true;
                    }
                }

                m_waitHandle.WaitOne(2000);
            }
        }
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            lock (m_lockFlag)
            {
                m_needUpdate = true;
            }
        }
        #endregion
        #region EventHandler
        private void YoutubeForm_Load(object sender, EventArgs e)
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            ScreenUtility.Initialize();
            webBrowser1.Navigate(m_videopath);
            if (PinToBackground())
            {
                m_waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                m_onRunning = true;
                m_checkParent = Task.Factory.StartNew(CheckParent, mainWindowSrc.Handle);
                timer.Start();
                webBrowser1.Navigate(m_videopath);
            }
            else
            {
                Close();
            }

        }
        private void timer_check_Tick(object sender, EventArgs e)
        {
            bool needUpdate = false;
            lock (m_lockFlag)
            {
                needUpdate = m_needUpdate;
                m_needUpdate = false;
            }

            if (needUpdate)
            {
                PinToBackground();
            }
        }

        private void YoutubeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;


            timer.Stop();


            if (m_checkParent != null)
            {
                m_onRunning = false;
                m_waitHandle.Set();
                m_checkParent.Wait(TimeSpan.FromSeconds(10.0));
                m_checkParent = null;

                m_waitHandle.Dispose();
            }
        }
        #endregion
    }
}
