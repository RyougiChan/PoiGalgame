using Assets.Script.Model.Datas;
using Assets.Script.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour {

    // All progress container containing Name/Value and progress bar
    public Dictionary<string, Transform> namedProgressList;
    private RoleAbility ability;

    // Use this for initialization
    void Start () {
        ability = GlobalGameData.GameValues.RoleAbility;
        namedProgressList = new Dictionary<string, Transform>();
        int progressCount = gameObject.transform.childCount;
        for(int i = 0; i < progressCount;i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            namedProgressList.Add(child.name, child);
        }

        if(null != ability)
        {
            // Set existing values
            foreach (KeyValuePair<string, Transform> progress in namedProgressList)
            {
                string propName = progress.Key;
                Transform p = progress.Value;
                int existed = EntityUtil.GetValue<int>(ability, propName);
                p.Find("Value").GetComponent<Text>().text = existed.ToString();
                p.Find("ProgressBar").GetComponent<Slider>().value = (float)existed / 100.0f;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        // Set existing values
        if (null != ability)
        {
            // Set existing values
            foreach (KeyValuePair<string, Transform> progress in namedProgressList)
            {
                string propName = progress.Key;
                Transform p = progress.Value;
                int existed = EntityUtil.GetValue<Int32>(ability, propName);
                p.Find("Value").GetComponent<Text>().text = existed.ToString();
                p.Find("ProgressBar").GetComponent<Slider>().value = (float)existed / 100.0f;
            }
        }
    }
}
