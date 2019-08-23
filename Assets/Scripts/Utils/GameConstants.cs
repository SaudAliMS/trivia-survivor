using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    public const int kEncyptionStringKey = 910867680;
    public const string metadata_path = "metadata_path";
    public const string metadata_version = "metadata_version";

    public const string languagefile_path = "languagefile_path";
    public const string languagefile_version = "languagefile_version";

        
    public const float QUESTION_TIME = 5;

    public const string MusicState = "MusicState";
    public const string SoundState = "SoundState";

    public const string XPCount         = "XPCount";
    public const string CoinsCount      = "CoinsCount";
    public const string Level           = "Level";
    public const int COIN_REWARD_ON_MATCH_COMPLETE = 10;
    public const int COIN_REWARD_ON_TRUE_ANSWER = 2;
    public const int XP_REWARD_ON_TRUE_ANSWER = 1;
    public const int MY_CHARACTER_ID = 0;
    public static readonly int[] REQUIRED_XP = {3, 5, 7, 10, 20};
    public static readonly string[] POSITIONS = { "WIN!", "2nd", "3rd","4th","5th","6th","7th","8th","9th","10th" };

    public static Color GreyColour = new Color(100f/255f, 100f/255f, 100f/255f);
    public static Color WhiteColour = new Color(1, 1, 1);

}
