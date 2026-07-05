using System.Text.Json;

namespace OSC_Stream.Services.Infra;

public class ConfigService
{
    private const string ConfigPath = "config.json";
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    public AppConfig Config { get; private set; }

    public ConfigService()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            Config = CreateDefaultConfig();
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Config, Indented));
        }
        else
        {
            var json = File.ReadAllText(ConfigPath);
            Config = JsonSerializer.Deserialize<AppConfig>(json) ?? CreateDefaultConfig();
        }
    }

    public void SaveConfig(AppConfig updated)
    {
        Config = updated;
        File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Config, Indented));
    }

    private static AppConfig CreateDefaultConfig()
    {
        return new AppConfig
        {
            AdminAccountIds = new List<string>(),
            EnableAccountCreation = true,
        };
    }

    public class AppConfig
    {
        public List<string> AdminAccountIds { get; set; } = new();
        public bool EnableAccountCreation { get; set; } = true;
    }
}
