using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DiscordRPC;
using System.Diagnostics;
using CyrupAPI;
using Cyrup.Properties;
using System.Windows.Forms;
using System.Threading;
using Terminal.Gui;

namespace Cyrup
{
    class Interface
    {
        [STAThread]
        public static void Draw()
        {
            DiscordRpcClient client = new DiscordRpcClient("797124617268625408");
            client.SetPresence(new RichPresence()
            {
                Details = "The one-of-a-kind LuaU Executor!",
                State = "Using Cyrup v4",
                Assets = new Assets()
                {
                    LargeImageKey = "untitled_7_",
                }
            });

            if (Settings.Default.RPC == true)
            {
                client.Initialize();
            }

            Console.SetWindowSize(80, 20);
            Console.Title = "Cyrup";
            Terminal.Gui.Application.Init();

            var top = Terminal.Gui.Application.Top;
            var win = new FrameView(new Rect(0, 0, top.Frame.Width - 68, top.Frame.Height), "");
            var win2 = new FrameView(new Rect(12, 0, top.Frame.Width - 12, top.Frame.Height), "");
            Colors.Base.Normal = Terminal.Gui.Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
            Colors.Menu.Normal = Terminal.Gui.Application.Driver.MakeAttribute(Color.Cyan, Color.Black);
            Colors.Dialog.Normal = Terminal.Gui.Application.Driver.MakeAttribute(Color.Magenta, Color.Black);
            win.ColorScheme = Colors.Base;
            win2.ColorScheme = Colors.Base;
            top.Add(win);
            top.Add(win2);

            var inj = new Terminal.Gui.Button(1, 1, "Inj ");
            var exec = new Terminal.Gui.Button(1, 3, "Exec");
            var clr = new Terminal.Gui.Button(1, 5, "Clr ");
            var open = new Terminal.Gui.Button(1, 7, "Open");
            var save = new Terminal.Gui.Button(1, 9, "Save");
            var opt = new Terminal.Gui.Button(1, 16, "Opt ");

            var editor = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = Colors.Menu
            };

            string credits = @"-- Join the discord! discord.io/cyrup";
            editor.Text = credits.Replace("\r\n", "\n");

            win2.KeyDown += (k) =>
            {
                if (k.KeyEvent.Key == Key.CtrlMask)
                {
                    string paste = System.Windows.Forms.Clipboard.GetText();
                    editor.Text = paste.Replace("\r\n", "\n");
                    k.Handled = true;
                }
            };

            inj.Clicked += () =>
            {
                if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
                {
                    Terminal.Gui.MessageBox.ErrorQuery(50, 5, "Error", "Please open Roblox before injecting!", "Okay");
                    return;
                }
                if (CyrupAPI.Cyrup.isAPIAttached())
                {
                    Terminal.Gui.MessageBox.ErrorQuery(50, 5, "Error", "Cyrup is already injected!", "Okay");
                    return;
                }
                else
                {
                    CyrupAPI.Cyrup.Inject();
                    if (Directory.EnumerateFiles(@Environment.CurrentDirectory + "/autoexec").Count() > 0)
                    {
                        AutoExecCy();
                    }
                    return;
                }
            };

            exec.Clicked += () =>
            {
                if (CyrupAPI.Cyrup.isAPIAttached())
                {
                    CyrupAPI.Cyrup.Execute(Convert.ToString(editor.Text));
                    return;
                }
                else
                {
                    Terminal.Gui.MessageBox.ErrorQuery(50, 5, "Error", "Exploit is not injected!", "Okay");
                    return;
                }
            };

            clr.Clicked += () =>
            {
                editor.Text = String.Empty;
            };

            save.Clicked += () =>
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                {
                    Title = "Save File",

                    CheckPathExists = true,

                    DefaultExt = "txt",
                    Filter = "Text files (*.txt)|*.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(saveFileDialog1.FileName);
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine(editor.Text.ToString());
                    }
                }
                return;
            };

            open.Clicked += () =>
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory + "\\scripts",
                    Title = "Browse Text Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "txt",
                    Filter = "Text files (*.txt)|*.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string text = File.ReadAllText(openFileDialog1.FileName);
                    editor.Text = text.Replace("\r\n", "\n");
                }
            };

            opt.Clicked += () =>
            {
                var optMenu = Terminal.Gui.MessageBox.Query(50, 5, "Options", "Options Menu", "Discord RPC", "Kill Roblox", "Discord Invite");

                switch (optMenu)
                {
                    case 0:
                        var _0 = Terminal.Gui.MessageBox.Query(50, 5, "Options Menu", "Discord RPC Settings", "Enable RPC", "Disable RPC");
                        switch (_0)
                        {
                            case 0:
                                Settings.Default.RPC = true;
                                Settings.Default.Save();
                                if (client.IsInitialized == false)
                                {
                                    client.Initialize();
                                    client.SetPresence(new RichPresence()
                                    {
                                        Details = "The one-of-a-kind Roblox Lua Executor!",
                                        State = "Using Cyrup v4",
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "untitled_7_",
                                        }
                                    });
                                }
                                Terminal.Gui.MessageBox.Query(50, 5, "Notice", "RPC has been enabled", "Okay");
                                break;
                            case 1:
                                Settings.Default.RPC = false;
                                Settings.Default.Save();
                                client.Deinitialize();
                                Terminal.Gui.MessageBox.Query(50, 5, "Notice", "RPC has been disabled", "Okay");
                                break;
                        }
                        break;
                    case 1:
                        var areyousure = Terminal.Gui.MessageBox.Query(50, 5, "Stop", "Are you sure?", "Yes kill Roblox", "No");
                        if (areyousure == 0)
                        {
                            foreach (var process in Process.GetProcessesByName("RobloxPlayerBeta"))
                            {
                                process.Kill();
                            }
                        }

                        if (areyousure == 1) return;
                        break;
                    case 2:
                        Process.Start("https://discord.gg/ejrRUM9cN9");
                        break;

                }
            };


            win.Add(
               inj,
               exec,
               clr,
               open,
               save,
               opt
               );

            win2.Add(
                editor
                );

            void AutoExecCy()
            {
                while (!CyrupAPI.Cyrup.isAPIAttached()) { Thread.Sleep(500); }
                foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory + "/autoexec", "*.*"))
                {
                    CyrupAPI.Cyrup.Execute(File.ReadAllText(file));
                }
            }

            Terminal.Gui.Application.Run();
        }
    }
}
