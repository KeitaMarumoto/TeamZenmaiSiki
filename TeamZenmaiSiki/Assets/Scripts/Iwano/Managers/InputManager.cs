﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/*
 The MIT License (MIT)
Copyright (c) 2016 Misawa Hiroki

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get { return instance; }
    }

    private float touchTime = 0f;

    //シーンまたいでもオブジェクトが破棄されなくする
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary> 実行環境が Android なら true を返す </summary>
    public bool isAndroid
    {
        get { return Application.platform == RuntimePlatform.Android; }
    }

    /// <summary> 実行環境が iOS なら true を返す </summary>
    public bool isIPhone
    {
        get { return Application.platform == RuntimePlatform.IPhonePlayer; }
    }

    /// <summary> 実行環境がスマートフォンなら true を返す </summary>
    public bool isSmartDevice
    {
        get { return isAndroid || isIPhone; }
    }

    /// <summary> タッチされたときのスクリーン座標を返す：左下 (0, 0) </summary>
    public Vector3 GetTouchPosition()
    {
        // Vector3 として扱うのは、オブジェクトの transform.position が Vector3 のため
        Vector3 touch = Vector3.zero;

#if UNITY_STANDALONE
        // スマートフォンでなければ、マウス座標を代わりに返す
        touch = Input.mousePosition;
#else
        // タッチされていれば、スクリーンの座標を返す
        if (Input.touchCount > 0) { touch = Input.touches[0].position; }
#endif

        return touch;
    }

    // タッチが指定された状態に一致するなら true を返す
    bool GetTouchState(TouchPhase state)
    {
        return Input.touchCount > 0 ?
          Input.touches[0].phase == state : false;
    }

    /// <summary> タッチされたら true を返す </summary>
    public bool IsTouchBegan()
    {
        bool result = false;
#if UNITY_STANDALONE
        // Unity エディター、または Windows、MacOSX 向けビルドの場合の判定
        // マウスの左クリックを判定基準にする
        result = Input.GetMouseButtonDown(0);
#else
        // タッチされていれば、その状態を調べる
        result = GetTouchState(TouchPhase.Began);
#endif
        return result;
    }

    /// <summary> タッチされ続けている間 true を返す </summary>
    public bool IsTouchMoved()
    {
        bool result = false;

#if UNITY_STANDALONE
        result = Input.GetMouseButton(0);
#else
        // タッチされていれば、その状態を調べる
        if (Input.touchCount > 0)
        {
            var touch = Input.touches[0];

            // 動いていないがタッチされ続けている場合もあるので、それも考慮する
            bool isMove = (touch.phase == TouchPhase.Moved);
            bool isWait = (touch.phase == TouchPhase.Stationary);

            result = isMove || isWait;
        }
#endif
        return result;
    }

    /// <summary> タッチが離されたら true を返す </summary>
    public bool IsTouchEnded()
    {
        bool result = false;

#if UNITY_STANDALONE
        result = Input.GetMouseButtonUp(0);
#else
        result = GetTouchState(TouchPhase.Ended);
#endif
        return result;
    }

    public float TouchTime()
    {
        float time = 0;
        if (IsTouchBegan()) touchTime = 0;
        if (IsTouchMoved())
        {
            touchTime += Time.deltaTime;
            time = touchTime;
        }
        return time;
    }

    /// <summary> 端末の戻るボタンが押されたら true を返す </summary>
    public bool IsPushedQuitKey()
    {
        // KeyCode.Escape は、スマートフォンの戻るボタンに対応している
        return Input.GetKeyDown(KeyCode.Escape);
    }
}