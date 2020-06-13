using Assets.Script.Model;
using Assets.Script.Model.Datas;
using Assets.Script.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Script.Chapter
{
    public class GameController : MonoBehaviour
    {
        public bool inGame;

        private GameObject background;
        private GameObject displayCanvas;
        private GameObject videoPlayer;
        private GameObject bgmAudioSource;
        private GameObject voiceAudioSource;

        private SpriteRenderer bg;
        private GameObject titleContainer;
        private GameObject lineContainer;
        private GameObject historyField;
        private GameObject savedDataField;
        private GameObject menuField;
        private GameObject popupWindow;
        private VideoPlayer _video;
        private AudioSource _bgmAudio;
        private AudioSource _voiceAudio;
        private AudioClip bgmMusic;

        #region Setting field
        private GameObject settingField;
        private Toggle PlayMode_Manual;
        private Toggle PlayMode_Auto;
        private Toggle PlayMode_Skip;
        private Toggle Visual_ShowCGInSkipMode;
        private Toggle Visual_SpecialEffects;
        private Toggle Visual_TextShadow;
        private Toggle Visual_Animation;
        private Toggle ScreenMode_FullScreen;
        private Toggle ScreenMode_Windowed;
        private Toggle Volume_MuteBGM;
        private Toggle Volume_MuteVoices;
        private Toggle Volume_MuteSound;
        private Toggle Other_AppActiveInBackground;
        private Slider Volume_BGM;
        private Slider Volume_Voices;
        private Slider Volume_Sound;
        private Slider MessageSpeed_UnreadText;
        private Slider MessageSpeed_AutoPlayDelayInterval;
        private Slider MessageSpeed_SkipInterval;
        private Dropdown ResolutionDropdown;
        private Dropdown LanguageDropdown;
        #endregion

        private ChapterController chapterController;

        private float startTime;
        private bool isMenuShow;
        private Vector3 defaultMenuLocalPos;
        private Vector3 shownMenuLocalPos;

        private const string DEFAULT_POPUP_NOTICE_TEXT = "本気ですか？";
        private Text popupNoticeText;
        private Text popupCallbackText;

        public static string rootPath;
        public static string savedDataPath;
        public static string savedDataFile;
        public static int savdDataPageCount;
        public static int lastLoadedSavedDataPage;
        public static string settingConfigPath;
        public static string settingConfig;
        public static string settingConfigFullName;

        public static List<SavedDataModel> savedDatas;
        public static GameObject[] historyQuene;

        void Awake()
        {
            rootPath = Application.dataPath;
            savedDataPath = rootPath + "/Resources/SavedData/";
            savedDataFile = "savedata.dat";
            savdDataPageCount = 10;
            lastLoadedSavedDataPage = 0;
            settingConfigPath = rootPath + "/Resources/Config/";
            settingConfig = "app.config";
            settingConfigFullName = string.Format("{0}{1}", settingConfigPath, settingConfig);

            InitializeSettingConfig();
            savedDatas = LoadSavedDatas();
        }

        void Start()
        {
            background = GameObject.Find("Background");
            displayCanvas = GameObject.Find("DisplayCanvas");
            videoPlayer = GameObject.Find("VideoPlayer/VideoPlayer");
            bgmAudioSource = GameObject.Find("AudioSource/BgmAudioSource");
            voiceAudioSource = GameObject.Find("AudioSource/VoiceAudioSource");
            startTime = Time.time;
            bg = background.transform.Find("Bg").gameObject.GetComponent<SpriteRenderer>();
            titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = displayCanvas.transform.Find("LineContainer").gameObject;
            historyField = displayCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = displayCanvas.transform.Find("SavedDataField").gameObject;
            settingField = displayCanvas.transform.Find("SettingField").gameObject;
            menuField = displayCanvas.transform.Find("MenuField").gameObject;
            popupWindow = displayCanvas.transform.Find("PopupWindow").gameObject;
            popupNoticeText = popupWindow.transform.Find("Popup").Find("NoticeText").gameObject.GetComponent<Text>();
            popupCallbackText = popupWindow.transform.Find("Popup").Find("CallbackMethod").gameObject.GetComponent<Text>();
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _bgmAudio = bgmAudioSource.GetComponent<AudioSource>();
            _voiceAudio = voiceAudioSource.GetComponent<AudioSource>();

            // setting field
            PlayMode_Manual = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "PlayMode:Manual:Toggle");
            PlayMode_Auto = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "PlayMode:Auto:Toggle");
            PlayMode_Skip = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "PlayMode:Skip:Toggle");
            Visual_ShowCGInSkipMode = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Visual:ShowCGInSkipMode:Toggle");
            Visual_SpecialEffects = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Visual:ShowCGInSkipMode:Toggle");
            Visual_TextShadow = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Visual:TextShadow:Toggle");
            Visual_Animation = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Visual:Animation:Toggle");
            ScreenMode_FullScreen = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "ScreenMode:FullScreen:Toggle");
            ScreenMode_Windowed = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "ScreenMode:Windowed:Toggle");
            Other_AppActiveInBackground = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Other:AppActiveInBackground:Toggle");
            Volume_MuteBGM = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Volume:BGM:Toggle");
            Volume_MuteVoices = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Volume:Voices:Toggle");
            Volume_MuteSound = GameObjectUtil.FindComponentByPath<Toggle>(settingField, "Volume:Sound:Toggle");
            Volume_BGM = GameObjectUtil.FindComponentByPath<Slider>(settingField, "Volume:BGM:Slider");
            Volume_Voices = GameObjectUtil.FindComponentByPath<Slider>(settingField, "Volume:Voices:Slider");
            Volume_Sound = GameObjectUtil.FindComponentByPath<Slider>(settingField, "Volume:Sound:Slider");
            MessageSpeed_UnreadText = GameObjectUtil.FindComponentByPath<Slider>(settingField, "MessageSpeed:UnreadText:Slider");
            MessageSpeed_AutoPlayDelayInterval = GameObjectUtil.FindComponentByPath<Slider>(settingField, "MessageSpeed:AutoPlayDelayInterval:Slider");
            MessageSpeed_SkipInterval = GameObjectUtil.FindComponentByPath<Slider>(settingField, "MessageSpeed:SkipInterval:Slider");
            ResolutionDropdown = GameObjectUtil.FindComponentByPath<Dropdown>(settingField, "ScreenMode:Windowed:Resolution");
            LanguageDropdown = GameObjectUtil.FindComponentByPath<Dropdown>(settingField, "Other:Language:Dropdown");

            bool isResInit = false;
            // resolution dropdown list
            foreach (Resolution res in Screen.resolutions)
            {
                if (res.refreshRate == 60)
                {
                    if (!isResInit)
                    {
                        SettingModel.resolution = string.Format("{0}x{1}", res.width, res.height);
                        isResInit = true;
                    }
                    ResolutionDropdown.options.Add(new Dropdown.OptionData(string.Format("{0}x{1}", res.width, res.height)));
                }
            }
            ResolutionDropdown.RefreshShownValue();

            bgmMusic = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));

            historyQuene = new GameObject[] { null, titleContainer };
            if (null == GlobalGameData.GameValues) GlobalGameData.GameValues = new GameValues();

            defaultMenuLocalPos = menuField.transform.localPosition;
            shownMenuLocalPos = new Vector3(defaultMenuLocalPos.x, defaultMenuLocalPos.y - menuField.GetComponent<RectTransform>().sizeDelta.y, defaultMenuLocalPos.z);

            _bgmAudio.clip = bgmMusic;
            _bgmAudio.loop = true;
            _bgmAudio.Play();

            GlobalGameData.GameValues = DefaultValues.DEFAULT_GAMEVALUES;
            chapterController = this.GetComponent<ChapterController>();

            InitSettingField();
        }

        void FixedUpdate()
        {
            if (inGame)
            {
                if (Input.mousePosition.y > Screen.height - menuField.GetComponent<RectTransform>().sizeDelta.y)
                {
                    MoveMenu(defaultMenuLocalPos, shownMenuLocalPos, (Time.time - startTime) * 5.0f);
                    chapterController.isMenuActive = true;
                    chapterController.SetManualMode(true);
                }
                else
                {
                    MoveMenu(shownMenuLocalPos, defaultMenuLocalPos, (Time.time - startTime) * 5.0f);
                    chapterController.isMenuActive = false;
                }
            }
        }

        /// <summary>
        /// Render SettingField game objects with <see cref="SettingModel"/>
        /// </summary>
        public void InitSettingField()
        {
            PlayMode_Manual.isOn = SettingModel.isManualModeOn;
            PlayMode_Auto.isOn = SettingModel.isAutoReadingModeOn;
            PlayMode_Skip.isOn = SettingModel.isSkipModeOn;

            Visual_ShowCGInSkipMode.isOn = SettingModel.showCGInSkipMode;
            Visual_SpecialEffects.isOn = SettingModel.showSpecialEffects;
            Visual_TextShadow.isOn = SettingModel.showTextShadow;
            Visual_Animation.isOn = SettingModel.showAnimation;

            Volume_MuteBGM.isOn = SettingModel.isBgmMute;
            Volume_MuteVoices.isOn = SettingModel.isVoicesMute;
            Volume_MuteSound.isOn = SettingModel.isSoundMute;
            Volume_BGM.value = SettingModel.bgmVolume;
            Volume_Voices.value = SettingModel.voicesVolume;
            Volume_Sound.value = SettingModel.soundVolume;

            ScreenMode_FullScreen.isOn = SettingModel.isFullScreenModeOn;
            ScreenMode_Windowed.isOn = !SettingModel.isFullScreenModeOn;
            int resIndex = 0;
            foreach (Resolution res in Screen.resolutions)
            {
                if (res.refreshRate == 60)
                {
                    if (string.Format("{0}x{1}", res.width, res.height).Equals(SettingModel.resolution))
                    {
                        break;
                    }
                    resIndex++;
                }
            }
            ResolutionDropdown.value = resIndex;
            ResolutionDropdown.RefreshShownValue();

            MessageSpeed_UnreadText.value = SettingModel.textShowDuration / SettingModel.MAX_TEXT_SHOW_DURATION;
            MessageSpeed_AutoPlayDelayInterval.value = SettingModel.lineSwitchDuration / SettingModel.MAX_LINE_SWITCH_DURATION;
            MessageSpeed_SkipInterval.value = SettingModel.skipModeLineSwitchDuration / SettingModel.MAX_SKIP_MODE_LINE_SWITCH_DURATION;

            Other_AppActiveInBackground.isOn = SettingModel.appActiveInBackground;

            int langIndex = SettingModel.languages.IndexOf(SettingModel.appLanguage);
            LanguageDropdown.value = langIndex;
            LanguageDropdown.RefreshShownValue();

            // TODO: Add Character voices controller
        }

        /// <summary>
        /// Clear all display items in screen and show CG picture
        /// </summary>
        public void ShowCG()
        {
            titleContainer.SetActive(false);
            lineContainer.SetActive(false);
            historyField.SetActive(false);
            savedDataField.SetActive(false);
            settingField.SetActive(false);
            popupWindow.SetActive(false);
        }

        public void MoveMenu(Vector3 start, Vector3 end, float speed)
        {
            if (menuField.transform.localPosition != end)
            {
                menuField.transform.localPosition = Vector3.Lerp(start, end, speed);
            }
            else
            {
                startTime = Time.time;
            }
        }

        #region Title Menu Callback
        /// <summary>
        /// Play game from the very start
        /// </summary>
        public void NewGame()
        {
            inGame = true;
            bg.sprite = null;
            _bgmAudio.Stop();
            // DeactiveGameObject(titleContainer);
            historyQuene[1].SetActive(false);
            ReleaseMemory();
        }

        /// <summary>
        /// Shut down game immediately
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Menu Callback
        /// <summary>
        /// Return to the title screen
        /// </summary>
        public void BackToTitle()
        {
            bg.sprite = Resources.Load<Sprite>("Sprite/STT_BG00");
            _bgmAudio.clip = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));
            _bgmAudio.Play();
            inGame = false;

            gameObject.GetComponent<ChapterController>().ResetChapter();

            // Deactive display objects
            ShowCG();
            // Active title menu
            ActiveGameObject(titleContainer);
            // Release menory
            ReleaseMemory();
        }
        #endregion

        #region Saved data
        /// <summary>
        /// Save a saveddata at index of `index` and write to file [savedata.dat]
        /// </summary>
        /// <param name="savedData">This data model to be saved</param>
        /// <returns></returns>
        public bool PersistSavedDatas()
        {
            try
            {
                if (!Directory.Exists(savedDataPath))
                {
                    Directory.CreateDirectory(savedDataPath);
                }
                using (StreamWriter w = new StreamWriter(savedDataPath + savedDataFile, false, Encoding.UTF8))
                {
                    // TODO: This convertion will fail as the SavedDataModel contains
                    string savedDataJson = JsonConvert.SerializeObject(savedDatas);
                    Debug.Log(savedDataJson);
                    w.Write(savedDataJson);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load saved game data from saveddata file
        /// </summary>
        /// <returns>Return saved datas list, including null value</returns>
        public List<SavedDataModel> LoadSavedDatas()
        {
            if (!Directory.Exists(savedDataPath))
            {
                Directory.CreateDirectory(savedDataPath);
            }
            using (FileStream fs = new FileStream(savedDataPath + savedDataFile, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string savedDataJson = sr.ReadToEnd();
                    if (null == savedDatas) savedDatas = InitList<SavedDataModel>(savdDataPageCount * 12);
                    if (!string.IsNullOrEmpty(savedDataJson))
                    {
                        JArray savedDataJArray = JArray.Parse(savedDataJson);
                        foreach (JToken jSavedData in savedDataJArray)
                        {
                            if (null != jSavedData && jSavedData.Type != JTokenType.Null)
                            {
                                SavedDataModel thisModel = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());
                                savedDatas[thisModel.savedDataIndex] = thisModel;
                            }
                        }
                        return savedDatas;
                    }
                }
            }
            return InitList<SavedDataModel>(savdDataPageCount * 12);
        }

        /// <summary>
        /// Load a saved data from the specific `index` for all saved datas
        /// </summary>
        /// <param name="index">The specific index</param>
        /// <returns>Return null if data in the `index` is null</returns>
        public SavedDataModel LoadSavedData(int index)
        {
            return savedDatas[index];
        }
        #endregion

        #region Popup Window
        /// <summary>
        /// Open popup window with comfirm callback
        /// </summary>
        /// <param name="callback">The comfirm callback</param>
        public void OpenPopup(string callback)
        {
            OpenPopup(callback, null);
        }

        /// <summary>
        /// Open popup window with comfirm callback
        /// </summary>
        /// <param name="callback">The comfirm callback</param>
        /// <param name="notice">The notice text</param>
        public void OpenPopup(string callback, string notice)
        {
            if (!string.IsNullOrEmpty(notice)) popupNoticeText.text = notice;
            popupCallbackText.text = callback.Trim();
            // ActiveGameObject(popupWindow);
            popupWindow.SetActive(true);
        }

        /// <summary>
        /// Close popup window
        /// </summary>
        public void ClosePopup()
        {
            // DeactiveGameObject(popupWindow);
            popupNoticeText.text = DEFAULT_POPUP_NOTICE_TEXT;
            popupWindow.SetActive(false);
        }

        /// <summary>
        /// popup window confirm button click listener
        /// </summary>
        public void PopupCallback()
        {
            switch (popupCallbackText.text)
            {
                case "QuitGame":
                    QuitGame();
                    break;
                case "BackToTitle":
                    BackToTitle();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Setting
        /// <summary>
        /// Set <see cref="SettingModel.textShowDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetTextShowDuration(float ratio)
        {
            SettingModel.textShowDuration = SettingModel.MAX_TEXT_SHOW_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.lineSwitchDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetLineSwitchDuration(float ratio)
        {
            SettingModel.lineSwitchDuration = SettingModel.MAX_LINE_SWITCH_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.skipModeLineSwitchDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetSkipModeLineSwitchDuration(float ratio)
        {
            SettingModel.skipModeLineSwitchDuration = SettingModel.MAX_SKIP_MODE_LINE_SWITCH_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.appActiveInBackground"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetAppActiveInBackground(bool active)
        {
            SettingModel.appActiveInBackground = active;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showCGInSkipMode"/>
        /// </summary>
        /// <param name="show"></param>
        public void SetShowCGInSkipMode(bool show)
        {
            SettingModel.showCGInSkipMode = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showSpecialEffects"/>
        /// </summary>
        /// <param name="show"></param>
        public void SetShowSpecialEffects(bool show)
        {
            SettingModel.showSpecialEffects = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showTextShadow"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetShowTextShadow(bool show)
        {
            SettingModel.showTextShadow = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showAnimation"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetShowAnimation(bool show)
        {
            SettingModel.showAnimation = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.appLanguage"/> with value in <see cref="SettingModel.languages"/> via <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        public void SetAppLanguage(int index)
        {
            SettingModel.appLanguage = SettingModel.languages[index];
            // TODO: Work after rebooting or work imediately?
        }

        /// <summary>
        /// Request full screen mode
        /// </summary>
        public void SetFullScreenMode(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
            SettingModel.isFullScreenModeOn = fullScreen;
        }

        /// <summary>
        /// Set application run with windowed mode
        /// </summary>
        /// <param name="windowed"></param>
        public void SetWindowedMode(bool windowed)
        {
            SetFullScreenMode(!windowed);
        }

        /// <summary>
        /// Set resoulution
        /// </summary>
        /// <param name="index">The index in <see cref="Screen.resolutions"/></param>
        public void SetWindowedResoulution(int index)
        {
            Resolution selected = Screen.resolutions[index];
            Screen.SetResolution(selected.width, selected.height, SettingModel.isFullScreenModeOn);
            SettingModel.resolution = string.Format("{0}x{1}", selected.width, selected.height);
        }

        /// <summary>
        /// Set volume of BGM to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetBgmMute(bool mute)
        {
            _bgmAudio.mute = mute;
            SettingModel.isBgmMute = mute;
        }

        /// <summary>
        /// Set volume of Voices to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetVoicesMute(bool mute)
        {
            _voiceAudio.mute = mute;
            SettingModel.isVoicesMute = mute;
        }

        /// <summary>
        /// Set volume of BGM
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetBgmVolume(float volume)
        {
            _bgmAudio.volume = volume;
            SettingModel.bgmVolume = volume;
        }

        /// <summary>
        /// Set volume of Voices
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetVoicesVolume(float volume)
        {
            _voiceAudio.volume = volume;
            SettingModel.voicesVolume = volume;
        }

        /// <summary>
        /// Save current <see cref="SettingModel"/> to config file.
        /// </summary>
        public void PersistSettingConfig()
        {
            if (!Directory.Exists(settingConfigPath))
            {
                Directory.CreateDirectory(settingConfigPath);
            }

            using (StreamWriter configWriter = new StreamWriter(settingConfigFullName, false))
            {
                configWriter.WriteLine(JsonConvert.SerializeObject(new SerializableSettingModel(true)));
            }
        }

        /// <summary>
        /// Initialize <see cref="SettingModel"/> via app.config
        /// </summary>
        public void InitializeSettingConfig()
        {
            // No config file exist, return
            if (!Directory.Exists(settingConfigPath) || !File.Exists(settingConfigFullName)) return;
            // Read config
            string configInJson = string.Empty;
            using (StreamReader configReader = new StreamReader(settingConfigFullName))
            {
                configInJson = configReader.ReadToEnd();
            }
            // Init
            SerializableSettingModel settingModel = null;
            try
            {
                settingModel = JsonConvert.DeserializeObject<SerializableSettingModel>(configInJson);
            }
            catch
            {
                // Log
            }
            SettingModel.Update(settingModel);
        }

        /// <summary>
        /// Reset <see cref="SettingModel"/> as default.
        /// </summary>
        public void ResetSettingConfig()
        {
            SettingModel.Update(new SerializableSettingModel());
        }
        #endregion

        /// <summary>
        /// To active a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void ActiveGameObject(GameObject gameObject)
        {
            if (null != historyQuene[1]) historyQuene[1].SetActive(false);
            gameObject.SetActive(true);
            historyQuene[0] = historyQuene[1];
            historyQuene[1] = gameObject;
            Debug.Log(DateTime.Now.ToString() + ": " + (null == historyQuene[0] ? "" : historyQuene[0].name) + " - " + (null == historyQuene[1] ? "" : historyQuene[1].name));
            // gameObject.SetActive(true);
        }

        /// <summary>
        /// To deactive a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void DeactiveGameObject(GameObject gameObject)
        {
            if (null != historyQuene[0]) historyQuene[0].SetActive(true);
            gameObject.SetActive(false);
            historyQuene[1] = historyQuene[0];
            historyQuene[0] = gameObject;
            Debug.Log(DateTime.Now.ToString() + ": " + (null == historyQuene[0] ? "" : historyQuene[0].name) + " - " + (null == historyQuene[1] ? "" : historyQuene[1].name));
            //gameObject.SetActive(false);
        }

        /// <summary>
        /// Release RAM
        /// </summary>
        public void ReleaseMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// Init a empty <see cref="List"/> with size
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="size">length of list</param>
        /// <returns></returns>
        public List<T> InitList<T>(int size)
        {
            List<T> newList = new List<T>();
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    newList.Add(default(T));
                }
            }
            return newList;
        }

        public void UpdateGlobalGameValues(GameValues gameValues)
        {
            List<FieldInfo> typeFields = new List<FieldInfo>();
            EntityUtil.GetTypeFields<int>(GlobalGameData.GameValues, typeFields);

            foreach (FieldInfo f in typeFields)
            {
                string fieldName = f.Name;
                int t1 = EntityUtil.GetDeepValue<int>(gameValues, fieldName);
                int t2 = EntityUtil.GetDeepValue<int>(GlobalGameData.GameValues, fieldName);
                int fieldValue = EntityUtil.GetDeepValue<int>(GlobalGameData.GameValues, fieldName) + EntityUtil.GetDeepValue<int>(gameValues, fieldName);

                EntityUtil.SetDeepValue(GlobalGameData.GameValues, fieldName, fieldValue);
            }

            //if (null != gameValues.RoleAbility)
            //{
            //    RoleAbility ability = GlobalGameData.GameValues.RoleAbility;
            //    ability.Attack += gameValues.RoleAbility.Attack;
            //    ability.Defence += gameValues.RoleAbility.Defence;
            //    ability.Evasion += gameValues.RoleAbility.Evasion;
            //}
            //if (null != gameValues.RoleStatus)
            //{
            //    RoleStatus status = GlobalGameData.GameValues.RoleStatus;
            //    status.HealthPoint += gameValues.RoleStatus.HealthPoint;
            //    status.ManaPoint += gameValues.RoleStatus.ManaPoint;
            //    status.SkillPoint += gameValues.RoleStatus.SkillPoint;
            //    status.ExperiencePoint += gameValues.RoleStatus.ExperiencePoint;
            //}
            //GlobalGameData.GameValues.ExampleOtherValue += gameValues.ExampleOtherValue;
        }
    }
}
