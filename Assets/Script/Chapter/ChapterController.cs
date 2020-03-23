using Assets.Script.Model;
using Assets.Script.Utility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Video;
using UnityEditor;
using Assets.Script.Model.Components;
using System.Linq;
using Assets.Script.Model.Datas;

namespace Assets.Script.Chapter
{
    public class ChapterController : MonoBehaviour
    {

        #region Fields
        private static string rootPath;
        private static string savedDataPath;
        private static string savedDataFile;

        private GameObject titleContainer;
        private GameObject lineContainer;
        private GameObject popupWindow;
        private GameObject menuField;
        private GameObject selectorField;
        private Text actorName;
        private Text line;   // Script line showing text
        private Font font;

        #region Game objects
        public GameObject mainCanvas;

        #region Prefabs
        public Text historyTextPrefab;
        public Button saveDataModelPrefab;
        public Button selectorOptionPrefab;
        #endregion

        #region History gameobjects
        private GameObject historyField;
        public GameObject historyTexts;
        private Text currentActiveHistoryText;
        #endregion

        #region Saved data field
        private GameObject savedDataField;
        private GameObject savedDataPanel;
        private static List<List<Button>> savedDataButtons;
        private static List<SavedDataModel> savedDatas;
        private static int savdDataPageCount;
        // Last loaded savedDataPage
        private static int lastLoadedSavedDataPage;
        #endregion

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

        #region Operation buttons
        public GameObject operationButtons;
        private GameObject autoPlayButton;
        private GameObject skipButton;
        private GameObject saveButton;
        private GameObject loadButton;
        private GameObject settingButton;
        #endregion

        #region Selector field
        private GameObject selector;
        #endregion
        #endregion

        // Game objects end

        // TODO: GalgameScript object may be a large data, so something must be done to optimize RAM.
        // Current displaying script
        public GalgameScript currentScript;
        public SpriteRenderer bgSpriteRenderer;
        public GameObject videoPlayer;
        public GameObject audioSource;
        public GameObject voiceAudioSource;

        private List<GalgameAction> galgameActions;
        private GalgameAction currentGalgameAction;
        private PSelectorOption ActiveSelectorOption;
        private int currentSelectorOptionActionIndex;
        // Current showing line's index in `currentScript`
        private int currentLineIndex;
        // Current showing line text's index of char
        private int currentLineCharIndex;
        // Is a line text is showing
        private bool isShowingLine;
        // Is now a option line text display time
        private bool isShowingSelectorOptionActionTime;
        // Is auto-reading mode is actived
        // private bool isAutoReadingModeOn;
        // Is skip mode is actived
        // private bool isSkipModeOn;
        // The previous skip time
        private DateTime preSkipTime;
        // Is menu is actived
        public bool isMenuActive;
        // Is saving saved data now
        private bool isSavingGameData;
        // Is loading saved data now
        private bool isLoadingSavedData;
        // Next line to display
        private string nextLine;

        private GameController gameController;

        private Coroutine currentTextShowCoroutine;
        private Coroutine currentLineSwitchCoroutine;
        private Coroutine hidehistoryFieldCoroutine;
        private static WaitForSeconds textShowWaitForSeconds;
        private static WaitForSeconds lineSwitchWaitForSeconds;
        private static WaitForSeconds skipModeLineSwitchWaitForSeconds;
        private VideoPlayer _video;
        private AudioSource _bgmAudio;
        private AudioSource _voiceAudio;
        private AudioClip bgmMusic;
        #endregion

        // Use this for initialization
        void Start()
        {
            gameController = Camera.main.GetComponent<GameController>();

            // Container
            titleContainer = mainCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = mainCanvas.transform.Find("LineContainer").gameObject;
            historyField = mainCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = mainCanvas.transform.Find("SavedDataField").gameObject;
            settingField = mainCanvas.transform.Find("SettingField").gameObject;
            popupWindow = mainCanvas.transform.Find("PopupWindow").gameObject;
            menuField = mainCanvas.transform.Find("MenuField").gameObject;
            selectorField = mainCanvas.transform.Find("SelectorField").gameObject;

            // Init line
            // Init currentScript, galgameActions, currentLineIndex
            // currentLineIndex = 0; // ?
            line = lineContainer.transform.Find("Line").GetComponent<Text>();
            actorName = lineContainer.transform.Find("ActorName").GetComponent<Text>();

            galgameActions = currentScript.GalgameActions;
            if (null != currentScript.Bg)
            {
                bgSpriteRenderer.sprite = currentScript.Bg;
            }

            textShowWaitForSeconds = new WaitForSeconds(SettingModel.textShowDuration);
            lineSwitchWaitForSeconds = new WaitForSeconds(SettingModel.lineSwitchDuration);
            skipModeLineSwitchWaitForSeconds = new WaitForSeconds(SettingModel.skipModeLineSwitchDuration);

            // opearation buttons
            autoPlayButton = operationButtons.transform.Find("AutoPlayBtn").gameObject;
            skipButton = operationButtons.transform.Find("SkipBtn").gameObject;
            saveButton = operationButtons.transform.Find("SaveBtn").gameObject;
            loadButton = operationButtons.transform.Find("LoadBtn").gameObject;
            settingButton = operationButtons.transform.Find("SettingBtn").gameObject;

            // media player
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _bgmAudio = audioSource.GetComponent<AudioSource>();
            _voiceAudio = voiceAudioSource.GetComponent<AudioSource>();

            // setting field
            PlayMode_Manual = FindComponentByPath<Toggle>(settingField, "PlayMode:Manual:Toggle");
            PlayMode_Auto = FindComponentByPath<Toggle>(settingField, "PlayMode:Auto:Toggle");
            PlayMode_Skip = FindComponentByPath<Toggle>(settingField, "PlayMode:Skip:Toggle");
            Visual_ShowCGInSkipMode = FindComponentByPath<Toggle>(settingField, "Visual:ShowCGInSkipMode:Toggle");
            Visual_SpecialEffects = FindComponentByPath<Toggle>(settingField, "Visual:ShowCGInSkipMode:Toggle");
            Visual_TextShadow = FindComponentByPath<Toggle>(settingField, "Visual:TextShadow:Toggle");
            Visual_Animation = FindComponentByPath<Toggle>(settingField, "Visual:Animation:Toggle");
            ScreenMode_FullScreen = FindComponentByPath<Toggle>(settingField, "ScreenMode:FullScreen:Toggle");
            ScreenMode_Windowed = FindComponentByPath<Toggle>(settingField, "ScreenMode:Windowed:Toggle");
            Other_AppActiveInBackground = FindComponentByPath<Toggle>(settingField, "Other:AppActiveInBackground:Toggle");
            Volume_MuteBGM = FindComponentByPath<Toggle>(settingField, "Volume:BGM:Toggle");
            Volume_MuteVoices = FindComponentByPath<Toggle>(settingField, "Volume:Voices:Toggle");
            Volume_MuteSound = FindComponentByPath<Toggle>(settingField, "Volume:Sound:Toggle");
            Volume_BGM = FindComponentByPath<Slider>(settingField, "Volume:BGM:Slider");
            Volume_Voices = FindComponentByPath<Slider>(settingField, "Volume:Voices:Slider");
            Volume_Sound = FindComponentByPath<Slider>(settingField, "Volume:Sound:Slider");
            MessageSpeed_UnreadText = FindComponentByPath<Slider>(settingField, "MessageSpeed:UnreadText:Slider");
            MessageSpeed_AutoPlayDelayInterval = FindComponentByPath<Slider>(settingField, "MessageSpeed:AutoPlayDelayInterval:Slider");
            MessageSpeed_SkipInterval = FindComponentByPath<Slider>(settingField, "MessageSpeed:SkipInterval:Slider");
            ResolutionDropdown = FindComponentByPath<Dropdown>(settingField, "ScreenMode:Windowed:Resolution");
            LanguageDropdown = FindComponentByPath<Dropdown>(settingField, "Other:Language:Dropdown");

            // selector field
            selector = selectorField.transform.Find("Selector").gameObject;

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

            // savedDatas = gameController.LoadSavedDatas();
            savedDataPanel = savedDataField.transform.Find("SavedDataPanel").gameObject;
            lastLoadedSavedDataPage = GameController.lastLoadedSavedDataPage;
            savdDataPageCount = GameController.savdDataPageCount;
            savedDatas = GameController.savedDatas;
            savedDataButtons = gameController.InitList<List<Button>>(savdDataPageCount);

            // InitSceneGameObject();
        }



        // Update is called once per frame
        void Update()
        {
            // mosue left button click
            if (Input.GetButtonDown("Fire1"))
            {

                if (titleContainer.activeSelf)
                {
                    return;
                }

                GameObject hitUIObject = null;

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    hitUIObject = GetMouseOverUIObject(mainCanvas);
                    Debug.Log("---- EventSystem.current.IsPointerOverGameObject ----" + hitUIObject);
                }

                if (null != hitUIObject && hitUIObject.tag.Trim() == "OperationButton")
                {
                    Debug.Log("hitUIObject.name: " + hitUIObject.name);
                    switch (hitUIObject.name)
                    {
                        case "CloseSaveData":
                            isSavingGameData = false;
                            isLoadingSavedData = false;
                            SetActiveSavedDataPanel(0);
                            break;
                    }
                }

                if (currentLineIndex <= galgameActions.Count && IsSwitchLineAllowed())
                {
                    if (!lineContainer.activeSelf)
                    {
                        gameController.ActiveGameObject(lineContainer);
                    }
                    if (null == hitUIObject || (null != hitUIObject && hitUIObject.tag.Trim() != "OperationButton"))
                    {
                        SwitchLine();
                    }
                }
            }

            if (SettingModel.isSkipModeOn && IsSwitchLineAllowed() && (null == preSkipTime || (DateTime.Now - preSkipTime).TotalSeconds > SettingModel.skipModeLineSwitchDuration))
            {
                if (!lineContainer.activeSelf)
                {
                    gameController.ActiveGameObject(lineContainer);
                }
                SwitchLine();
                preSkipTime = DateTime.Now;
            }

            // mouse scroll up
            if (Input.mouseScrollDelta.y > 0 && !historyField.activeSelf && gameController.inGame)
            {
                // TODO: Fix bug of adding double history text
                ShowhistoryField();
            }
        }

        #region Pubilc scene action
        /// <summary>
        /// Show history TextView
        /// </summary>
        public void ShowhistoryField()
        {
            historyField.SetActive(true);
            // If `currentTextShowCoroutine` is going
            if (isShowingLine)
            {
                ShowLineImmediately();
            }
            SetManualMode(true);
        }

        /// <summary>
        /// Hide history TextView
        /// </summary>
        public void HideHistoryField()
        {
            hidehistoryFieldCoroutine = StartCoroutine(HideHistoryFieldTimeOut());
        }

        /// <summary>
        /// Set reading mode to manual
        /// </summary>
        /// <param name="manual"></param>
        public void SetManualMode(bool manual)
        {
            SettingModel.isManualModeOn = manual;
            if (manual)
            {
                SetAutoMode(false);
                SetSkipMode(false);
                StopAllCoroutines();
                // SettingModel.isAutoReadingModeOn = false;
                // SettingModel.isSkipModeOn = false;
            }
        }

        /// <summary>
        /// Auto Reading
        /// </summary>
        public void AutoReading()
        {
            SetAutoMode(!SettingModel.isAutoReadingModeOn);
        }

        /// <summary>
        /// Set auto mode
        /// </summary>
        /// <param name="auto"></param>
        public void SetAutoMode(bool auto)
        {
            skipButton.GetComponent<Image>().color = Color.white;
            SettingModel.isAutoReadingModeOn = auto;
            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!auto)
            {
                autoPlayButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                SettingModel.isManualModeOn = false;
                SettingModel.isSkipModeOn = false;
            }
            if (SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed() && !SettingModel.isSkipModeOn && !isShowingLine)
            {
                autoPlayButton.GetComponent<Image>().color = Color.black;
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Enable/Disable skip mode
        /// </summary>
        public void ChangeSkipMode()
        {
            SetSkipMode(!SettingModel.isSkipModeOn);
        }

        /// <summary>
        /// Set skip mode
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipMode(bool skip)
        {
            autoPlayButton.GetComponent<Image>().color = Color.white;
            SettingModel.isSkipModeOn = skip;
            if (!skip)
            {
                skipButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                SettingModel.isManualModeOn = false;
                SettingModel.isAutoReadingModeOn = false;
            }
            if (SettingModel.isSkipModeOn && IsSwitchLineAllowed())
            {
                skipButton.GetComponent<Image>().color = Color.black;
                StopAllCoroutines();
                ShowLineImmediately();
                SwitchLine();
            }
        }

        /// <summary>
        /// Save game data
        /// </summary>
        public void SaveData()
        {
            isSavingGameData = true;
            SetSavedDataModelButtons(0, 12);
            // gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
            SetManualMode(true);
            Debug.Log(string.Format("Save Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Quick save
        /// </summary>
        public void QuickSave()
        {
            Debug.Log(string.Format("Quick Save Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Load saved data
        /// </summary>
        public void LoadSavedData()
        {
            isLoadingSavedData = true;
            SetSavedDataModelButtons(0, 12);
            // gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
            SetManualMode(true);
            Debug.Log(string.Format("Load Saved Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Switch page display to display saved data
        /// </summary>
        /// <param name="step"></param>
        public void SwitchSavedDataPage(int step)
        {
            // If out of range, do nothing
            if (lastLoadedSavedDataPage + step < 0 || lastLoadedSavedDataPage + step >= savdDataPageCount) return;
            // Hide previous display saved data page
            Transform target = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != target)
            {
                target.gameObject.SetActive(false);
                // gameController.DeactiveGameObject(target.gameObject);
            }

            lastLoadedSavedDataPage += step;
            Transform nTarget = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != nTarget)
            {
                nTarget.gameObject.SetActive(true);
                // gameController.ActiveGameObject(nTarget.gameObject);
            }
            // Set now display saved data page
            SetSavedDataModelButtons(lastLoadedSavedDataPage);
        }

        /// <summary>
        /// Open setting field
        /// </summary>
        public void OpenSetting()
        {
            // gameController.ShowCG();
            gameController.ActiveGameObject(settingField);
            SetManualMode(true);
        }

        /// <summary>
        /// Close setting field
        /// </summary>
        public void CloseSetting()
        {
            gameController.DeactiveGameObject(settingField);
            gameController.PersistSettingConfig();
            GameObject lineObject = lineContainer.transform.Find("Line").gameObject;
            if (SettingModel.showTextShadow)
            {
                if (!lineObject.GetComponent<Shadow>())
                {
                    Shadow s = lineObject.AddComponent<Shadow>();
                    s.effectDistance = new Vector2(2, -1);
                }
            }
            else
            {
                if (lineObject.GetComponent<Shadow>())
                {
                    Destroy(lineObject.GetComponent<Shadow>());
                }
            }
        }

        /// <summary>
        /// Reset 
        /// </summary>
        public void ResetChapter()
        {
            currentLineIndex = 0;
            currentLineCharIndex = 0;
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

        #endregion

        #region Private methods
        /// <summary>
        /// Find child game component of <paramref name="parent"/> via <paramref name="fullPath"/>
        /// </summary>
        /// <typeparam name="T">Type of the game component</typeparam>
        /// <param name="parent">parent game object</param>
        /// <param name="fullPath">path format in `path1:path2:path3...`</param>
        /// <returns>The component found or null</returns>
        private T FindComponentByPath<T>(GameObject parent, string fullPath)
        {
            return (null == FindChildTransform(parent, fullPath) ? default(T) : FindChildTransform(parent, fullPath).GetComponent<T>());
        }

        /// <summary>
        /// Find child tramsform of <paramref name="parent"/> via <paramref name="fullPath"/>
        /// </summary>
        /// <param name="parent">parent game object</param>
        /// <param name="fullPath">path format in `path1:path2:path3...`</param>
        /// <returns>The transform or null</returns>
        private Transform FindChildTransform(GameObject parent, string fullPath)
        {
            Transform tmp = parent.transform;
            string[] ps = fullPath.Split(':');
            foreach (string p in ps)
            {
                if ((tmp = tmp.Find(p)) == null) return null;
            }
            return tmp;
        }

        private void InitSceneGameObject()
        {
            Vector2 sceneDeltaSize = mainCanvas.GetComponent<RectTransform>().sizeDelta;

            historyField.GetComponent<RectTransform>().sizeDelta = sceneDeltaSize;
            savedDataField.GetComponent<RectTransform>().sizeDelta = sceneDeltaSize;
        }

        /// <summary>
        /// Whether switch line operation is allow or not
        /// </summary>
        /// <returns></returns>
        private bool IsSwitchLineAllowed()
        {
            return gameController.inGame && !isMenuActive && !historyField.activeSelf && !savedDataField.activeSelf && !settingField.activeSelf && !popupWindow.activeSelf && !selectorField.activeSelf;
        }

        /// <summary>
        /// Hide history TextView with duration
        /// </summary>
        private IEnumerator HideHistoryFieldTimeOut()
        {
            if (historyField.activeSelf)
            {
                historyField.SetActive(false);
                if (SettingModel.isAutoReadingModeOn)
                {
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
        }

        /// <summary>
        /// Hide history TextView with duration
        /// </summary>
        private IEnumerator HideSelectorFieldTimeOut()
        {
            if (selectorField.activeSelf)
            {
                selectorField.SetActive(false);
                if (SettingModel.isAutoReadingModeOn)
                {
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
        }

        /// <summary>
        /// Show new line controller
        /// </summary>
        private void SwitchLine()
        {
            line.text = string.Empty; // clear previous line
            if (isShowingLine)
            {
                Debug.Log(DateTime.Now.ToString() + "准备跳过");
                if (null != currentTextShowCoroutine)
                {
                    StopCoroutine(currentTextShowCoroutine);
                    ShowLineImmediately(nextLine);
                    AddHistoryText(nextLine); // Add line to history text list
                    isShowingLine = false;
                    Debug.Log(DateTime.Now.ToString() + "已跳过");
                    return;
                }
            }

            currentLineCharIndex = -1; // read from index: -1

            if (isShowingSelectorOptionActionTime)
            {
                GalgamePlainAction action = ActiveSelectorOption.Actions[currentSelectorOptionActionIndex];

                BuildAAction(action);
                if (++currentSelectorOptionActionIndex >= ActiveSelectorOption.Actions.Count)
                {
                    isShowingSelectorOptionActionTime = false;
                };
            }
            else
            {
                if (currentLineIndex == galgameActions.Count)
                {
                    // this chapter is end
                    if (SettingModel.isSkipModeOn)
                    {
                        SetSkipMode(false);
                    }
                    if (SettingModel.isAutoReadingModeOn)
                    {
                        SetAutoMode(false);
                    }
                    // TODO: maybe consider loading another chapter?
                    line.text = "『つづく...』";
                    return;
                }

                Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
                currentGalgameAction = galgameActions[currentLineIndex];

                BuildAAction(currentGalgameAction);

                // Move index to next
                currentLineIndex++;
            }

        }

        /// <summary>
        /// To build a action
        /// </summary>
        /// <param name="action"></param>
        private void BuildAAction(GalgamePlainAction action)
        {
            nextLine = action.Line.text.Replace("\\n", "\n");
            if (SettingModel.isSkipModeOn)
            {
                ShowLineImmediately();
            }
            else
            {
                currentTextShowCoroutine = StartCoroutine(ShowLineTimeOut(nextLine));
            }
            if (null != action.Bgm)
            {
                // bgm
                _bgmAudio.clip = action.Bgm;
                _bgmAudio.Play();
            }
            if (null != action.Voice)
            {
                // voice
                _voiceAudio.clip = action.Voice;
                _voiceAudio.Play();
            }
            if (action.Actor != Actor.NULL)
            {
                // actor's name
                actorName.text = action.Actor.ToString();
            }
            if (null != action.Background)
            {
                // current background
                bgSpriteRenderer.sprite = action.Background;
            }
            // text-align
            line.alignment = EnumMap.AlignToTextAnchor(action.Line.align);

            // font
            if (null != font)
            {
                line.font = font;
            }
            // font-style
            if (action.Line.fstyle == FontStyle.Normal)
            {
                line.fontStyle = DefaultScriptProperty.fstyle;
            }
            else
            {
                line.fontStyle = action.Line.fstyle;
            }
            // font-size
            if (action.Line.fsize != 0)
            {
                line.fontSize = Mathf.RoundToInt(action.Line.fsize);
            }
            else if (DefaultScriptProperty.fsize != 0)
            {
                line.fontSize = Mathf.RoundToInt(DefaultScriptProperty.fsize);
            }
            // line-spacing
            if (action.Line.linespacing != 0)
            {
                line.lineSpacing = action.Line.linespacing;
            }
            else if (DefaultScriptProperty.linespacing != 0)
            {
                line.lineSpacing = DefaultScriptProperty.linespacing;
            }
            // font-color
            if (!string.IsNullOrEmpty(action.Line.fcolor))
            {
                line.color = ColorUtil.HexToUnityColor(uint.Parse(action.Line.fcolor, System.Globalization.NumberStyles.HexNumber));
            }
            else if (!string.IsNullOrEmpty(DefaultScriptProperty.fcolor))
            {
                line.color = ColorUtil.HexToUnityColor(uint.Parse(DefaultScriptProperty.fcolor, System.Globalization.NumberStyles.HexNumber));
            }

            if(action.GetType().Equals(typeof(GalgameAction)))
            {
                
                // Is there a selector
                if (null != ((GalgameAction)action).Selector)
                {
                    BuildSelector(((GalgameAction)action).Selector);
                }
            }
        }

        /// <summary>
        /// Build current scene's Selector component
        /// </summary>
        private void BuildSelector(PSelector selector)
        {
            if ((null == selector.Options || selector.Options.Count == 0) && (null == selector.Texts || selector.Texts.Count == 0) 
                && (null == selector.Bgms || selector.Bgms.Count == 0) && (null == selector.Bgs || selector.Bgs.Count == 0))
                return;

            // Selector.Options has higher priority than Options set on properties(Texts,Bgs,Bgms) of Selector
            if (null == selector.Options || selector.Options.Count == 0)
            {
                selector.Options = BuildSelectorOptions(selector);
                // TODO: Consider update chapter
            }

            // Re-build Selector Container
            if (null != this.selector)
            {
                Destroy(this.selector);
            }
            this.selector = new GameObject("Selector");
            Image image = this.selector.AddComponent<Image>();
            image.color = new Color(1.0f, 1.0f, 1.0f, 0f);

            Vector2 opSize = Vector2.zero;
            Vector2 opSpacing = new Vector2(10.0f, 10.0f);
            int opNumber = selector.Options.Count;

            Grid selectorGrid = this.selector.AddComponent<Grid>();
            GridLayoutGroup selectorGroup = this.selector.AddComponent<GridLayoutGroup>();
            selectorGroup.spacing = opSpacing;

            // Set size of selector panel
            switch (selector.Type)
            {
                case Model.Enum.SelectorType.Horizontal:
                    opSize = new Vector2(200.0f, 300.0f);
                    selectorGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    selectorGroup.cellSize = opSize;
                    this.selector.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(opNumber * opSize.x + (opNumber - 1) * opSize.x, opSize.y);
                    break;
                case Model.Enum.SelectorType.Vertical:
                    opSize = new Vector2(400.0f, 80.0f);
                    selectorGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    selectorGroup.cellSize = opSize;
                    this.selector.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(opSize.x, opNumber * opSize.y + (opNumber - 1) * opSpacing.y);
                    break;
            }
            selectorGroup.constraintCount = opNumber;

            foreach (PSelectorOption option in selector.Options)
            {
                Button newEmptyOption = Instantiate(selectorOptionPrefab);
                Button.ButtonClickedEvent optionClickEvent = new Button.ButtonClickedEvent();
                optionClickEvent.AddListener(() =>
                {
                    selector.IsSelected = true;
                    selector.SelectedItem = selector.Options.IndexOf(option);
                    if(null != option.Actions && option.Actions.Count > 0)
                    {
                        this.ActiveSelectorOption = option;
                        this.isShowingSelectorOptionActionTime = true;
                    }
                    gameController.UpdateGlobalGameValues(option.DeltaGameValues);                  // Update global values
                    StartCoroutine(HideSelectorFieldTimeOut());                                     // Hide selector panel
                    SwitchLine();
                    Debug.Log("Global Values: \n"+GlobalGameData.GameValues.ToJSONString());
                });
                newEmptyOption.onClick = optionClickEvent;
                if (null != option.Bg)
                {
                    newEmptyOption.GetComponent<RawImage>().texture = option.Bg.texture;
                }
                if(null != option.Bgm)
                {
                    newEmptyOption.GetComponent<AudioSource>().clip = option.Bgm;
                }
                Text ot = newEmptyOption.transform.Find("OptionText").GetComponent<Text>();
                ot.text = option.Text.text;
                ot.alignment = EnumMap.AlignToTextAnchor(option.Text.align);
                // font-style
                if (option.Text.fstyle == FontStyle.Normal)
                {
                    ot.fontStyle = DefaultScriptProperty.fstyle;
                }
                else
                {
                    ot.fontStyle = option.Text.fstyle;
                }
                // font-size
                if (option.Text.fsize != 0)
                {
                    ot.fontSize = Mathf.RoundToInt(option.Text.fsize);
                }
                else if (DefaultScriptProperty.fsize != 0)
                {
                    ot.fontSize = Mathf.RoundToInt(DefaultScriptProperty.fsize);
                }
                // line-spacing
                if (option.Text.linespacing != 0)
                {
                    ot.lineSpacing = option.Text.linespacing;
                }
                else if (DefaultScriptProperty.linespacing != 0)
                {
                    ot.lineSpacing = DefaultScriptProperty.linespacing;
                }
                // font-color
                if (!string.IsNullOrEmpty(option.Text.fcolor))
                {
                    ot.color = ColorUtil.HexToUnityColor(uint.Parse(option.Text.fcolor, System.Globalization.NumberStyles.HexNumber));
                }
                else if (!string.IsNullOrEmpty(DefaultScriptProperty.fcolor))
                {
                    ot.color = ColorUtil.HexToUnityColor(uint.Parse(DefaultScriptProperty.fcolor, System.Globalization.NumberStyles.HexNumber));
                }
                newEmptyOption.transform.SetParent(this.selector.transform);
            }
            // this.selector.GetComponent<RectTransform>().position = Vector3.zero;
            this.selector.transform.SetParent(selectorField.transform);
            this.selector.transform.localScale = Vector3.one;
            selectorField.SetActive(true);
        }

        /// <summary>
        /// Build Selector's Options
        /// </summary>
        private List<PSelectorOption> BuildSelectorOptions(PSelector selector)
        {
            List<PSelectorOption> options = new List<PSelectorOption>();
            int optionNumber = new int[] { selector.Bgms.Count, selector.Bgs.Count, selector.Texts.Count }.Max();
            for(int n = 0; n < optionNumber; n++)
            {
                PSelectorOption o = new PSelectorOption();
                o.Text = n > selector.Texts.Count-1 ? default(PText) : selector.Texts[n];
                o.Bg = n > selector.Bgs.Count-1 ? default(Sprite) : selector.Bgs[n];
                o.Bgm = n > selector.Bgms.Count-1 ? default(AudioClip) : selector.Bgms[n];
                options.Add(o);
            }
            return options;
        }

        /// <summary>
        /// Set current active history text when a history text clicked
        /// </summary>
        private void SetCurrentActiveHistoryText(Text nextActiveHistoryText)
        {
            if (null != currentActiveHistoryText)
            {
                currentActiveHistoryText.color = Color.white;
            }
            nextActiveHistoryText.color = Color.cyan;
            currentActiveHistoryText = nextActiveHistoryText;
        }

        /// <summary>
        /// Show line text with duration <see cref="textShowDuration"/>
        /// </summary>
        /// <param name="newLine">A new full-text line</param>
        /// <param name="autoSwitchLine">Auto reading mode</param>
        /// <returns></returns>
        private IEnumerator ShowLineTimeOut(string newLine, bool autoSwitchLine = false)
        {
            isShowingLine = true;
            if(string.IsNullOrEmpty(newLine))
            {
                if (SettingModel.isAutoReadingModeOn)
                {
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
            foreach (char lineChar in newLine)
            {
                currentLineCharIndex++;
                line.text += lineChar;
                if (currentLineCharIndex == newLine.Length - 1)
                {
                    isShowingLine = false;
                    Debug.Log(DateTime.Now.ToString() + "清除ShowingLine状态");
                    AddHistoryText(newLine); // Add line to history text list
                    Debug.Log("currentLineCharIndex: " + currentLineCharIndex);
                    if (SettingModel.isAutoReadingModeOn)
                    {
                        yield return lineSwitchWaitForSeconds;
                        SwitchLine();
                    }
                }
                yield return textShowWaitForSeconds;
            }
        }

        /// <summary>
        /// Show line text immediately
        /// </summary>
        private void ShowLineImmediately()
        {
            if (isShowingLine)
            {
                Debug.Log(DateTime.Now.ToString() + "准备跳过");
                if (null != currentTextShowCoroutine)
                {
                    StopCoroutine(currentTextShowCoroutine);
                    isShowingLine = false;
                }
            }
            AddHistoryText(nextLine); // Add line to history text list
            ShowLineImmediately(nextLine);
            Debug.Log(DateTime.Now.ToString() + "已跳过");
            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!SettingModel.isSkipModeOn && SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed())
            {
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Show line text immediately
        /// </summary>
        /// <param name="newLine">A new full-text line</param>
        private void ShowLineImmediately(string newLine)
        {
            line.text = newLine;

            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!SettingModel.isSkipModeOn && SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed())
            {
                Debug.Log("1: historyField.activeSelf=" + historyField.activeSelf);
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Show line text with duration <see cref="waitForSeconds" />
        /// </summary>
        /// <param name="waitForSeconds"></param>
        private IEnumerator SwitchLineTimeout(WaitForSeconds waitForSeconds)
        {
            yield return waitForSeconds;
            if (!isShowingLine && IsSwitchLineAllowed())
            {
                SwitchLine();
            }
        }

        /// <summary>
        /// Show line text with duration <see cref="lineSwitchDuration"/>
        /// </summary>
        private IEnumerator SwitchLineTimeout()
        {
            yield return lineSwitchWaitForSeconds;
            if (!SettingModel.isSkipModeOn && !isShowingLine && IsSwitchLineAllowed())
            {
                SwitchLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex">The index of page, total number of page will be `savdDataPageCount`.</param>
        /// <param name="number">The page size, default: 12.</param>
        /// <returns></returns>
        private List<Button> SetSavedDataModelButtons(int pageIndex, int number = 12)
        {
            int savedDataPageNumbers = savedDataPanel.transform.childCount;
            List<Button> pageButtons;

            if (savedDataPageNumbers >= pageIndex + 1)
            {
                // Saved data buttons are already initialized, load from cache
                pageButtons = savedDataButtons[pageIndex];
                return pageButtons;
            }
            else
            {
                // No saved data buttons in this page yet, initial they
                pageButtons = InitSavedDataButton(pageIndex, number);
                savedDataButtons[lastLoadedSavedDataPage] = pageButtons;
                return pageButtons;
            }
        }

        /// <summary>
        /// SavedData show page controller, set page at <paramref name="activeIndex"/> to be actived
        /// </summary>
        /// <param name="activeIndex">active page</param>
        private void SetActiveSavedDataPanel(int activeIndex)
        {
            Transform pre = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            pre.gameObject.SetActive(false);
            Transform now = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", activeIndex));
            now.gameObject.SetActive(true);
            lastLoadedSavedDataPage = activeIndex;
        }

        /// <summary>
        /// Init saved data button in SavedDataPage
        /// </summary>
        /// <param name="pageIndex">Index of SavedDataPage, 0 as start</param>
        /// <param name="number">Button number per page, default: 12</param>
        /// <returns></returns>
        internal List<Button> InitSavedDataButton(int pageIndex, int number = 12)
        {
            List<Button> currentSaveDataList = new List<Button>();
            GameObject gameObject = new GameObject(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            Grid savedDataGrid = gameObject.AddComponent<Grid>();
            GridLayoutGroup savedDataGroup = gameObject.AddComponent<GridLayoutGroup>();
            savedDataGroup.cellSize = new Vector2(200.0f, 120.0f);
            savedDataGroup.spacing = Vector2.one;

            for (int i = 0; i < number; i++)
            {
                Button newEmptySaveDataModel = Instantiate(saveDataModelPrefab);
                Button.ButtonClickedEvent saveDataClickEvent = new Button.ButtonClickedEvent();
                int savedDataIndex = pageIndex * number + i;
                saveDataClickEvent.AddListener(delegate ()
                {
                    // Click Callback
                    if (isSavingGameData)
                    {
                        // Save game data

                        // TODO: Test it
                        // Save current status of game.
                        if (null == savedDatas[savedDataIndex])
                        {
                            savedDatas[savedDataIndex] = new SavedDataModel()
                            {
                                savedDataIndex = savedDataIndex,
                                savedTime = DateTime.Now,
                                galgameActionIndex = currentLineIndex,
                                gameValues = GlobalGameData.GameValues
                            };
                        }
                        else
                        {
                            savedDatas[savedDataIndex].savedDataIndex = savedDataIndex;
                            savedDatas[savedDataIndex].savedTime = DateTime.Now;
                            savedDatas[savedDataIndex].galgameActionIndex = currentLineIndex;
                            savedDatas[savedDataIndex].gameValues = GlobalGameData.GameValues;
                        }
                        // TODO: Considering doing this when application exit to avoid unnecessary IO operations?
                        // Persist saved data
                        gameController.PersistSavedDatas();
                        // Renew saved data display field
                        RenewSavedDataField(newEmptySaveDataModel, savedDatas[savedDataIndex]);
                    }
                    if (isLoadingSavedData)
                    {
                        // Load saved game data
                        SavedDataModel theSavedData = gameController.LoadSavedData(savedDataIndex);
                        if (null == theSavedData) return;

                        // TODO: Test it
                        // Refresh scene via the saved data.
                        gameController.DeactiveGameObject(savedDataField);
                        isLoadingSavedData = false;
                        SetCurrentGalgameAction(theSavedData);
                    }
                });
                newEmptySaveDataModel.onClick = saveDataClickEvent;
                newEmptySaveDataModel.transform.SetParent(gameObject.transform);
                newEmptySaveDataModel.name = string.Format("SaveData_{0}", i + 1);
                newEmptySaveDataModel.GetComponent<RectTransform>().localScale = Vector3.one;
                // Set display data
                if (null != savedDatas[savedDataIndex])
                {
                    RenewSavedDataField(newEmptySaveDataModel, savedDatas[savedDataIndex]);
                }
                currentSaveDataList.Add(newEmptySaveDataModel);
            }
            // Append saved data list to `savedDataPanel`
            gameObject.transform.SetParent(savedDataPanel.transform);
            gameObject.transform.position = savedDataPanel.transform.position;
            gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            gameObject.GetComponent<RectTransform>().sizeDelta = savedDataPanel.GetComponent<RectTransform>().sizeDelta;
            return currentSaveDataList;
        }

        internal void RenewSavedDataField(Button b, SavedDataModel sdm)
        {
            Text t = b.gameObject.transform.GetChild(0).GetComponent<Text>();
            // TODO: Make decision of what should be renew. Background, display text for example.
            t.text = string.Format("Saved Date: {0}\nSaved Time: {1}", sdm.savedTime.ToString("yyyy/MM/dd"), sdm.savedTime.ToString("hh:mm:ss"));
        }

        /// <summary>
        /// Set current galgame action.
        /// TODO: The SavedDataModel class will be modeified. The method must to be adjusted.
        /// </summary>
        /// <param name="theSavedData"></param>
        internal void SetCurrentGalgameAction(SavedDataModel theSavedData)
        {
            currentLineIndex = theSavedData.galgameActionIndex;
            currentGalgameAction = galgameActions[currentLineIndex];
            bgSpriteRenderer.sprite = currentGalgameAction.Background;
            nextLine = currentGalgameAction.Line.text.Replace("\\n","\n");
            GlobalGameData.GameValues = theSavedData.gameValues;
            line.text = string.Empty;
        }

        /// <summary>
        /// Add line to history
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool AddHistoryText(string text)
        {
            Text newHistoryText = Instantiate(historyTextPrefab);
            newHistoryText.text = text;
            Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(delegate ()
            {
                SetCurrentActiveHistoryText(newHistoryText);
            });
            newHistoryText.transform.GetComponent<Button>().onClick = buttonClickedEvent;
            newHistoryText.transform.SetParent(historyTexts.transform);
            newHistoryText.GetComponent<RectTransform>().localScale = Vector3.one;
            SetCurrentActiveHistoryText(newHistoryText);
            return true;
        }

        /// <summary>
        /// Get the UI object of current mouse pointer hover on.
        /// </summary>
        /// <param name="canvas">The specific canvas</param>
        /// <returns></returns>
        private GameObject GetMouseOverUIObject(GameObject canvas)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (results.Count != 0)
            {
                return results[0].gameObject;
            }

            return null;
        }

        /// <summary>
        /// Wait for seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private IEnumerator WaitForSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Wait for seconds
        /// </summary>
        /// <param name="waitForSeconds"></param>
        /// <returns></returns>
        private IEnumerator WaitForSeconds(WaitForSeconds waitForSeconds)
        {
            yield return waitForSeconds;
        }
        #endregion
    }
}