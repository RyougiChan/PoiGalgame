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
		if(Input.GetButtonDown("Fire1"))
        {
            if(currentLineIndex <= galgameActions.Count)
            {
                SwitchLine();
            }
        }
	}

    private void SwitchLine()
    {
        Debug.Log(DateTime.Now.ToString() + " --> isShowingLine = " + isShowingLine);
        if(isShowingLine)
        {
            Debug.Log(DateTime.Now.ToString() + "准备跳过");
            if (null != currentCoroutine)
            {
                StopCoroutine(currentCoroutine);
                line.text = newLine;
                isShowingLine = false;
                Debug.Log(DateTime.Now.ToString() + "已跳过");
                return;
            }
        }
        if (currentLineIndex == galgameActions.Count)
        {
            // this chapter is end
            // maybe consider loading another chapter?
            return;
        }

        Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
        line.text = string.Empty; // clear previous line
        currentLineCharIndex = -1; // read from index: -1
        currentGalgameAction = galgameActions[currentLineIndex];
        newLine = currentGalgameAction.Line.text;
        currentCoroutine = StartCoroutine(ShowLineTimeOut(newLine));

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
                Debug.Log("currentLineCharIndex: " + currentLineCharIndex);
            }
            yield return textWaitForSeconds;
        }
    }
}
