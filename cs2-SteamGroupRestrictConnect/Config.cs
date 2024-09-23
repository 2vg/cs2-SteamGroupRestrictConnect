using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace cs2_SteamGroupRestrictConnect;

public class Config : IBasePluginConfig
{

    [JsonPropertyName("GeneralSettings")]
    public GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

    [JsonPropertyName("Messages")]
    public Messages Messages { get; set; } = new Messages();

    [JsonPropertyName("ConfigVersion")]
    public int Version { get; set; } = 1;

}

public class GeneralSettings
{

    [JsonPropertyName("SteamGroupURL")]
    public string SteamGroupURL { get; set; } = "";

    [JsonPropertyName("SteamApiKey")]
    public string SteamApiKey { get; set; } = "";
}

public class Messages
{

    [JsonPropertyName("KickMessage")]
    public string KickMessage { get; set; } = "You must have joined our steam group, before connect to the server.";
}

