using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WpfApp1
{
    class BehindDesktopIcon
    {
        public static bool FixBehindDesktopIcon(IntPtr formHandle)
        {
            IntPtr progman = Winapi.FindWindow("Progman", null);

            if (progman == IntPtr.Zero)
                return false;


            IntPtr workerw = IntPtr.Zero;

            // 여러번 시도함.
            for (int step = 0; step < 8; ++step)
            {
                // 한번씩은 건너뜀.
                if (step % 2 == 0)
                {
                    IntPtr result = IntPtr.Zero;
                    Winapi.SendMessageTimeout(progman,
                        0x052C,
                        new IntPtr(0),
                        IntPtr.Zero,
                        Winapi.SendMessageTimeoutFlags.SMTO_NORMAL,
                        10000,
                        out result);
                }


                Winapi.EnumWindows(new Winapi.EnumWindowsProc((tophandle, topparamhandle) =>
                {
                    IntPtr p = Winapi.FindWindowEx(tophandle,
                        IntPtr.Zero,
                        "SHELLDLL_DefView",
                        IntPtr.Zero);

                    if (p != IntPtr.Zero)
                    {
                        workerw = Winapi.FindWindowEx(IntPtr.Zero,
                            tophandle,
                            "WorkerW",
                            IntPtr.Zero);
                    }

                    return true;
                }), IntPtr.Zero);


                if (workerw == IntPtr.Zero)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }

            if (workerw == IntPtr.Zero)
                return false;


            Winapi.ShowWindow(workerw, 0/*HIDE*/);
            Winapi.SetParent(formHandle, progman);


            return true;
        }
    }
}
