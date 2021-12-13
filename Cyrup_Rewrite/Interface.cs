using System;
using System.Collections.Generic;
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
        private FrameView win3 { get; set; }
        private TextView editor { get; set; }
        private ListView scriptlist { get; set; }
        private List<string> scriptlist_paths { get; set; }

        private ExploitAPI api { get; set; }

        private bool enable_scriptlist = true;
        private bool enable_autoexec = true;

        public Interface(string title)
        {
            Console.Title = title;
            Application.Init();

            top = Application.Top;
            win = new FrameView(new Rect(0, 0, top.Frame.Width - 68, top.Frame.Height), title);
            win2 = new FrameView(new Rect(12, 0, top.Frame.Width - 28, top.Frame.Height));
            win3 = new FrameView(new Rect(top.Frame.Width - 16, 0, top.Frame.Width - 64, top.Frame.Height), string.Empty);
            api = new ExploitAPI();

            Colors.Base.Normal = new Terminal.Gui.Attribute(Color.BrightMagenta, Color.Black);
            win.ColorScheme = win2.ColorScheme = win3.ColorScheme = Colors.Base;

            Button inj = new Button(1, 1, "Inj ");
            Button exec = new Button(1, 3, "Exec");
            Button clr = new Button(1, 5, "Clr ");
            Button open = new Button(1, 7, "Open");
            Button save = new Button(1, 9, "Save");
            Button opt = new Button(1, 16, "Opt ");

            scriptlist = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = new ColorScheme()
                {
                    Normal = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black),
                    Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
                    HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black)
                }
            };

            editor = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = new ColorScheme()
                {
                    Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
                    Focus = new Terminal.Gui.Attribute(Color.White, Color.Black)
                },
                Text = "-- Join the discord: discord.io/cyrupofficial"
            };

            editor.KeyDown += (k) =>
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

            scriptlist.OpenSelectedItem += (ListViewItemEventArgs e) => { editor.Text = File.ReadAllText(scriptlist_paths[e.Item]).Trim(); };

            win.Add(inj, exec, clr, open, save, opt);
            win2.Add(editor);
            win3.Add(scriptlist);
            top.Add(win, win2, win3);
        }

        public void Start()
        {
            scriptlist_paths = new List<string>(Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\scripts"));
            List<string> names = new List<string>();
            foreach (string file in scriptlist_paths) names.Add(Path.GetFileNameWithoutExtension(file));
            scriptlist.SetSource(names);
            Application.Run();
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
                if (enable_autoexec)
                {
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

            if (opd.FilePaths.Count > 0) editor.Text = File.ReadAllText(opd.FilePaths[0]).Trim();
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
            Window main = new Window(new Rect(0, 0, top.Frame.Width + 10, top.Frame.Height + 10), "", 0, new Border { BorderStyle = BorderStyle.None });
            FrameView view = new FrameView(new Rect(0, 0, 80, 20), "Options");

            Button killrbx = new Button(1, 1, "Kill Roblox");
            Button discord = new Button(1, 3, "Discord Invite");
            CheckBox sl_visible = new CheckBox(1, 6, "Show script list", enable_scriptlist);
            CheckBox autoexec = new CheckBox(1, 8, "Auto execute", enable_autoexec);

            Button exitopt = new Button(view.Frame.Width - 6, 0, "X");

            killrbx.Clicked += () =>
            {
                if (MessageBox.Query(50, 5, "Alert", "Are you sure?", "Yes", "No") == 0)
                {
                    foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta")) process.Kill();
                }
            };

            discord.Clicked += () => Process.Start("https://discord.io/cyrupofficial");
            exitopt.Clicked += () => Application.RequestStop(main);

            view.Add(killrbx, discord, sl_visible, autoexec);
            main.Add(view);
            main.Add(exitopt);

            Application.Run(main);
            enable_scriptlist = sl_visible.Checked;
            enable_autoexec = autoexec.Checked;

            if (enable_scriptlist)
            {
                win2.Width = top.Frame.Width - 28;
                win3.Visible = true;
            }
            else
            {
                win2.Width = top.Frame.Width - 12;
                win3.Visible = false;
            }
        }
    }
}
