using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DataButtion : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    private GameObject DataPanel;

    [SerializeField]
    private GameObject ResetPanel;

    [SerializeField]
    private GameObject dataFirstbutton;

    [SerializeField]
    private GameObject resetFirstbutton;

    [SerializeField]
    private GameObject titleFirstbutton;

    public void OnClickDataButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(dataFirstbutton);

        DataPanel.SetActive(true);
    }

    public void OnClickResetButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(resetFirstbutton);

        ResetPanel.SetActive(true);

        DataPanel.SetActive(false);
    }

    public void OnClickTitleRetrunButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(titleFirstbutton);

        DataPanel.SetActive(false);
    }

    public void OnClickDataReturnButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(dataFirstbutton);

        ResetPanel.SetActive(false);
        DataPanel.SetActive(true);
    }

    public void OnClickResetollButton()
    {
        //初期選択ボタンの初期化
        EventSystem.current.SetSelectedGameObject(null);
        //初期選択ボタンの再指定
        EventSystem.current.SetSelectedGameObject(titleFirstbutton);

        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("NowStage", 1);
        DataPanel.SetActive(false);
        ResetPanel.SetActive(false);
    }
}
