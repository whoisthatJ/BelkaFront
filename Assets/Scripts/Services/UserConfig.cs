using UnityEngine;

[System.Serializable]
public class UserConfig
{
    public bool isSound;
    public bool isMusic;
    public bool isVibro;
    public string appVersion;

    // Variable to define will next game be single player
    // or multiplayer game. Variable value changes
    // when Game scene loads.
    public bool isOnline;

    public static UserConfig GetDefault()
    {
        UserConfig defaultConfig = new UserConfig();
        defaultConfig.isSound = true;
        defaultConfig.isMusic = true;
        defaultConfig.isVibro = true;
        defaultConfig.appVersion = Application.version;

        defaultConfig.isOnline = false;

        return defaultConfig;
    }
}
