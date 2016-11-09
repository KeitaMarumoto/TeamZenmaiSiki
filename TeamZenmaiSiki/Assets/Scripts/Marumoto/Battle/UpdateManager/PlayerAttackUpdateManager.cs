﻿using UnityEngine;
using System.Collections;

public class PlayerAttackUpdateManager : MonoBehaviour {
    private static PlayerAttackUpdateManager instance;
    public static PlayerAttackUpdateManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    PlayerAttackController playerAttackController;

    GameObject attackCircle;      //InstantiateしたAttackCircle。
    GameObject targetObject;      //現在のターゲットオブジェクト。
    bool circleColliderEnable;    //CircleColliderが有効化されているか。
    bool isHit;                   //CircleColliderに敵がヒットしたか。

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        circleColliderEnable = false;
        isHit = false;

    }

	void LateUpdate ()
    {
        if (circleColliderEnable)
        {
            StartCoroutine(PlayerAttacking());
        }
	}

    /// <summary>
    /// プレイヤーの一連の攻撃のコルーチン関数
    /// </summary>
    /// <returns>呼ばれた直後に1フレーム処理を待つ</returns>
    private IEnumerator PlayerAttacking()
    {
        SetCircleColliderEnable(false);
        yield return null;

        Debug.Log("Coroutine");
        HitSequence();
        playerAttackController.ProgressCurrentTargetIndex();
        Destroy(attackCircle);
        Debug.Log("Destroy AttackCircle!");
    }

    /// <summary>
    /// 攻撃が当たっていたときの処理。
    /// ターゲットを破壊し、その情報を消す。
    /// </summary>
    private void HitSequence()
    {
        if (isHit)
        {
            Debug.Log("isHit");
            Destroy(targetObject);
            Debug.Log("Destroy TargetObject");
            EnemyManager.Instance.EnemyPosErase();
            playerAttackController.DecreaseCurrentTargetIndex();
            isHit = false;
        }
    }

    public void SetCircleColliderEnable(bool _cond) { circleColliderEnable = _cond; }
    public void SetAttackCircleObject(GameObject _refObj) { attackCircle = _refObj; }
    public void SetTargetObject(GameObject _refObj) { targetObject = _refObj; }
    public void SetIsHit(bool _cond) { isHit = _cond; }
}
