using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public class SettingModelTest
    {
        // Setting: Play mode
        // Is play line in manual mode
        public bool isManualModeOn = true;
        // Is play line in auto mode
        public bool isAutoReadingModeOn = false;
        // Is play line in skip mode
        public bool isSkipModeOn = false;

        // Setting: Message Speed
        // Duration of text showing speed, in seconds
        public const float MAX_TEXT_SHOW_DURATION = 0.06f;
        public float textShowDuration = 0.03f;
        // Duration of line auto switch speed, in seconds
        public const float MAX_LINE_SWITCH_DURATION = 6.0f;
        public float lineSwitchDuration = 3.0f;
        // Duration of line switch speed under skip mode, in seconds
        public const float MAX_SKIP_MODE_LINE_SWITCH_DURATION = 0.2f;
        public float skipModeLineSwitchDuration = 0.1f;

        // Setting: Volume
        // Volume of BGM
        public float bgmVolume = 1.0f;
        // Is BGM mute
        public bool isBgmMute = false;
        // Volume of character Voices 
        public float voicesVolume = 1.0f;
        // Is Voices mute
        public bool isVoicesMute = false;

        // Setting: Screen Mode
        // Is in fullscreen mode
        public bool isFullScreenModeOn = false;
        // The resolution in Windowed mode
        public string resolution = "0x0";

        // Setting: Visual
        // Show CG picture during ship mode or not
        public bool showCGInSkipMode = true;
        // Show special effects or not
        public bool showSpecialEffects = true;
        // Show text shadow or not
        public bool showTextShadow = true;
        // Show animation or not
        public bool showAnimation = true;

        // Setting: Other
        // Is this application remain active in background
        public bool appActiveInBackground = true;
        // Language
        public List<string> languages = new List<string> { "简体中文", "繁体中文", "日本语", "English" };
        public string appLanguage = "Chinese";

        // Setting: Character voices
        // List of playing character voices or not
        public Dictionary<string, bool> characterVoicesSwitcher = new Dictionary<string, bool>()
        {
            { "Ryougi", true }
        };
    }
}
