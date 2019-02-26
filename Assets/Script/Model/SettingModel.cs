using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public class SettingModel
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
        public const float MAX_TEXT_SHOW_DURATION = 0.06f;
        public static float textShowDuration = 0.03f;
        // Duration of line auto switch speed, in seconds
        public const float MAX_LINE_SWITCH_DURATION = 6.0f;
        public static float lineSwitchDuration = 3.0f;
        // Duration of line switch speed under skip mode, in seconds
        public const float MAX_SKIP_MODE_LINE_SWITCH_DURATION = 0.2f;
        public static float skipModeLineSwitchDuration = 0.1f;

        // Setting: Volume
        // Volume of BGM
        public static float bgmVolume = 0.5f;
        // Is BGM mute
        public static bool isBgmMute = false;
        // Volume of character Voices 
        public static float voicesVolume = 0.5f;
        // Is Voices mute
        public static bool isVoicesMute = false;
        // Volume of character sound FX 
        public static float soundVolume = 0.5f;
        // Is sound FX mute
        public static bool isSoundMute = false;

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
        public static List<string> languages = new List<string> { "简体中文", "繁体中文", "日本语", "English" };
        public static string appLanguage = "Chinese";

        // Setting: Character voices
        // List of playing character voices or not
        public static Dictionary<string, bool> characterVoicesSwitcher = new Dictionary<string, bool>()
        {
            { "Ryougi", true }
        };

        /// <summary>
        /// Update static <see cref="SettingModel"/> using existing <see cref="SerializableSettingModel"/>
        /// </summary>
        /// <param name="model">The existing <see cref="SerializableSettingModel"/></param>
        public static void Update(SerializableSettingModel model)
        {
            if (null == model) return;

            isManualModeOn = model.isManualModeOn;
            isAutoReadingModeOn = model.isAutoReadingModeOn;
            isSkipModeOn = model.isSkipModeOn;
            textShowDuration = model.textShowDuration;
            lineSwitchDuration = model.lineSwitchDuration;
            skipModeLineSwitchDuration = model.skipModeLineSwitchDuration;
            bgmVolume = model.bgmVolume;
            isBgmMute = model.isBgmMute;
            voicesVolume = model.voicesVolume;
            isVoicesMute = model.isVoicesMute;
            soundVolume = model.soundVolume;
            isSoundMute = model.isSoundMute;
            isFullScreenModeOn = model.isFullScreenModeOn;
            resolution = model.resolution;
            showCGInSkipMode = model.showCGInSkipMode;
            showSpecialEffects = model.showSpecialEffects;
            showTextShadow = model.showTextShadow;
            showAnimation = model.showAnimation;
            appActiveInBackground = model.appActiveInBackground;
            appLanguage = model.appLanguage;
            characterVoicesSwitcher = model.characterVoicesSwitcher;
        }
    }
}
