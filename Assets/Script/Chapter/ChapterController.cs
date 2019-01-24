using Assets.Script.Model;
using Assets.Script.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour {

    public Text line;  // Script line showing text
    public Font font;

    // History gameobject start
    public GameObject historyTextView;
    public GameObject historyTexts;
    public Text historyTextPrefab;
    
    private Text currentActiveHistoryText;
    // History gameobject end

    public GalgameScript currentScript;
    public SpriteRenderer spriteRenderer;
    public float textShowDuration = 0.1f;

    private List<GalgameAction> galgameActions;
    private GalgameAction currentGalgameAction;
    private int currentLineIndex;
    private int currentLineCharIndex;
    private bool isShowingLine;
    private bool isBreakShowingLine;
    private string newLine;
    private Coroutine currentCoroutine;

    private static WaitForSeconds textWaitForSeconds;

    // Use this for initialization
    void Start () {
        // Init line
        // Init currentScript, galgameActions, currentLineIndex
        // currentLineIndex = 0; // ?
        galgameActions = currentScript.GalgameActions;
        if(null != currentScript.Bg)
        {
            spriteRenderer.sprite = currentScript.Bg;
        }
        textWaitForSeconds = new WaitForSeconds(textShowDuration);
    }

    // Update is called once per frame
    void Update () {
        // mosue left button click
        if (Input.GetButtonDown("Fire1"))
        {
            if(currentLineIndex <= galgameActions.Count && !historyTextView.activeSelf)
            {
                SwitchLine();
            }
        }

        // mouse scroll up
        if(Input.mouseScrollDelta.y > 0)
        {
            ActiveGameObject(historyTextView);
            ShowLineImmediately();
        }
	}

    /// <summary>
    /// Hide history TextView
    /// </summary>
    public void HideHistoryTextView()
    {
        if(historyTextView.activeSelf) DeactiveGameObject(historyTextView);
    }

    /// <summary>
    /// Show new line controller
    /// </summary>
    private void SwitchLine()
    {
        Debug.Log(DateTime.Now.ToString() + " --> isShowingLine = " + isShowingLine);
        line.text = string.Empty;
        if (isShowingLine)
        {
            Debug.Log(DateTime.Now.ToString() + "准备跳过");
            if (null != currentCoroutine)
            {
                StopCoroutine(currentCoroutine);
                ShowLineImmediately(newLine);
                AddHistoryText(newLine); // Add line to history text list
                isShowingLine = false;
                Debug.Log(DateTime.Now.ToString() + "已跳过");
                return;
            }
        }
        if (currentLineIndex == galgameActions.Count)
        {
            // this chapter is end
            // TODO: maybe consider loading another chapter?
            return;
        }

        Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
        line.text = string.Empty; // clear previous line
        currentLineCharIndex = -1; // read from index: -1
        currentGalgameAction = galgameActions[currentLineIndex];
        newLine = currentGalgameAction.Line.text;
        currentCoroutine = StartCoroutine(ShowLineTimeOut(newLine));
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
    public void SetCurrentActiveHistoryText(Text nextActiveHistoryText)
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
    /// <returns></returns>
    private IEnumerator ShowLineTimeOut(string newLine)
    {
        isShowingLine = true;
        foreach (char lineChar in newLine)
        {
            currentLineCharIndex++;
            line.text += lineChar;
            if(currentLineCharIndex == newLine.Length - 1)
            {
                isShowingLine = false;
                AddHistoryText(newLine); // Add line to history text list
                Debug.Log("currentLineCharIndex: " + currentLineCharIndex);
            }
            yield return textWaitForSeconds;
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
            if (null != currentCoroutine)
            {
                StopCoroutine(currentCoroutine);
                ShowLineImmediately(newLine);
                AddHistoryText(newLine); // Add line to history text list
                isShowingLine = false;
                Debug.Log(DateTime.Now.ToString() + "已跳过");
            }
        }
    }

    /// <summary>
    /// Show line text immediately
    /// </summary>
    /// <param name="newLine">A new full-text line</param>
    private void ShowLineImmediately(string newLine)
    {
        line.text = newLine;
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
        buttonClickedEvent.AddListener(delegate() {
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
    private void ActiveGameObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// To deactive a gameobject
    /// </summary>
    /// <param name="gameObject">The target object</param>
    private void DeactiveGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
