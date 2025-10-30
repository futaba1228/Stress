using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonCoolTime : MonoBehaviour
{

    [Tooltip("ボタンのクールタイム")]
    private float buttoncooltime;

    [Tooltip("ボタン取得")]
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        //ボタン取得
        button = this.GetComponent<Button>();
        //シーンを移動したので初期設定
        buttoncooltime = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //遷移後ボタンクールタイム
        if (buttoncooltime > 0)
        {
            //クールタイムカウント
            buttoncooltime -= Time.deltaTime;
            return;

        }
        //クールタイムが過ぎたら
        else if (buttoncooltime <= 0)
        {
            //ボタンをオン
            button.enabled = true;
        }
    }
}
