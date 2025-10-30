using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButtonImage : MonoBehaviour
{

    [Tooltip("ButtonSelectスクリプト取得用")]
    [SerializeField]
    GameObject buttonObj;

    ButtonSelect buttonSelect;

    [Tooltip("選択時人魂画像")]
    [SerializeField]
    Sprite SpritA;
    [Tooltip("非選択時人魂画像")]
    [SerializeField]
    Sprite SpritB;

    [Tooltip("SpriteRendererコンポーネント取得用")]
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        //ボタン選択スクリプト取得
        buttonSelect = buttonObj.GetComponent<ButtonSelect>();
        // SpriteRendererコンポーネント取得
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonSelect.select_f)
        {
            //画像切り替え
            image.sprite = SpritA;
        }
        else
        {
            //画像切り替え
            image.sprite = SpritB;
        }
    }
}
