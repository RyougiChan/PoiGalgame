using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public static class SettingModel
    {
        // Setting: Play mode
        // Is play line in manual mode
        public static bool isManualModeOn = true;
        // Is play line in auto mode
        public static bool isAutoReadingModeOn = false;
        // Is play line in skip mode
        public static bool isSkipModeOn = false;

        // Setting: Message Speed
        // Duration of text showing speed, in seconds
        public static float textShowDuration = 0.03f;
        // Duration of line auto switch speed, in seconds
        public static float lineSwitchDuration = 3.0f;
        // Duration of line switch speed under skip mode, in seconds
        public static float skipModeLineSwitchDuration = 0.1f;

        // Setting: Volume
        // Volume of BGM
        public static float bgmVolume = 1.0f;
        // Is BGM mute
        public static bool isBgmMute = false;
        // Volume of character Voices 
        public static float voicesVolume = 1.0f;
        // Is Voices mute
        public static bool isVoicesMute = false;

        // Setting: Screen Mode
        // Is in fullscreen mode
        public static bool isFullScreenModeOn = false;
        // The resolution in Windowed mode
        public static string resolution = "0x0";

        // Setting: Visual
        // Show CG picture during ship mode or not
        public static bool showCGInSkipMode = true;
        // Show special effects or not
        public static bool showSpecialEffects = true;
        // Show text shadow or not
        public static bool showTextShadow = true;
        // Show animation or not
        public static bool showAnimation = true;

        // Setting: Other
        // Is this application remain active in background
        public static bool appActiveInBackground = true;
        // Language
        public static string language = "Chinese";

        // Setting: Character voices
        // List of playing character voices or not
        public static Dictionary<string, bool> characterVoicesSwitcher = new Dictionary<string, bool>()
        {
            { "Ryougi", true }
        };
    }
}
