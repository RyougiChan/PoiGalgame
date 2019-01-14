using Assets.Script.Model;
using Assets.Script.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour {

    public Text line;  // Script line showing text
    public Font font;

    public GalgameScript currentScript;
    public SpriteRenderer spriteRenderer;

    private List<GalgameAction> galgameActions;
    private GalgameAction currentGalgameAction;
    private int currentLineIndex;

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
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetButtonDown("Fire1"))
        {
            if(currentLineIndex < galgameActions.Count)
            {
                SwitchLine();
            }
        }
	}

    private void SwitchLine()
    {
        Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
        currentGalgameAction = galgameActions[currentLineIndex];
        line.text = currentGalgameAction.Line.text;
        // font
        if(null != font)
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
}
