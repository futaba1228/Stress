using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StageSelectManager : MonoBehaviour
{
    // ボタンを配列として定義
    [SerializeField] private Button[] _stageButton;
    void Start()
    {
        // ステージのクリア数を取得
        int stageUnlock = PlayerPrefs.GetInt("StageUnlock", 1);

        // ステージボタンの表示・非表示の設定
        for (int i = 0; i < _stageButton.Length; i++)
        {
            if (i < stageUnlock)
                _stageButton[i].interactable = true;
            else
                _stageButton[i].interactable = false;
        }
    }
    public void StageSelect(int stage)
    {
        // 受け取った引数(stage)のステージをロードする
        SceneManager.LoadScene(stage);
    }

    private void Update()
    {
        if(Input.GetKeyDown("joystick button 1"))
        {
            SceneManager.LoadScene("Title");
        }
    }
}