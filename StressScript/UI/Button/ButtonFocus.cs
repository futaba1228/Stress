using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFocus : MonoBehaviour
{
    //フォーカスされるボタン
    [SerializeField] Button focusButton;

    // Start is called before the first frame update
    void Start()
    {
        // ボタンコンポーネントの取得
        focusButton = focusButton.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //全てのフォーカスを解除する
            EventSystem.current.SetSelectedGameObject(null);
            //focusButtonにフォーカスする
            focusButton.Select();
            //Canvasコンポーネントを無効にする。Buttonコンポーネントで設定可。
        }
    }

}
