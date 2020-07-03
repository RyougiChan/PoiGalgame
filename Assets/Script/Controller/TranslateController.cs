using Assets.Script.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TranslateController : MonoBehaviour {

    static string path;
    InputField input;
    Button translateBtn;

	// Use this for initialization
	void Start () {
        path = Application.dataPath + "/Resources/Chapter.ks/";
        input = GameObject.Find("DisplayCanvas/MenuField/InputField").GetComponent<InputField>();
        translateBtn = GameObject.Find("DisplayCanvas/MenuField/Ks2ScriptBtn").GetComponent<Button>();
        Button.ButtonClickedEvent clickedEvent = new Button.ButtonClickedEvent();
        clickedEvent.AddListener(() => {
            string fileName = input.text;
            if(!string.IsNullOrEmpty(fileName) && File.Exists(path + fileName))
            {
                Debug.Log("Start translate " + fileName);
                new GalgameScriptTranslator().KsToAsset(path + fileName);
            } else {
                Debug.Log("File no existed");
            }
        });
        translateBtn.onClick = clickedEvent;
        Debug.Log("translateBtn: " + translateBtn.name);
        Debug.Log("path: " + path);
    }
}
