using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 【背景のコントロール用クラス】
///     背景は3枚、カメラから見切れたら回り込む
/// </summary>
public class BackGroundController : MonoBehaviour
{

    // 背景の枚数
    public int spriteCount = 1;  //Insprctorから手入力できます
    // 背景が回り込み
    float rightOffset = 1.6f;  //微調整してください
    float leftOffset = -0.6f;  //微調整してください

    Transform bgTfm;
    SpriteRenderer mySpriteRndr;
    float width;

    void Start()
    {
        bgTfm = transform;
        mySpriteRndr = GetComponent<SpriteRenderer>();
        width = mySpriteRndr.bounds.size.x;
    }


    void Update()
    {
        // 座標変換
        Vector3 myViewport = Camera.main.WorldToViewportPoint(bgTfm.position);

        // 背景の回り込み(カメラがX軸プラス方向に移動時)
        if (myViewport.x < leftOffset)
        {
            bgTfm.position += Vector3.right * (width * spriteCount);
        }
        // 背景の回り込み(カメラがX軸マイナス方向に移動時)
        else if (myViewport.x > rightOffset)
        {
            bgTfm.position -= Vector3.right * (width * spriteCount);
        }
    }
}