using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;
using WeAreDevs_API;

namespace Cyrup_Rewrite
{
    public class Interface
    {
        private Toplevel top { get; set; }
        private FrameView win { get; set; }
        private FrameView win2 { get; set; }
        private TextView editor { get; set; }

        private ExploitAPI api { get; set; }

        public void Start() => Application.Run();

        public Interface(string title)
        {
            Console.Title = title;
            Application.Init();

            top = Application.Top;
            win = new FrameView(new Rect(0, 0, top.Frame.Width - 68, top.Frame.Height), title);
            win2 = new FrameView(new Rect(12, 0, top.Frame.Width - 12, top.Frame.Height), string.Empty);
            api = new ExploitAPI();

            Colors.Base.Normal = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
            Colors.Menu.Normal = Application.Driver.MakeAttribute(Color.Cyan, Color.Black);
            Colors.Dialog.Normal = Application.Driver.MakeAttribute(Color.Magenta, Color.Black);

            win.ColorScheme = Colors.Base;
            win2.ColorScheme = Colors.Base;

            top.Add(win, win2);

            Button inj = new Button(1, 1, "Inj ");
            Button exec = new Button(1, 3, "Exec");
            Button clr = new Button(1, 5, "Clr ");
            Button open = new Button(1, 7, "Open");
            Button save = new Button(1, 9, "Save");
            Button opt = new Button(1, 16, "Opt ");

            editor = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = Colors.Menu,
                Text = "-- join the discord etc etc idk"
            };

            win2.KeyDown += (k) =>
            {
                if (k.KeyEvent.Key == (Key.CtrlMask | Key.V))
                {
                    string data = string.Empty;
                    if (Clipboard.TryGetClipboardData(out data)) editor.Text += data;
                }
                else if (k.KeyEvent.Key == (Key.CtrlMask | Key.C)) Clipboard.TrySetClipboardData(editor.SelectedText.ToString());
                else if (k.KeyEvent.Key == (Key.CtrlMask | Key.A)) editor.SelectAll();

                k.Handled = true;
            };

            inj.Clicked += OnInject;
            exec.Clicked += OnExecute;
            clr.Clicked += () => { editor.Text = string.Empty; };
            open.Clicked += OnOpen;
            save.Clicked += OnSave;
            opt.Clicked += OnOptMenu;

            win.Add(inj, exec, clr, open, save, opt);
            win2.Add(editor);
        }

        private void OnInject()
        {
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
                MessageBox.ErrorQuery(50, 5, "Error", "Please open Roblox before injecting!", "Okay");
            else if (api.isAPIAttached())
                MessageBox.ErrorQuery(50, 5, "Error", "Cyrup is already injected!", "Okay");
            else
            {
                Task.Factory.StartNew(api.LaunchExploit);
                Task.Factory.StartNew(() => // auto execute
                {
                    while (!api.isAPIAttached()) Thread.Sleep(10);
                    foreach (string path in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\autoexec"))
                    {
                        api.SendLuaScript(File.ReadAllText(path));
                    }
                });
            }
        }

        private void OnExecute()
        {
            if (api.isAPIAttached())
                Task.Factory.StartNew(() => { api.SendLuaScript(editor.Text.ToString()); });
            else
                MessageBox.ErrorQuery(50, 5, "Error", "Exploit is not injected!", "Okay");
        }

        private void OnOpen()
        {
            OpenDialog opd = new OpenDialog("Open file", "");
            opd.Width = Console.WindowWidth;
            opd.Height = Console.WindowHeight;
            opd.AllowsMultipleSelection = false;
            opd.AllowedFileTypes = new string[] { "txt", "lua" };
            opd.CanChooseFiles = true;
            opd.CanChooseDirectories = false;
            opd.DirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\scripts";
            opd.ColorScheme = Colors.Base;

            Application.Run(opd);

            if (opd.FilePaths.Count > 0) editor.Text = File.ReadAllText(opd.FilePaths[0]);
            opd.Dispose();
        }

        private void OnSave()
        {
            SaveDialog opd = new SaveDialog("Save file", "");
            opd.Width = Console.WindowWidth;
            opd.Height = Console.WindowHeight;
            opd.DirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\scripts";
            opd.ColorScheme = Colors.Base;

            Application.Run(opd);

            if (opd.FileName != null)
            {
                StreamWriter savewriter = File.CreateText(opd.FileName.ToString());
                savewriter.Write(editor.Text.ToString());
                savewriter.Dispose();
            }
            opd.Dispose();
        }

        private void OnOptMenu()
        {
            switch (MessageBox.Query(50, 5, "Options", "Options Menu", "Kill Roblox", "Discord Invite"))
            {
                case 0:
                    {
                        if (MessageBox.Query(50, 5, "Alert", "Are you sure?", "Yes", "No") == 0)
                        {
                            foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta")) process.Kill();
                        }
                        break;
                    }
                case 1:
                    {
                        Process.Start("https://discord.gg/QqSS9286vV");
                        break;
                    }
            }
        }
    }
}
