using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Cyrup_Bootstrapper
{
    public class Program
    {
        private static WebClient wc { get; set; }
        private static string version { get; set; }

        private static void Main(string[] args)
        {
            wc = new WebClient();
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\bin")) Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\bin");
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\latestversion")) File.Create($"{AppDomain.CurrentDomain.BaseDirectory}\\latestversion").Close();

            Console.WriteLine("Checking version...");
            try
            { version = wc.DownloadString("https://pastebin.com/raw/ZFNbFVnN").Trim(); }
            catch
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("error: failed to grab latest version number");
                Console.ResetColor();
                Console.WriteLine("your firewall may be blocking the connection");
                Console.WriteLine("join the discord server for more help: https://discord.gg/QqSS9286vV");
                Console.ReadKey(true);
                Environment.Exit(1);
            }

            if (version != File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\latestversion").Trim())
            {
                File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\latestversion", string.Empty);
                File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\latestversion", version);

                Console.WriteLine("Downloading files...");
                wc.DownloadFile($"https://github.com/deaddlocust/Cyrup-Rewrite/releases/download/{version}/Release.zip", $"{AppDomain.CurrentDomain.BaseDirectory}\\Release.zip");
                Console.WriteLine("Extracting files...");
                
                if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\Release"))
                {
                    foreach (string file in Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\Release")) File.Delete(file);
                    foreach (string folder in Directory.GetDirectories($"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\Release"))
                    {
                        foreach (string file in Directory.GetFiles(folder)) File.Delete(file);
                        Directory.Delete(folder);
                    }
                    Directory.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\Release");
                }

                ZipFile.ExtractToDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\Release.zip", $"{AppDomain.CurrentDomain.BaseDirectory}\\bin");
                Console.WriteLine("Cleaning up...");
                File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\Release.zip");
            }

            Console.WriteLine("Starting...");
            Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\Release\\Cyrup_Rewrite.exe");
        }
    }
}
