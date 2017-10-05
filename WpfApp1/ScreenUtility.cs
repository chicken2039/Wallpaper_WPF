using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1
{
    class ScreenUtility
    {
        static ScreenUtility()
        {
            Initialize();
        }

        //#################################################################################################################

        private static Winapi.RECT s_combinedRect;

        public static Winapi.MONITORINFO[] Screens
        { get; private set; }

        //#################################################################################################################

        public static void Initialize()
        {
            s_combinedRect = new Winapi.RECT(0, 0, 0, 0);

            List<Winapi.MONITORINFO> screens = new List<Winapi.MONITORINFO>();

            Winapi.MonitorEnumDelegate callback = (IntPtr hDesktop, IntPtr hdc, ref Winapi.RECT pRect, int d) =>
            {
                var info = new Winapi.MONITORINFO();
                info.cbSize = sizeof(int) * 4 * 2 + sizeof(int) * 2;
                if (Winapi.GetMonitorInfo(hDesktop, ref info) == false)
                    return false;

                var rect = info.rcMonitor;
                if (rect.Left < s_combinedRect.Left)
                    s_combinedRect.Left = rect.Left;
                if (rect.Top < s_combinedRect.Top)
                    s_combinedRect.Top = rect.Top;
                if (rect.Right > s_combinedRect.Right)
                    rect.Right = s_combinedRect.Right;
                if (rect.Bottom > s_combinedRect.Bottom)
                    rect.Bottom = s_combinedRect.Bottom;

                screens.Add(info);

                return true;
            };

            if (Winapi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
            {
                Screens = screens.ToArray();
            }
            else
            {
                Screens = new[]
                {
                    new Winapi.MONITORINFO()
                    {
                        rcMonitor = Screen.PrimaryScreen.Bounds,
                        rcWork = Screen.PrimaryScreen.WorkingArea,
                    }
                };
            }
        }

        //#################################################################################################################

        public static void FillScreen(Form form, Winapi.MONITORINFO screen)
        {
            var rect = screen.rcMonitor;

            int x = rect.Left - s_combinedRect.Left;
            int y = rect.Top - s_combinedRect.Top;

            Winapi.MoveWindow(form.Handle, x, y, rect.Width, rect.Height, false);
        }

        public static bool IsOverlayed(Form form)
        {
            /// 전체화면이더라도 무시할 클래스명 목록
            string[] classNamesExcluded =
            {
                "WorkerW",                              // 배경화면
                "ProgMan",
                "ImmersiveLauncher",                    // Win8 시작화면
            };


            // 최상위 컨트롤의 핸들을 얻는다.
            IntPtr foregroundWindow = Winapi.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
                return false;
            foregroundWindow = Winapi.GetAncestor(foregroundWindow, Winapi.GetAncestorFlags.GetRoot);


            // 자기 자신이면 가려진게 아님.
            if (foregroundWindow == form.Handle)
                return false;


            // 컨트롤의 클래스이름을 얻어옵니다.
            string className = Winapi.GetClassName(foregroundWindow);
            if (className.Length <= 0)
                return false;


            // 제외목록에서 하나라도 해당되는지 확인.
            if (classNamesExcluded.Any((s) => s.ToUpper() == className.ToUpper()))
                return false;


            // 현재 컨트롤이 속하는 모니터의 사각영역을 얻어옵니다.
            Winapi.RECT desktop;
            IntPtr monitor = Winapi.MonitorFromWindow(form.Handle, Winapi.MONITOR_DEFAULTTONEAREST);
            if (monitor == IntPtr.Zero)
            {
                // 모니터를 찾을 수 없으면 현재 윈도우 화면의 핸들로 설정한다.
                IntPtr desktopWnd = Winapi.GetDesktopWindow();
                if (desktopWnd == IntPtr.Zero)
                    return false;

                if (Winapi.GetWindowRect(desktopWnd, out desktop) == false)
                    return false;
            }
            else
            {
                var info = new Winapi.MONITORINFO();
                info.cbSize = sizeof(int) * 4 * 2 + sizeof(int) * 2;
                if (Winapi.GetMonitorInfo(monitor, ref info) == false)
                    return false;

                desktop = info.rcMonitor;
            }


            // 컨트롤의 작업영역을 알아낸다.
            Winapi.RECT client;
            if (Winapi.GetWindowRect(foregroundWindow, out client) == false)
                return false;


            // 컨트롤이 모니터에 완전히 들어오지 않았거나 크기보다 작으면 전체화면이 아니다.
            if (client.Left > desktop.Left + 1
                ||
                client.Top > desktop.Top + 1
                ||
                client.Right < desktop.Right - 1
                ||
                client.Bottom < desktop.Bottom - 1)
                return false;


            // 여기까지 도달했으면 전체화면이다.
            return true;
        }
    }
}
