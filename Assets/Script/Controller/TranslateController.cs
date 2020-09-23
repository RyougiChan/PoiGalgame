#if UNITY_EDITOR
using Assets.Script.Model.Datas;
using Assets.Script.Utility;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TranslateController : MonoBehaviour {

    static string path;
    InputField input;
    Button translateBtn;

	// Use this for initialization
	void Start () {

        GlobalGameData.GameValues = new GameValues();

        path = Application.dataPath + "/Resources/Chapter.ks/";
        input = GameObject.Find("DisplayCanvas/MenuField/InputField").GetComponent<InputField>();
        translateBtn = GameObject.Find("DisplayCanvas/MenuField/Ks2ScriptBtn").GetComponent<Button>();
        Button.ButtonClickedEvent clickedEvent = new Button.ButtonClickedEvent();
        clickedEvent.AddListener(() => {
            string fileName = input.text;
            if(!string.IsNullOrEmpty(fileName) && File.Exists(path + fileName))
            {
                Debug.Log("Start translate " + fileName);
                GalgameScriptTranslator.KsToAsset(path + fileName);
            } else {
                Debug.Log("File no existed");
            }
        });
        translateBtn.onClick = clickedEvent;
    }

    public void TranslateAllInPath(string s)
    {
        if(string.IsNullOrEmpty(s))
        {
            s = path;
        }
        string[] files = Directory.GetFiles(path);
        foreach(string f in files)
        {
            // Translate .ks file only
            if(f.EndsWith(".ks"))
            {
                GalgameScriptTranslator.KsToAsset(f);
                Debug.Log("Translate Success: " + f);
            }
        }
    }
}
#endif