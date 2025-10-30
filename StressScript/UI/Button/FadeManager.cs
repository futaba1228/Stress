using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField]
    private ImageWrapper _fadeBackGround;
    [SerializeField]
    private ImageWrapper _fadeImage;

    private Durator _durator;

    [SerializeField]
    private float _fadeTime;

    public bool IsFade
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _durator = new();
        _fadeImage.Initialize();
        _fadeBackGround.Initialize();
        IsFade = true;
    }

    // Update is called once per frame
    void Update()
    {
        _durator.Update();
    }

    /// <summary>
    /// 暗転を始めるか、解除する関数
    /// </summary>
    /// <param name="isFadeIn">暗転させるか</param>
    public void StartFade(bool isFadeIn, Durator.CallBackTask callBack = null)
    {
        if (callBack == null)
            callBack = isFadeIn ? FadeInComplete : FadeOutComplete;

        if (isFadeIn)
        {
            _durator.CreateTask(FadeIn, callBack, _fadeTime);
            IsFade = true;
        }
        else
            _durator.CreateTask(FadeOut, callBack, _fadeTime);
    }

    /// <summary>
    /// 画面暗転させる関数
    /// </summary>
    /// <returns>1フレーム待機</returns>
    public void FadeIn(float elapsedTime, float endTime)
    {
        //色変更前の色を取得
        Color currentColor = _fadeImage.GetCurrentColor();

        //変更後の色を取得
        Color startColor = new(currentColor.r, currentColor.g, currentColor.b, 0.0f);

        //変更後の色を取得
        Color endColor = new(currentColor.r, currentColor.g, currentColor.b, 1.0f);

        float t = Mathf.Clamp01(elapsedTime / endTime);

        _fadeImage.SetImageAlpha(t);
        _fadeBackGround.SetImageAlpha(t);
    }

    private void FadeInComplete()
    {
        //色変更前の色を取得
        Color currentColor = _fadeImage.GetCurrentColor();

        _fadeImage.SetImageAlpha(1);
        _fadeBackGround.SetImageAlpha(1);
    }

    /// <summary>
    /// 画面暗転を解除する関数
    /// </summary>
    /// <returns>1フレーム待機</returns>
    public void FadeOut(float elapsedTime, float endTime)
    {
        //色変更前の色を取得
        Color currentColor = _fadeImage.GetCurrentColor();

        //変更後の色を取得
        Color startColor = new(currentColor.r, currentColor.g, currentColor.b, 1.0f);

        //変更後の色を取得
        Color endColor = new(currentColor.r, currentColor.g, currentColor.b, 0.0f);

        float t = Mathf.Clamp01(elapsedTime / endTime);

        _fadeImage.SetImageAlpha(t);
        _fadeBackGround.SetImageAlpha(t);
    }

    private void FadeOutComplete()
    {
        //色変更前の色を取得
        Color currentColor = _fadeImage.GetCurrentColor();

        _fadeImage.SetImageAlpha(0);
        _fadeBackGround.SetImageAlpha(0);

        IsFade = false;
    }
}
