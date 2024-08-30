using System.Text.Json;

namespace VR_Killer;

public class AppConfig
{
    public bool AutoPath { get; set; }
    public string OpenvrPath { get; set; } = string.Empty;
    public bool StartWithWindows { get; set; }

    public static void Save(AppConfig config, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(config, options);
        System.IO.File.WriteAllText(filePath, json);
    }

    public static AppConfig Load(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            return new AppConfig();

        string json = System.IO.File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }
}