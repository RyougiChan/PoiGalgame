using Assets.Script.Model;
using Assets.Script.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour
{

    #region Fields
    public Text line;   // Script line showing text
    public Font font;

    #region Game objects
    public GameObject mainCanvas;
    #region History gameobjects
    public GameObject historyField;
    public GameObject historyTexts;
    public Text historyTextPrefab;
    private Text currentActiveHistoryText;
    #endregion

    #region Saved data field
    public GameObject savedDataField;
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


    // Current displaying script
    public GalgameScript currentScript;
    public SpriteRenderer spriteRenderer;
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
    // Is menu is actived
    private bool isMenuActive;
    // Next line to display
    private string nextLine;

    private Coroutine currentTextShowCoroutine;
    private Coroutine currentLineSwitchCoroutine;
    private Coroutine hidehistoryFieldCoroutine;
    private static WaitForSeconds textShowWaitForSeconds;
    private static WaitForSeconds lineSwitchWaitForSeconds;
    private static WaitForSeconds skipModeLineSwitchWaitForSeconds;
    #endregion

    // Use this for initialization
    void Start()
    {
        // Init line
        // Init currentScript, galgameActions, currentLineIndex
        // currentLineIndex = 0; // ?
        isAutoReadingModeOn = false;
        galgameActions = currentScript.GalgameActions;
        if (null != currentScript.Bg)
        {
            spriteRenderer.sprite = currentScript.Bg;
        }
        textShowWaitForSeconds = new WaitForSeconds(textShowDuration);
        lineSwitchWaitForSeconds = new WaitForSeconds(lineSwitchDuration);
        skipModeLineSwitchWaitForSeconds = new WaitForSeconds(skipModeLineSwitchDuration);
    }

    // Update is called once per frame
    void Update()
    {
        // mosue left button click
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject hitUIObject = null;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                hitUIObject = GetMouseOverUIObject(mainCanvas);
                Debug.Log("---- EventSystem.current.IsPointerOverGameObject ----" + GetMouseOverUIObject(mainCanvas).tag);
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
                        ActiveGameObject(savedDataField);
                        break;
                    case "Load":
                        // Change text style
                        if (null == loadButton) loadButton = hitUIObject;
                        ActiveGameObject(savedDataField);
                        break;
                    case "Setting":
                        // Change text style
                        if (null == settingButton) settingButton = hitUIObject;
                        break;
                }
            }

            if (currentLineIndex <= galgameActions.Count && IsSwitchLineAllowed())
            {
                if (null == hitUIObject || (null != hitUIObject && hitUIObject.tag.Trim() != "OperationButton"))
                {
                    SwitchLine();
                }
            }
        }

        // mouse scroll up
        if (Input.mouseScrollDelta.y > 0)
        {
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
        ShowLineImmediately();
    }

    /// <summary>
    /// Hide history TextView
    /// </summary>
    public void HidehistoryField()
    {
        hidehistoryFieldCoroutine = StartCoroutine(HidehistoryFieldTimeOut());
    }

    /// <summary>
    /// Auto Reading
    /// </summary>
    public void AutoReading()
    {
        isAutoReadingModeOn = isAutoReadingModeOn ? false : true;
        // If isAutoReadingModeOn == true, call SwitchLine()
        if (isAutoReadingModeOn && IsSwitchLineAllowed() && !isShowingLine)
        {
            currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
        }
    }

    /// <summary>
    /// Enable/Disable skip mode
    /// </summary>
    public void ChangeSkipMode()
    {
        isSkipModeOn = isSkipModeOn ? false : true;
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
        Debug.Log(string.Format("Load Saved Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
    }

    /// <summary>
    /// System configuration
    /// </summary>
    public void ChangeSetting()
    {
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

    #endregion

    #region Private methods
    /// <summary>
    /// Whether switch line operation is allow or not
    /// </summary>
    /// <returns></returns>
    private bool IsSwitchLineAllowed()
    {
        return !isSkipModeOn && !isMenuActive && !historyField.activeSelf && !savedDataField.activeSelf && !settingField.activeSelf;
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
    private IEnumerator HidehistoryFieldTimeOut()
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
        if (isAutoReadingModeOn && IsSwitchLineAllowed())
        {
            currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
        }

        if (isSkipModeOn)
        {
            // ShowLineImmediately();
            Debug.Log("StartCoroutine(SwitchLineTimeout(skipModeLineSwitchWaitForSeconds))");
            StartCoroutine(SwitchLineTimeout(skipModeLineSwitchWaitForSeconds));
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
        if (isAutoReadingModeOn && IsSwitchLineAllowed())
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
        if (!isShowingLine && IsSwitchLineAllowed())
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
        SetCurrentActiveHistoryText(newHistoryText);
        Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
        buttonClickedEvent.AddListener(delegate ()
        {
            SetCurrentActiveHistoryText(newHistoryText);
        });
        newHistoryText.transform.GetComponent<Button>().onClick = buttonClickedEvent;
        newHistoryText.transform.SetParent(historyTexts.transform);
        return true;
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
    #endregion
}
