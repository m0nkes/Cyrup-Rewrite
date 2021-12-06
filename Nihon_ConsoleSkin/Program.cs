using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

namespace Cyrup
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        private const int SWP_NOSIZE = 0x0001;

        public static string appver = "4.5.7";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Getting console window...");
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            IntPtr sysMenu = GetSystemMenu(handle, false);

            Console.WriteLine("Disabling maximize and resizing...");
            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            int xpos = 300;
            int ypos = 150;
            Console.WriteLine("Setting window position and size...");
            Console.SetWindowSize(80, 20);
            SetWindowPos(GetConsoleWindow(), 0, xpos, ypos, 0, 0, SWP_NOSIZE);
            if (Directory.Exists(Environment.CurrentDirectory + "\\autoexec") == false)
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\autoexec");
            }
            Console.WriteLine("Checking version...");
            try
            {
                WebClient wc = new WebClient();
                var version = Convert.ToString(wc.DownloadString("https://pastebin.com/raw/rPkz0Nc8"));
                wc.Dispose();
                if (version != appver)
                {
                    System.Windows.Forms.MessageBox.Show("This version of Cyrup is outdated. Please run the bootstrapper again.", "Notice");
                    Environment.Exit(0);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Operation failed. Could not check vesion.");
            }
            Console.WriteLine("Downloading DLL...");
            CyrupAPI.Cyrup.Init();
            Console.WriteLine("Drawing GUI...");
            Console.Clear();
            ShowWindow(handle, SW_SHOW);
            Interface.Draw();
        }
    }
}