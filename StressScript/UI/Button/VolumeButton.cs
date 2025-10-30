using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeButton : MonoBehaviour
{
    [SerializeField]
    private GameObject VolumePanel;

    [SerializeField]
    private GameObject volumeFirstbutton;

    [SerializeField]
    private GameObject stageSelectFirstbutton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickVolumeButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(volumeFirstbutton);

        VolumePanel.SetActive(true);
    }

    public void OnClickReturnVolumeButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(stageSelectFirstbutton);

        VolumePanel.SetActive(false);
    }
}
