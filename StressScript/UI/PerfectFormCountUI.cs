using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerfectFormCountUI : MonoBehaviour
{
    [Tooltip("Playerスクリプト取得用")]
    [SerializeField]
    GameObject PlayerObj;

    Player player;

    [Tooltip("消える方の画像")]
    [SerializeField]
    Image UIObj;
    [Tooltip("何秒回るか")]
    public float countTime; //プレイヤーはまだ

    // Start is called before the first frame update
    void Start()
    {
        //Playerスクリプト取得
        player = PlayerObj.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //もし浄化されたら
        if (player.IsPerfectModeInterval)
        {
            UIObj.enabled = true;
            UIObj.fillAmount -= Time.deltaTime / countTime;
        }
        else
        {
            UIObj.enabled = false;
            UIObj.fillAmount = 1.0f;
        }
    }
}
