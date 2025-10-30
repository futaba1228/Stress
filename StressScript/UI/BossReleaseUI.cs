using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossReleaseUI : MonoBehaviour
{

    [Tooltip("敵残りカウントテキスト")]
    [SerializeField]
    TMPro.TextMeshProUGUI ReleaseText;

    [Tooltip("GameManager取得用")]
    GameManager gameManager;

    [Tooltip("残りの敵カウント")]
    float ReleaseNum;

    // Start is called before the first frame update
    void Start()
    {
        //GameManeger取得
        gameManager=GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //最大数から浄化した敵の数を引いて残りの敵を求める
        ReleaseNum = gameManager.MaxKillEnemy - gameManager.killEnemyCount;
        
        //残り数が0になったら
        if (ReleaseNum <= 0)
        {
            ReleaseText.SetText("霧解放済み");
        }
        else
        {
            //表示
            ReleaseText.SetText("解放まで：{0}", ReleaseNum);
        }
    }
}
