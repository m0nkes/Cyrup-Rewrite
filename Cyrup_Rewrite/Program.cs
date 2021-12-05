using System;
using System.IO;
using System.Threading;
using static Cyrup_Rewrite.Native;

namespace Cyrup_Rewrite
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
            if (!Directory.Exists(Environment.CurrentDirectory + "\\autoexec")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\autoexec");
            if (!Directory.Exists(Environment.CurrentDirectory + "\\scripts")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\scripts");

            IntPtr consolehwnd = GetConsoleWindow();
            if (consolehwnd == IntPtr.Zero)
            {
                Console.WriteLine("fatal error: failed to grab console window handle");
                Thread.Sleep(3000);
                Environment.Exit(1);
            }

            IntPtr menuhandle = GetSystemMenu(consolehwnd, false);
            DeleteMenu(menuhandle, SC_MAXIMIZE, 0); // Disable maximize
            DeleteMenu(menuhandle, SC_SIZE, 0); // Disable resize

            Interface gui = new Interface("Cyrup");
            gui.Start();
        }
    }
}
