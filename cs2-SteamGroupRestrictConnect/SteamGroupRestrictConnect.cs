using System.Xml;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace cs2_SteamGroupRestrictConnect;

[MinimumApiVersion(266)]
public class SteamGroupRestrictConnect : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "SteamGroupRestrictConnect";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "uru";
    private int ModuleConfigVersion => 1;
    public required Config Config { get; set; }

    private static Timer _timer;
    private readonly List<string> _playersList = [];

    public override void Load(bool hotReload)
    {
        base.Load(hotReload);
        RefreshSteamGroupUsers();

        _timer = new Timer(RefreshSteamGroupUsers, null, 1000 * 60, 1000 * 60);

        RegisterEventHandler<EventPlayerConnectFull>(OnClientConnect);
    }

    public void OnConfigParsed(Config config)
    {
        if (config.Version < ModuleConfigVersion)
        {
            Console.WriteLine($"[{ModuleName}] You are using an old configuration file. Version you are using:{config.Version} - New Version: {ModuleConfigVersion}");
        }

        Config = config;
    }

    private HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null || @event.Userid == null) return HookResult.Continue;

        CCSPlayerController player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot ||
            !IsValidConfigString(Config.GeneralSettings.SteamApiKey) ||
            !IsValidConfigString(Config.GeneralSettings.SteamGroupURL))
            return HookResult.Continue;

        if (!CheckPlayerGroups(player.SteamID.ToString()))
            Server.ExecuteCommand($"kickid {player.UserId} \"{Config.Messages.KickMessage}\"");

        return HookResult.Continue;
    }

    private bool CheckPlayerGroups(string steamId)
    {
        if (!IsValidConfigString(Config.GeneralSettings.SteamGroupURL) || !IsValidConfigString(Config.GeneralSettings.SteamGroupURL))
            return false;

        if (string.IsNullOrEmpty(steamId))
        {
            return false;
        }

        return _playersList.Contains(steamId);
    }

    private void RefreshSteamGroupUsers(object state = null)
    {
        try {
            string apiUrl = $"https://steamcommunity.com/groups/{Config.GeneralSettings.SteamGroupURL}/memberslistxml/?xml=1";
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(3);
            HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            string content = response.Content.ReadAsStringAsync().Result;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            XmlNodeList steamIDNodes = doc.SelectNodes("//steamID64");

            _playersList.Clear();

            foreach (XmlNode node in steamIDNodes)
            {
                _playersList.Add(node.InnerText);
            }
        } catch (Exception e)
        {
            Console.WriteLine($"[{ModuleName}]: error occured => {e}");
        }
    }

    private static bool IsValidConfigString(string value) => !string.IsNullOrEmpty(value);
}
