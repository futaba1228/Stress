using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵に継承する基底クラス
/// </summary>
public abstract class EnemyBase : ActorMonoBehaviour<EnemyBase>
{
    /// <summary>
    /// パラメータークラス
    /// </summary>
    protected EnemyParameter _param;

    /// <summary>
    /// 移動クラス
    /// </summary>
    protected EnemyMover _mover;

    /// <summary>
    /// 体力クラス
    /// </summary>
    protected EnemyLife _life;

    /// <summary>
    /// 視覚クラス
    /// </summary>
    protected EnemyEye _eye;

    /// <summary>
    /// アニメーション管理クラス
    /// </summary>
    protected EnemyAnimationController _anim;

    /// <summary>
    /// 画像描画クラス
    /// </summary>
    protected SpriteRendererWrapper _sprite;

    /// <summary>
    /// 現在の向き
    /// </summary>
    protected Direction _currentDir = Direction.Left;

    /// <summary>
    /// プレイヤーを発見したか
    /// </summary>
    protected bool _isDiscoveredPlayer = false;

    /// <summary>
    /// 前回の地面を見たときの状態
    /// </summary>
    private bool _lastGroundState;

    /// <summary>
    /// 生成ID
    /// </summary>
    public int CreateID
    { get; set; }

    /// <summary>
    /// 死亡フラグ
    /// </summary>
    public bool IsDead
    { get; set; }

    /// <summary>
    /// 無敵か？
    /// </summary>
    protected bool _isInvincible = false;

    /// <summary>
    /// カメラに映っている
    /// </summary>
    public bool IsCameraIn
    { get => _sprite.InScreen(); }

    public override void Initialize()
    {
        base.Initialize();

        if (_param == null)
            return;

        //使用する各クラスを初期化
        _mover = new(this.transform, _param.MoveSpeed);
        _life = new(_param.Life);
        _eye = new();
        SetScale();
        Rect = new(this);

        void SetScale()
        {
            var scale = this.transform.localScale;
            scale.x = _param.Width;
            scale.y = _param.Height;
            this.transform.localScale = scale;
        }
    }

    /// <summary>
    /// 画像描画クラスをセットする関数
    /// </summary>
    public void SetSprite(SpriteRendererWrapper renderer)
        => _sprite = renderer;

    /// <summary>
    /// パラメーターをセットする関数
    /// </summary>
    /// <param name="setParam">セットするパラメーター</param>
    public void SetParameter(EnemyParameter setParam)
        => _param = setParam;

    /// <summary>
    /// パラメーターをセットする関数
    /// </summary>
    /// <param name="setAnim">セットするアニメーション</param>
    public virtual void SetAnimation(RuntimeAnimatorController setAnim)
    {
        //画像描画クラスからゲームオブジェクトの情報をもらう
        var spriteObj = _sprite?.gameObject;
        //アニメーターをアタッチ
        var anim = spriteObj.AddComponent<Animator>();
        //使用するアニメーションのコントロールをセット
        anim.runtimeAnimatorController = setAnim;
        //アニメーション管理クラスを生成
        _anim = new(anim);
    }

    /// <summary>
    /// 浄化時に呼ばれる関数
    /// </summary>
    public abstract void OnClarification();

    /// <summary>
    /// 死亡時に呼ばれる関数
    /// </summary>
    public virtual void OnDead()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.EnemyDeath);

        //死亡フラグを立てる
        IsDead = true;
        _anim?.SetFlag("IsEnemyDie", true);

        if (!(this is EnemyBullet))
        GameManager.Instance.KillCount();

        //破棄処理を予約
        _timer.CreateTask(() => Destroy(this.gameObject), 1.0f);
    }

    /// <summary>
    /// プレイヤー発見時の処理
    /// </summary>
    public abstract void DiscoverPlayer();

    protected bool IsOutMoveArea()
    {
        float playerX = PlayerManager.Instance.Player.transform.position.x;

        //36以内なら動いていいエリア判定
        return Mathf.Abs(playerX - this.transform.position.x) >= 36.0f;
    }

    /// <summary>
    /// 前方斜め下にRayを飛ばして接地状況が
    /// 変わりそうかを返す関数
    /// </summary>
    /// <returns>引き返すべきか</returns>
    protected bool CheckGroundState()
    {
        LayerMask mask = 1 << 6;

        bool lastState = _lastGroundState;

        _lastGroundState = Physics2D.Raycast(
            this.transform.position,
            -this.transform.up + (_sprite.FlipX ? this.transform.right : -this.transform.right),
            this.transform.localScale.x,
            mask);

        return _lastGroundState == lastState && !lastState;
    }
}
