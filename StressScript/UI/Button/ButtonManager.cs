using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [Tooltip("現在のステージ")]
    private int stageNo;

    [Tooltip("ボタンのクールタイム")]
    private float buttoncooltime;

    [Tooltip("ボタンが押されたか")]
    private bool pushButton = false;
    [Tooltip("シーン遷移できるか")]
    private bool scenetransition = false;

    [SerializeField]
    private GameObject Fade;
    [SerializeField]
    private GameObject Fade1;

    private caramel.Timer _timer;

    void Start()
    {
        //初期設定
        buttoncooltime = 1.0f;

        pushButton = false;

        _timer = new();

#if UNITY_EDITOR
        //PlayerPrefs.SetInt("NowStage", 1);
#endif
    }

    private void Update()
    {


        //遷移後ボタンクールタイム
        if (buttoncooltime > 0 && pushButton)
        {
            //クールタイムカウント
            buttoncooltime -= Time.deltaTime;

        }
        //クールタイムが過ぎたら
        else if (buttoncooltime <= 0)
        {
            scenetransition = true;
        }
    }

    private void FixedUpdate()
    {
        _timer.Update();
    }

    public void OnClickLoadStageButton(int num)
    {
        if (pushButton)
            return;

        Fade.SetActive(true);
        Fade1.SetActive(true);

        FadeManager.Instance.StartFade(isFadeIn: true);

        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);

        _timer.CreateTask(() => SceneManager.LoadScene("Stage1"), 4.0f);
        
        //ボタンが押されたか
        pushButton = true;

        StageManager.LoadStageNumber = num;

        ////シーン遷移できるか
        //if (scenetransition)
        //    SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// ロード画面へ
    /// </summary>
    public void OnClickLoad1Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Load1");
        //ボタンが押されたか
        pushButton = true;

        //シーン遷移できるか
        if (scenetransition)
            SceneManager.LoadScene("Load1");

    }

    /// <summary>
    /// ロード画面へ
    /// </summary>
    public void OnClickLoad2Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Load2");
        //ボタンが押されたか
        pushButton = true;

        //シーン遷移できるか
        if (scenetransition)
            SceneManager.LoadScene("Load2");
    }

    /// <summary>
    /// ロード画面へ
    /// </summary>
    public void OnClickLoad3Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Load3");
        //ボタンが押されたか
        pushButton = true;

        //シーン遷移できるか
        if (scenetransition)
            SceneManager.LoadScene("Load3");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void OnClickQuitButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        Application.Quit();
    }

    /// <summary>
    /// タイトル画面へ
    /// </summary>
    public void OnClickTitleButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// 遊び方へ
    /// </summary>
    public void OnClickGuideButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Guide");
    }

    /// <summary>
    /// ステージセレクト画面へ
    /// </summary>
    public void OnClickStageSelectButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        Time.timeScale = 1;
        pushButton = true;

        Time.timeScale = 1;

        SceneManager.LoadScene("StageSelect");

    }

    /// <summary>
    /// ステージ１面へ
    /// </summary>
    public void OnClickStage1Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// ステージ２面へ
    /// </summary>
    public void OnClickStage2Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Stage2");
    }

    /// <summary>
    /// ステージ３面へ
    /// </summary>
    public void OnClickStage3Button()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        SceneManager.LoadScene("Stage3");
    }

    public void OnClickRetryButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        stageNo = PlayerPrefs.GetInt("NowStage", 1);

        if (pushButton)
            return;

        Fade.SetActive(true);
        Fade1.SetActive(true);

        FadeManager.Instance.StartFade(isFadeIn: true);

        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);

        _timer.CreateTask(() => SceneManager.LoadScene("Stage1"), 4.0f);

        //ボタンが押されたか
        pushButton = true;

        StageManager.LoadStageNumber = stageNo;
    }

    public void OnClickNextStageButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);
        stageNo = PlayerPrefs.GetInt("NowStage");
        
        if (pushButton)
            return;

        Fade.SetActive(true);
        Fade1.SetActive(true);

        FadeManager.Instance.StartFade(isFadeIn: true);

        SoundManager.Instance.PlaySE(SESoundData.SE.Enter);

        _timer.CreateTask(() => SceneManager.LoadScene("Stage1"), 4.0f);

        //ボタンが押されたか
        pushButton = true;

        //if (stageNo < 3)
        //    ++stageNo;

        StageManager.LoadStageNumber = stageNo;
    }
}