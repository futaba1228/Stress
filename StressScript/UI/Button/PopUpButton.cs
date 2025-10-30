using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpButton : MonoBehaviour
{

    [SerializeField]
    ButtonFocus buttonFocus;

    private void Start()
    {
        buttonFocus=buttonFocus.GetComponent<ButtonFocus>();
    }

    public void OnClikPopUp()
    {
        //ポップアップボタンが押されたので元をオフ
        buttonFocus.enabled = false;
    }
}
