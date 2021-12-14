using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Cyrup_Rewrite
{
    public class Settings
    {
        public static SettingsObject Load()
        {
            string content = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\settings");
            return JsonSerializer.Deserialize<SettingsObject>(content);
        }

        public static void Save(SettingsObject s)
        {
            if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\settings")) File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\settings");

            StreamWriter sw = File.CreateText($"{AppDomain.CurrentDomain.BaseDirectory}\\settings");
            sw.WriteLine(JsonSerializer.Serialize(s));
            sw.Close();
            sw.Dispose();
        }
    }

    public class SettingsObject
    {
        public bool enable_topmost { get; set; }
        public bool enable_scriptlist { get; set; }
        public bool enable_autoexec { get; set; }
        public bool enable_autoattach { get; set; }
        public bool enable_opacity{ get; set; }
        public bool enable_multirbx { get; set; }
    }
}
