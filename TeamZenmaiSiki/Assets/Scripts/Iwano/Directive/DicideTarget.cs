﻿using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class DicideTarget : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        int temp = DataManager.Instance.ScenarioChapterNumber;
        int temp2 = DataManager.Instance.ScenarioSectionNumber;
        
        GetComponent<Text>().text = DataManager.Instance.DirectiveDatas[temp][temp2].missionObjective;

        //GetComponent<Text>().text = "β版完成";
    }
}
