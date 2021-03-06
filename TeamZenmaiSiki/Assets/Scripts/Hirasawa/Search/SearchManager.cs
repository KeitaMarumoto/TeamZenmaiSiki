﻿using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SearchManager : MonoBehaviour , ISceneBase
{

	// Use this for initialization
	
    private static SearchManager instance;

    public static SearchManager Instance
    {
        get { return instance; }
    }
    private int episodeNum;
    private int stageNum;
    public int EpisodeNum()
    {
        return episodeNum;
    }
    public int StageNum()
    {
        return stageNum;
    }
    private int enemynumber;

    public int GetEnemyNum()
    {
        return enemynumber;
    }
    public void setEnemyNUm(int n)
    {
        enemynumber = n;
    }
    private float unitPos;
    public float GetUnitPos()
    {
        return unitPos;
    }
    public void SetUnitPos(float value)
    {
        unitPos = value;
    }
    private bool isUnitTouch;
    public bool GetisUnitTouch()
    {
        return isUnitTouch;
    }
    public void SetisUnitTouch(bool value)
    {
        isUnitTouch= value;
    }
    [SerializeField]
    EnemyDataPlate plate;
    private List<EnemyData.EnemyInternalDatas> sendDatas;
    private List<EnemyData.EnemyInternalDatas> batlleDataList;
    public List<EnemyData.EnemyInternalDatas> getSendEnemyDatas()
    {
        return sendDatas;
    }
    public List<EnemyData.EnemyInternalDatas> getBattleEnemyDatas()
    {
        return batlleDataList;
    }
    public void setEnemyDatas(List<EnemyData.EnemyInternalDatas> _sendDatas)
    {
        sendDatas = _sendDatas;
        List<EnemyData.EnemyInternalDatas> buf = new List<EnemyData.EnemyInternalDatas>();
        for(int i = 0; i < sendDatas.Count; i++)
        {
            if (sendDatas[i].isbattle)
            {
                buf.Add(sendDatas[i]);
            }
        }
        //batlleDataList = buf;
        DataManager.Instance.EnemyInternalDatas = buf;
    }
    public void ClearEnemyDatas()
    {
        sendDatas.Clear();
    }
    void Start()
    {
        instance = this;
        //FadeManager.Instance.FadeIn(0);
        batlleDataList = new List<EnemyData.EnemyInternalDatas>();
        sendDatas = new List<EnemyData.EnemyInternalDatas>();
        unitPos = 0;
        isUnitTouch = false;
        setEpisodeStage();
        //FadeManager.Instance.FadeOut(2);
    }

    // Update is called once per frame
    void Update()
    {
        OnTouchDown();
    }
    public void GoScenario()
    {
        FadeManager.Instance.FadeInActionFadeout(2,1,()=> { ReslutCanvas.Instance.SetEnable(true);});
        
    }
    void OnTouchDown()
    {
        if (TabManager.Instance.Getisblood()) return;
        if (InputManager.Instance.IsTouchBegan() && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit)
            {
                GameObject obj = hit.collider.gameObject;
                if (hit.collider.gameObject.tag == "Unit")
                {
                    obj.GetComponent<Unit>().OnTouchDown();
                    //Debug.Log("タッチ");
                    NewCamera.Instance.SetUnitT(0);
                    unitPos = obj.GetComponent<Unit>().getPos();
                    AudioManager.Instance.PlaySe("hitotap.wav");
                    NewCamera.Instance.SetUnitStartPos(NewCamera.Instance.cameraposx);
                    isUnitTouch = true;
                }
                else if (hit.collider.tag == "Untagged")
                {
                    //Debug.Log("よくわからないもの");
                    MyCanvas.SetInteractive("Start", false);
                }
                else if (hit.collider.tag == "Goal")
                {
                    Debug.Log("あたり");
                    EnableReturnUI();
                }
                if (hit.collider.gameObject.tag != "Unit")
                {
                   
                }
            }
            else
            {
                EnemyDataCanvas.Instance.CancelPlate();
                
                Debug.Log("はずれ");
                MyCanvas.SetInteractive("Start", false);
            }
        }

    }
    public void BattleSceneChange()
    {
        float time = 1.5f;
        FadeManager.Instance.FadeInOut(time - 0.1f, 0.4f);
        Invoke("SceneChangeBattle", time);

    }
    private void SceneChangeBattle()
    {
        SceneManager.LoadScene("Battle");
    }
    public void SceneChange(string nextSceneName_)
    {
        SceneManager.LoadScene(nextSceneName_);
    }
    public void EnableReturnUI()
    {
        ReturnCanvas.setenableReturnUI(true);
    }
    public void DisEnableReturnUI()
    {
        DataManager.Instance.IsUnitDestroys[enemynumber] = true;
        DataManager.Instance.CameraPos = new Vector3(NewCamera.Instance.cameraposx, 0, 0);
        ReturnButtonCanvas.Instance.setenableReturnUI(false);
    }
    public void BackScenarioSelect()
    {
        float waittime = 2.0f;
        FadeManager.Instance.FadeInOut(waittime-0.1f, 0.5f);
        Invoke("ChangeSceneScenarioChoice", waittime);
        
    }
    private void ChangeSceneScenarioChoice()
    {
        SceneChange("ScenarioChoice");
    }
    public string[] EpisodeStageNum(string lines_, char[] spliter_, int trialNumber_)
    {
        //カンマとカンマの間に何もなかったら格納しないことにする設定
        System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;

        //リターン値。カンマ分けしたデータを一行分格納する。
        string[] CommaSeparationData = new string[trialNumber_];
        for (int i = 0; i < trialNumber_; i++)
        {
            //１行にあるCsvDataの要素数分準備する
            string[] readStrData = new string[trialNumber_];
            //CsvDataを引数の文字で区切って1つずつ格納
            readStrData = lines_.Split(spliter_, option);
            //readStrDataをリターン値に格納
            CommaSeparationData[i] = readStrData[i];
        }
        return CommaSeparationData;
    }
    private void setEpisodeStage()
    {
        string stage = "1_4";
        char[] commaSpliter = { '_' };
        string[] ep = EpisodeStageNum(stage, commaSpliter, 2);
        episodeNum = int.Parse(ep[0]);
        stageNum = int.Parse(ep[1]);
    }
}
