public class SteamProfileResponse
{
    public SteamProfileResponseData Response { get; set; }
}

public class SteamProfileResponseData
{
    public List<SteamPlayer> Players { get; set; }
}

public class SteamPlayer
{
    public string SteamId { get; set; }
    public string PersonaName { get; set; }
    public string Avatar { get; set; }
    public string RealName { get; set; }
    public string LocCountryCode { get; set; }
}

public class SteamGamesResponse
{
    public SteamGamesResponseData Response { get; set; }
}

public class SteamGamesResponseData
{
    public int GameCount { get; set; }
    public List<SteamGame> Games { get; set; }
}

public class SteamGame
{
    public int AppId { get; set; }
    public string Name { get; set; }
    public int PlaytimeForever { get; set; }
}
