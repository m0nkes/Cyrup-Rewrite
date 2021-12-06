using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;


namespace CyrupAPI
{
    public class Cyrup
    {
        private static string exploitdllname = "Cyrup.dll";
        public static string luapipename = "cyrupLBI";
        public static WebClient wc = new System.Net.WebClient();

        // Pipes
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);
        private static bool NamedPipeExist(string pipeName)
        {
            try
            {
                if (!WaitNamedPipe($"\\\\.\\pipe\\{pipeName}", 0))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0)
                    {
                        return false;
                    }
                    if (lastWin32Error == 2)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void LuaPipe(string script)
        {
            if (NamedPipeExist(luapipename))
            {
                new Thread(() =>
                {
                    try
                    {
                        using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", luapipename, PipeDirection.Out))
                        {
                            namedPipeClientStream.Connect();
                            using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream, System.Text.Encoding.Default, 999999))//changed buffer to max 1mb since default buffer is 1kb
                            {
                                streamWriter.Write(script);
                                streamWriter.Dispose();
                            }
                            namedPipeClientStream.Dispose();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error occured connecting to the pipe.", "Connection Failed!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }).Start();
            }
            else
            {
                MessageBox.Show("Inject " + exploitdllname + " before Using this!", "Cyrup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        // Functions
        public static void Inject()
        {
            if (NamedPipeExist(luapipename))//check if the pipe exist
            {
                MessageBox.Show("Exploit is already injected!", "Cyrup", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);//if the pipe exist that's mean that we don't need to inject
                return;
            }
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
            {
                MessageBox.Show("Please open Roblox before injecting!", "Cyrup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (NamedPipeExist(luapipename) == false)//check if the pipe don't exist
            {
                if (Process.GetProcessesByName("PuppyMilkV3").Length > 0)
                {
                    foreach (var process in Process.GetProcessesByName("PuppyMilkV3")) { process.Kill(); }
                }
                string DLLPath = "\"" + Environment.CurrentDirectory + @"\Cyrup.dll" + "\"";
                Process.Start(new ProcessStartInfo("bin\\PuppyMilkV3.exe")
                {
                    Arguments = DLLPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
        }

        public static void Execute(string script)
        {
            if (NamedPipeExist(luapipename))
            {
                if (script.Contains("game:HttpGet"))
                {
                    script = script.Replace("game:HttpGet", "HttpGet");
                }
                if (script.Contains("game:HttpGetAsync"))
                {
                    script = script.Replace("game:HttpGetAsync", "HttpGet");
                }
                if (script.Contains("HttpGetAsync"))
                {
                    script = script.Replace("HttpGetAsync", "HttpGet");
                }
                if (script.Contains("game:GetObjects"))
                {
                    script = script.Replace("game:GetObjects", "GetObjects");
                }
                LuaPipe(script);
            }
            else
            {
                MessageBox.Show($"Please inject the exploit first!", "Cyrup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        public static void Init()
        {
            if (File.Exists("Cyrup.dll"))
            {
                File.Delete("Cyrup.dll");
            }
            wc.DownloadFile(new Uri("https://github.com/deaddlocust/Cyrup-Executor/raw/main/Cyrup.dll"), Environment.CurrentDirectory + "\\Cyrup.dll");
        }

        public static bool isAPIAttached()
        {
            if (NamedPipeExist(luapipename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
