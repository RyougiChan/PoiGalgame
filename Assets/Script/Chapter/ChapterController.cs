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
    public int savdDataPageCount;
    #endregion

    #region Setting field
    public GameObject settingField;
    #endregion

    #region Operation buttons
    public GameObject autoPlayButon;
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
    // Setting: Duration of text showing speed, in seconds
    public float textShowDuration = 0.1f;
    // Setting: Duration of line auto switch speed, in seconds
    public float lineSwitchDuration = 3.0f;
    // Duration of line switch speed under skip mode, in seconds
    private float skipModeLineSwitchDuration = 0.1f;

    private List<GalgameAction> galgameActions;
    private GalgameAction currentGalgameAction;
    // Current showing line's index in `currentScript`
    private int currentLineIndex;
    // Current showing line text's index of char
    private int currentLineCharIndex;
    // Is a line text is showing
    private bool isShowingLine;
    // Is auto-reading mode is actived
    private bool isAutoReadingModeOn;
    // Is skip mode is actived
    private bool isSkipModeOn;
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
    // Last loaded savedDataPage
    private int lastLoadedSavedDataPage;

    private Coroutine currentTextShowCoroutine;
    private Coroutine currentLineSwitchCoroutine;
    private Coroutine hidehistoryFieldCoroutine;
    private static WaitForSeconds textShowWaitForSeconds;
    private static WaitForSeconds lineSwitchWaitForSeconds;
    private static WaitForSeconds skipModeLineSwitchWaitForSeconds;
    private static List<SavedDataModel> savedDatas;
    private static List<List<Button>> savedDataButtons;
    private VideoPlayer _video;
    private AudioSource _audio;
    private AudioSource _voiceAudio;
    private AudioClip bgmMusic;
    #endregion

    // Use this for initialization
    void Start()
    {
        rootPath = Application.dataPath;
        savedDataPath = rootPath + "/Resources/SavedData/";
        savedDataFile = "savedata.dat";
        // Init line
        // Init currentScript, galgameActions, currentLineIndex
        // currentLineIndex = 0; // ?
        line = lineContainer.transform.Find("Line").GetComponent<Text>();
        actorName = lineContainer.transform.Find("ActorName").GetComponent<Text>();
        lastLoadedSavedDataPage = 0;
        savdDataPageCount = 10;
        isAutoReadingModeOn = false;
        galgameActions = currentScript.GalgameActions;
        if (null != currentScript.Bg)
        {
            bgSpriteRenderer.sprite = currentScript.Bg;
        }
        textShowWaitForSeconds = new WaitForSeconds(textShowDuration);
        lineSwitchWaitForSeconds = new WaitForSeconds(lineSwitchDuration);
        skipModeLineSwitchWaitForSeconds = new WaitForSeconds(skipModeLineSwitchDuration);

        _video = videoPlayer.GetComponent<VideoPlayer>();
        _audio = audioSource.GetComponent<AudioSource>();
        _voiceAudio = voiceAudioSource.GetComponent<AudioSource>();

        savedDatas = LoadSavedDatas();

        savedDataButtons = InitList<List<Button>>(savdDataPageCount);

        InitSceneGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        // mosue left button click
        if (Input.GetButtonDown("Fire1"))
        {

            if(title.activeSelf)
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
                    // Click on UI button named 'AutoPlayBtn'
                    case "AutoPlay":
                        // Change text style
                        Debug.Log("isAutoReadingModeOn: " + isAutoReadingModeOn);
                        if (null == autoPlayButon) autoPlayButon = hitUIObject;
                        hitUIObject.GetComponent<Text>().color = isAutoReadingModeOn ? Color.black : Color.cyan;
                        break;
                    case "Skip":
                        // Change text style
                        Debug.Log("isSkipModeOn: " + isSkipModeOn);
                        if (null == skipButton) skipButton = hitUIObject;
                        hitUIObject.GetComponent<Text>().color = isSkipModeOn ? Color.black : Color.cyan;
                        break;
                    case "Save":
                        if (null == saveButton) saveButton = hitUIObject;
                        break;
                    case "Load":
                        if (null == loadButton) loadButton = hitUIObject;
                        break;
                    case "Setting":
                        if (null == settingButton) settingButton = hitUIObject;
                        break;
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
                    lineContainer.SetActive(true);
                }
                if (null == hitUIObject || (null != hitUIObject && hitUIObject.tag.Trim() != "OperationButton"))
                {
                    SwitchLine();
                }
            }
        }

        if (isSkipModeOn && (null == preSkipTime || (DateTime.Now - preSkipTime).TotalSeconds > skipModeLineSwitchDuration))
        {
            if (!lineContainer.activeSelf)
            {
                lineContainer.SetActive(true);
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
        ActiveGameObject(historyField);
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
    /// Auto Reading
    /// </summary>
    public void AutoReading()
    {
        isAutoReadingModeOn = !isAutoReadingModeOn;
        // If isAutoReadingModeOn == true, call SwitchLine()
        if (isAutoReadingModeOn && IsSwitchLineAllowed() && !isSkipModeOn && !isShowingLine)
        {
            currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
        }
    }

    /// <summary>
    /// Enable/Disable skip mode
    /// </summary>
    public void ChangeSkipMode()
    {
        isSkipModeOn = !isSkipModeOn;
        if(isSkipModeOn)
        {
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
        ShowCG();
        ActiveGameObject(savedDataField);
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
        ShowCG();
        ActiveGameObject(savedDataField);
        Debug.Log(string.Format("Load Saved Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
    }

    public void SwitchSavedDataPage(int step)
    {
        // If out of range, do nothing
        if (lastLoadedSavedDataPage + step < 0 || lastLoadedSavedDataPage + step >= savdDataPageCount) return;
        // Hide previous display saved data page
        Transform target = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
        if(null != target)
        {
            DeactiveGameObject(target.gameObject);
        }

        lastLoadedSavedDataPage += step;
        Transform nTarget = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
        if (null != nTarget)
        {
            ActiveGameObject(nTarget.gameObject);
        }
        // Set now display saved data page
        SetSavedDataModelButtons(lastLoadedSavedDataPage);
    }

    /// <summary>
    /// System configuration
    /// </summary>
    public void ChangeSetting()
    {
        ShowCG();
        ActiveGameObject(settingField);
        Debug.Log(string.Format("Change Setting"));
    }

    /// <summary>
    /// Request full screen mode
    /// </summary>
    public void FullScreen()
    {
        
    }

    /// <summary>
    /// Return to title screen
    /// </summary>
    public void ReturnToTitleScreen()
    {

    }

    /// <summary>
    /// Hide text display panel
    /// </summary>
    public void HideMessage()
    {

    }

    /// <summary>
    /// Close game
    /// </summary>
    public void CloseGame()
    {
        
    }

    public void ShowCG()
    {
        lineContainer.SetActive(false);
        historyField.SetActive(false);
        savedDataField.SetActive(false);
        settingField.SetActive(false);
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
    /// Disable skip mode
    /// </summary>
    private void DisableSkipMode()
    {
        isSkipModeOn = false;
        skipButton.GetComponent<Text>().color = Color.black;
    }

    /// <summary>
    /// Hide history TextView with duration
    /// </summary>
    private IEnumerator HideHistoryFieldTimeOut()
    {
        if (historyField.activeSelf)
        {
            DeactiveGameObject(historyField);
            if (isAutoReadingModeOn)
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
        line.text = string.Empty;
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
            if(isSkipModeOn)
            {
                DisableSkipMode();
            }
            // TODO: maybe consider loading another chapter?
            line.text = "『つづく...』";
            return;
        }

        Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
        line.text = string.Empty; // clear previous line
        currentLineCharIndex = -1; // read from index: -1
        currentGalgameAction = galgameActions[currentLineIndex];
        nextLine = currentGalgameAction.Line.text;
        if (isSkipModeOn)
        {
            ShowLineImmediately();
        }
        else
        {
            currentTextShowCoroutine = StartCoroutine(ShowLineTimeOut(nextLine));
        }
        if(null != currentGalgameAction.Bgm)
        {
            // bgm
            _audio.clip = currentGalgameAction.Bgm;
            _audio.Play();
        }
        if (null != currentGalgameAction.Voice)
        {
            // voice
            _voiceAudio.clip = currentGalgameAction.Voice;
            _voiceAudio.Play();
        }
        if (currentGalgameAction.Actor != Actor.NULL) {
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
        foreach (char lineChar in newLine)
        {
            currentLineCharIndex++;
            line.text += lineChar;
            if (currentLineCharIndex == newLine.Length - 1)
            {
                isShowingLine = false;
                AddHistoryText(newLine); // Add line to history text list
                Debug.Log("currentLineCharIndex: " + currentLineCharIndex);
                if (isAutoReadingModeOn)
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
        // If isAutoReadingModeOn == true, call SwitchLine()
        if (!isSkipModeOn && isAutoReadingModeOn && IsSwitchLineAllowed())
        {
            currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
        }

        if (isSkipModeOn)
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

        // If isAutoReadingModeOn == true, call SwitchLine()
        if (!isSkipModeOn && isAutoReadingModeOn && IsSwitchLineAllowed())
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
        if (!isSkipModeOn && !isShowingLine && IsSwitchLineAllowed())
        {
            SwitchLine();
        }
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
                    if(null == savedDatas[savedDataIndex])
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
                    PersistSavedDatas();
                    // Renew saved data display field
                    RenewSavedDataField(newEmptySaveDataModel, savedDatas[savedDataIndex]);
                }
                if (isLoadingSavedData)
                {
                    // Load saved game data
                    SavedDataModel theSavedData = LoadSavedData(savedDataIndex);
                    if (null == theSavedData) return;

                    // TODO: Test it
                    // Refresh scene via the saved data.
                    DeactiveGameObject(savedDataField);
                    isLoadingSavedData = false;
                    SetCurrentGalgameAction(theSavedData);
                }
            });
            newEmptySaveDataModel.onClick = saveDataClickEvent;
            newEmptySaveDataModel.transform.SetParent(gameObject.transform);
            newEmptySaveDataModel.name = string.Format("SaveData_{0}", i + 1);
            newEmptySaveDataModel.GetComponent<RectTransform>().localScale = Vector3.one;
            // Set display data
            if(null != savedDatas[savedDataIndex])
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
    /// Save a saveddata at index of `index` and write to file [savedata.dat]
    /// </summary>
    /// <param name="savedData">This data model to be saved</param>
    /// <returns></returns>
    private bool PersistSavedDatas()
    {
        try
        {
            if(!Directory.Exists(savedDataPath))
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
    private List<SavedDataModel> LoadSavedDatas()
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
                        if(null != jSavedData && jSavedData.Type != JTokenType.Null)
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
    private SavedDataModel LoadSavedData(int index)
    {
        return savedDatas[index];
    }

    /// <summary>
    /// Initial savedDatas Array, fullfill it with empty SavedDataModel
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    [Obsolete]
    internal List<SavedDataModel> initialSavedDatas(int size)
    {
        for (int i = 0; i < size; i++)
        {
            savedDatas.Add(new SavedDataModel());
        }
        return savedDatas;
    }

    /// <summary>
    /// To active a gameobject
    /// </summary>
    /// <param name="gameObject">The target object</param>
    public void ActiveGameObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// To deactive a gameobject
    /// </summary>
    /// <param name="gameObject">The target object</param>
    public void DeactiveGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
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

    private List<T> InitList<T>(int size)
    {
        List<T> newList = new List<T>();
        if(size > 0)
        {
            for(int i = 0; i < size; i++)
            {
                newList.Add(default(T));
            }
        }
        return newList;
    }
    #endregion
}
