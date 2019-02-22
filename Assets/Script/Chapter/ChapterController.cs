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

namespace Assets.Script.Chapter
{
    public class ChapterController : MonoBehaviour
    {

        #region Fields
        private static string rootPath;
        private static string savedDataPath;
        private static string savedDataFile;

        public GameObject title;
        public GameObject lineContainer;
        public Text actorName;
        public Text line;   // Script line showing text
        public Font font;

        #region Game objects
        public GameObject mainCanvas;

        #region Prefabs
        public Text historyTextPrefab;
        public Button saveDataModelPrefab;
        #endregion

        #region History gameobjects
        public GameObject historyField;
        public GameObject historyTexts;
        private Text currentActiveHistoryText;
        #endregion

        #region Saved data field
        public GameObject savedDataField;
        public GameObject savedDataPanel;
        public static List<List<Button>> savedDataButtons;
        public static List<SavedDataModel> savedDatas;
        public static int savdDataPageCount;
        // Last loaded savedDataPage
        public static int lastLoadedSavedDataPage;
        #endregion

        #region Setting field
        public GameObject settingField;
        private Dropdown resolutionDropdown;
        #endregion

        #region Operation buttons
        public GameObject operationButtons;
        public GameObject autoPlayButton;
        public GameObject skipButton;
        public GameObject saveButton;
        public GameObject loadButton;
        public GameObject settingButton;
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
        // Current showing line's index in `currentScript`
        private int currentLineIndex;
        // Current showing line text's index of char
        private int currentLineCharIndex;
        // Is a line text is showing
        private bool isShowingLine;
        // Is auto-reading mode is actived
        // private bool isAutoReadingModeOn;
        // Is skip mode is actived
        // private bool isSkipModeOn;
        // The previous skip time
        private DateTime preSkipTime;
        // Is menu is actived
        private bool isMenuActive;
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

            // Init line
            // Init currentScript, galgameActions, currentLineIndex
            // currentLineIndex = 0; // ?
            line = lineContainer.transform.Find("Line").GetComponent<Text>();
            actorName = lineContainer.transform.Find("ActorName").GetComponent<Text>();
            // SettingModel.isAutoReadingModeOn = false;
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

            // resolution dropdown list
            resolutionDropdown = settingField.transform.Find("ScreenMode").Find("Windowed").Find("Resolution").GetComponent<Dropdown>();
            foreach(Resolution resolution in Screen.resolutions)
            {
                resolutionDropdown.options.Add(new Dropdown.OptionData(string.Format("{0}x{1}", resolution.width, resolution.height)));
            }
            resolutionDropdown.RefreshShownValue();

            // savedDatas = gameController.LoadSavedDatas();
            lastLoadedSavedDataPage = GameController.lastLoadedSavedDataPage;
            savdDataPageCount = GameController.savdDataPageCount;
            savedDatas = GameController.savedDatas;
            savedDataButtons = gameController.InitList<List<Button>>(savdDataPageCount);

            InitSceneGameObject();
        }

        // Update is called once per frame
        void Update()
        {
            // mosue left button click
            if (Input.GetButtonDown("Fire1"))
            {

                if (title.activeSelf)
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
                            lastLoadedSavedDataPage = 0;
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
            if (Input.mouseScrollDelta.y > 0 && !historyField.activeSelf)
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
            gameController.ActiveGameObject(historyField);
            // If `currentTextShowCoroutine` is going
            if (isShowingLine)
            {
                ShowLineImmediately();
            }
        }

        /// <summary>
        /// Hide history TextView
        /// </summary>
        public void HidehistoryField()
        {
            hidehistoryFieldCoroutine = StartCoroutine(HideHistoryFieldTimeOut());
        }

        /// <summary>
        /// Set reading mode to manual
        /// </summary>
        /// <param name="manual"></param>
        public void SetManualMode(bool manual)
        {
            if(manual)
            {
                SettingModel.isAutoReadingModeOn = false;
                SettingModel.isSkipModeOn = false;
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
            SettingModel.isSkipModeOn = false;
            skipButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            SettingModel.isAutoReadingModeOn = auto;
            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if(!auto)
            {
                autoPlayButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            }
            if (SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed() && !SettingModel.isSkipModeOn && !isShowingLine)
            {
                autoPlayButton.transform.GetChild(0).GetComponent<Text>().color = Color.cyan;
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
            SettingModel.isAutoReadingModeOn = false;
            autoPlayButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            SettingModel.isSkipModeOn = skip;
            if(!skip)
            {
                skipButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            }
            if (SettingModel.isSkipModeOn && IsSwitchLineAllowed())
            {
                skipButton.transform.GetChild(0).GetComponent<Text>().color = Color.cyan;
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
            gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
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
            gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
            Debug.Log(string.Format("Load Saved Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        public void SwitchSavedDataPage(int step)
        {
            // If out of range, do nothing
            if (lastLoadedSavedDataPage + step < 0 || lastLoadedSavedDataPage + step >= savdDataPageCount) return;
            // Hide previous display saved data page
            Transform target = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != target)
            {
                gameController.DeactiveGameObject(target.gameObject);
            }

            lastLoadedSavedDataPage += step;
            Transform nTarget = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != nTarget)
            {
                gameController.ActiveGameObject(nTarget.gameObject);
            }
            // Set now display saved data page
            SetSavedDataModelButtons(lastLoadedSavedDataPage);
        }

        /// <summary>
        /// System configuration
        /// </summary>
        public void ChangeSetting()
        {
            gameController.ShowCG();
            gameController.ActiveGameObject(settingField);
            Debug.Log(string.Format("Change Setting"));
        }
        #endregion

        #region Private methods

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
            return !isMenuActive && !historyField.activeSelf && !savedDataField.activeSelf && !settingField.activeSelf;
        }

        /// <summary>
        /// Hide history TextView with duration
        /// </summary>
        private IEnumerator HideHistoryFieldTimeOut()
        {
            if (historyField.activeSelf)
            {
                gameController.DeactiveGameObject(historyField);
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
            if (currentLineIndex == galgameActions.Count)
            {
                // this chapter is end
                if (SettingModel.isSkipModeOn)
                {
                    SetSkipMode(false);
                }
                if(SettingModel.isAutoReadingModeOn)
                {
                    SetAutoMode(false);
                }
                // TODO: maybe consider loading another chapter?
                line.text = "『つづく...』";
                return;
            }

            Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
            currentLineCharIndex = -1; // read from index: -1
            currentGalgameAction = galgameActions[currentLineIndex];
            nextLine = currentGalgameAction.Line.text;
            if (SettingModel.isSkipModeOn)
            {
                ShowLineImmediately();
            }
            else
            {
                currentTextShowCoroutine = StartCoroutine(ShowLineTimeOut(nextLine));
            }
            if (null != currentGalgameAction.Bgm)
            {
                // bgm
                _bgmAudio.clip = currentGalgameAction.Bgm;
                _bgmAudio.Play();
            }
            if (null != currentGalgameAction.Voice)
            {
                // voice
                _voiceAudio.clip = currentGalgameAction.Voice;
                _voiceAudio.Play();
            }
            if (currentGalgameAction.Actor != Actor.NULL)
            {
                // actor's name
                actorName.text = currentGalgameAction.Actor.ToString();
            }
            if (null != currentGalgameAction.Background)
            {
                // current background
                bgSpriteRenderer.sprite = currentGalgameAction.Background;
            }
            // text-align
            line.alignment = EnumMap.AlignToTextAnchor(currentGalgameAction.Line.align);

            // font
            if (null != font)
            {
                line.font = font;
            }
            // font-style
            if (currentGalgameAction.Line.fstyle == FontStyle.Normal)
            {
                line.fontStyle = DefaultScriptProperty.fstyle;
            }
            else
            {
                line.fontStyle = currentGalgameAction.Line.fstyle;
            }
            // font-size
            if (currentGalgameAction.Line.fsize != 0)
            {
                line.fontSize = Mathf.RoundToInt(currentGalgameAction.Line.fsize);
            }
            else if (DefaultScriptProperty.fsize != 0)
            {
                line.fontSize = Mathf.RoundToInt(DefaultScriptProperty.fsize);
            }
            // line-spacing
            if (currentGalgameAction.Line.linespacing != 0)
            {
                line.lineSpacing = currentGalgameAction.Line.linespacing;
            }
            else if (DefaultScriptProperty.linespacing != 0)
            {
                line.lineSpacing = DefaultScriptProperty.linespacing;
            }
            // font-color
            if (!string.IsNullOrEmpty(currentGalgameAction.Line.fcolor))
            {
                line.color = ColorUtil.HexToUnityColor(uint.Parse(currentGalgameAction.Line.fcolor, System.Globalization.NumberStyles.HexNumber));
            }
            else if (!string.IsNullOrEmpty(DefaultScriptProperty.fcolor))
            {
                line.color = ColorUtil.HexToUnityColor(uint.Parse(DefaultScriptProperty.fcolor, System.Globalization.NumberStyles.HexNumber));
            }
            // Move index to next
            currentLineIndex++;
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

            if (SettingModel.isSkipModeOn)
            {
                // ShowLineImmediately();
                Debug.Log("StartCoroutine(SwitchLineTimeout(skipModeLineSwitchWaitForSeconds))");
                // StartCoroutine(SwitchLineTimeout(skipModeLineSwitchWaitForSeconds));
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
                                galgameActionIndex = currentLineIndex
                            };
                        }
                        else
                        {
                            savedDatas[savedDataIndex].savedDataIndex = savedDataIndex;
                            savedDatas[savedDataIndex].savedTime = DateTime.Now;
                            savedDatas[savedDataIndex].galgameActionIndex = currentLineIndex;
                        }
                        // TODO: Considering doing this then application exit to avoid unnecessary IO operations?
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

        internal void RenewSavedDataPage(List<Button> button)
        {

        }

        internal void RenewSavedDataField(Button b, SavedDataModel sdm)
        {
            Text t = b.gameObject.transform.GetChild(0).GetComponent<Text>();
            // TODO: Make decision of what show be renew. Background, display text for example.
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
            nextLine = currentGalgameAction.Line.text;
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