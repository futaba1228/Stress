using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{

    [SerializeField]
    ButtonFocus buttonFocus;

    private void Start()
    {
        buttonFocus = buttonFocus.GetComponent<ButtonFocus>();
    }


    //戻るボタンが押されたか
    public void OnClikBack()
    {
        //戻るので元のスクリプトをオン
        buttonFocus.enabled=true;
    }
}
