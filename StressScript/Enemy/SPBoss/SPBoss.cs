using DG.Tweening;
using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SPBoss : EnemyBase
{
    /// <summary>
    /// 攻撃のシークエンス
    /// </summary>
    private enum AttackSequence
    {
        /// <summary>
        /// 範囲攻撃
        /// </summary>
        AuraSpell,
        /// <summary>
        /// バリア生成
        /// </summary>
        CreateBarrier,
        /// <summary>
        /// 弾を発射する
        /// </summary>
        ShootBullet,
        /// <summary>
        /// 突進
        /// </summary>
        Dash,
        /// <summary>
        /// 敵を投げる
        /// </summary>
        ThrowEnemy,
        /// <summary>
        /// 疲労
        /// </summary>
        Tired
    }

    [SerializeField]
    private Transform _collider;

    [SerializeField]
    private Transform _spellAuraPoint;

    [SerializeField]
    private SpriteRendererWrapper _realSprite;

    [SerializeField]
    private SpriteRendererWrapper _barrier;

    [SerializeField]
    private Animator _animator;

    /// <summary>
    /// 突進用の移動クラス
    /// </summary>
    private EnemyMover _dashMover;

    /// <summary>
    /// 攻撃中か？
    /// </summary>
    private bool _isAttack;

    /// <summary>
    /// 突進中か？
    /// </summary>
    private bool _isDash;

    /// <summary>
    /// 現在の攻撃
    /// </summary>
    private AttackSequence _currentSequence = AttackSequence.Tired;

    /// <summary>
    /// 疲労時のタスク管理用ID
    /// </summary>
    private int _tiredTaskID = -1;

    private void Start()
    {
        _dashMover = new(this.transform, _param.MoveSpeed * 2.0f);
        Rect = new(_collider, hasParent: true);
        _realSprite.Initialize();
        _sprite = _realSprite;
        _barrier.Initialize();
    }

    private void FixedUpdate()
    {
        if (!_isDiscoveredPlayer)
        {
            if (CheckInScreen())
            {
                DiscoverPlayer();
                EnterTiredMode(0.0f, AttackSequence.CreateBarrier);
            }
            else
                return;
        }

        _timer.Update();

        if (IsDead)
            return;

        if (_currentSequence == AttackSequence.Dash)
        {
            _dashMover.Walk(_currentDir);
            _dashMover.Fall();
            _dashMover.Move();

            if (_dashMover.IsNeedTurnBack)
                TurnBack();
        }
        else
            TurnToPlayer();

        _mover.Fall();
        _mover.Move();

        return;
    }

    /// <summary>
    /// パラメーターをセットする関数
    /// </summary>
    /// <param name="setAnim">セットするアニメーション</param>
    public override void SetAnimation(RuntimeAnimatorController setAnim)
    {
        //画像描画クラスからゲームオブジェクトの情報をもらう
        var spriteObj = _sprite?.gameObject;
        //アニメーターをアタッチ
        //使用するアニメーションのコントロールをセット
        _animator.runtimeAnimatorController = setAnim;
        //アニメーション管理クラスを生成
        _anim = new(_animator);
    }

    /// <summary>
    /// 反対に向き直る関数
    /// </summary>
    private void TurnBack()
    {
        _currentDir = _currentDir == Direction.Right ? Direction.Left : Direction.Right;
        _sprite.SetFlipX(!_sprite.FlipX);
    }

    /// <summary>
    /// プレイヤーの方を向く関数
    /// </summary>
    private void TurnToPlayer()
    {
        var playerPosX = PlayerManager.Instance.Player.transform.position.x;
        _currentDir = this.transform.position.x > playerPosX ? Direction.Left : Direction.Right;
        _sprite.SetFlipX(!(this.transform.position.x > playerPosX));
    }

    /// <summary>
    /// 浄化時の処理
    /// </summary>
    public override void OnClarification()
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        _life.TakeDamage();

        SoundManager.Instance.PlaySE(SESoundData.SE.BossDamege);

        _anim.SetTrigger("IsDamaged");
        _anim.SetFlag("IsJump", false);
        _anim.SetFlag("IsDash", false);

        EnterTiredMode(1.0f, AttackSequence.CreateBarrier);

        if (_life.IsArrive)
            return;

        OnDead();
    }

    public override void DiscoverPlayer()
    {
        //プレイヤー発見フラグを立てる
        _isDiscoveredPlayer = true;

        //攻撃フラグをセット
        _anim.SetFlag("IsEnemyAttack", true);

        //カメラを引く
        PlayerManager.Instance.PlayerCam.ChanngeCameraScale(isScaleUp: true);
    }

    private void CreateBarrier()
    {
        _barrier.SetSpriteAlpha(1.0f);
        _isInvincible = true;

        _timer.CreateTask(CloseBarrier, 7.0f);

        EnterTiredMode(_param.AttackInterval, GetNextSeqence());

        AttackSequence GetNextSeqence()
        {
            return _life.CurrentLife switch
            {
                3 => AttackSequence.ShootBullet,
                2 => AttackSequence.Dash,
                1 => AttackSequence.ThrowEnemy,
                _ => AttackSequence.Dash
            };
        }
    }

    private void CloseBarrier()
    {
        _barrier.SetSpriteAlpha(0.0f);
        _isInvincible = false;
    }

    private void AuraSpell()
    {
        _anim.SetTrigger("IsThrow");

        var shootDir = (PlayerManager.Instance.Player.transform.position - this.transform.position).normalized;
        shootDir.y = 0;

        var bullet = caramel.EnemyManager.CreateEnemyBullet<EnemyBullet>(_param.Name, _spellAuraPoint.position, shootDir);

        //範囲攻撃の大きさを設定
        bullet.transform.localScale = new(_param.Width * 0.05f, _param.Height * 0.05f);

        EnterTiredMode(_param.SerchedAttackInterval, GetNextSeqence());

        AttackSequence GetNextSeqence()
        {
            int rand = Random.Range(0, 3);

            return _life.CurrentLife switch
            {
                0 => AttackSequence.ShootBullet,
                1 => AttackSequence.Dash,
                2 => AttackSequence.ThrowEnemy,
                _ => AttackSequence.AuraSpell
            };
        }
    }

    private void StartDash()
    {
        _isDash = true;

        _anim.SetFlag("IsDash", true);

        _timer.CreateTask(EndDash, _param.SerchedAttackInterval);
    }

    private void EndDash()
    {
        _anim.SetFlag("IsDash", false);

        EnterTiredMode(_param.SerchedAttackInterval, AttackSequence.ThrowEnemy);
    }

    /// <summary>
    /// プレイヤーに向けて弾を飛ばす関数
    /// </summary>
    private void ShootBullet()
    {
        //体力が1,3の時は撃ってくる
        if (_life.CurrentLife == 2)
            return;

        SoundManager.Instance.PlaySE(SESoundData.SE.SPBossAttack);

        _anim.SetTrigger("IsThrow");

        var shootDir = (PlayerManager.Instance.Player.transform.position - this.transform.position).normalized;

        var bullet = caramel.EnemyManager.CreateEnemyBullet<EnemyBullet>(_param.Name, this.transform.position, shootDir);

        //弾の大きさを設定
        bullet.transform.localScale = new(_param.Width * 0.05f, _param.Height * 0.05f);

        EnterTiredMode(_param.SerchedAttackInterval, AttackSequence.AuraSpell);
    }

    /// <summary>
    /// プレイヤーに向けて弾を飛ばす関数
    /// </summary>
    private void ShootEnemy()
    {
        //体力が1,2の時は撃ってくる
        if (_life.CurrentLife == 3)
            return;

        _anim.SetTrigger("IsThrow");

        var enemy = caramel.EnemyManager.SpawnEnemy(EnemyType.NPEnemy, this.transform.position);

        enemy.DiscoverPlayer();

        EnterTiredMode(_param.SerchedAttackInterval, AttackSequence.AuraSpell);
    }

    /// <summary>
    /// 疲労状態に入る関数
    /// </summary>
    /// <param name="tiredTime">疲労時間</param>
    /// <param name="nextSequence">疲労後の次の行動</param>
    private void EnterTiredMode(float tiredTime, AttackSequence nextSequence)
    {
        _anim.SetTrigger("IsSign");

        if (_timer.GetTaskFromID(_tiredTaskID) != null)
            _timer.CanncellTask(_tiredTaskID);

        _currentSequence = AttackSequence.Tired;

        //指定時間の疲労の後、行動を再開
        _tiredTaskID = _timer.CreateTask(NextSequence, tiredTime);

        void NextSequence()
        {
            _currentSequence = nextSequence;

            switch (nextSequence)
            {
                case AttackSequence.CreateBarrier:

                    CreateBarrier();

                    break;

                case AttackSequence.Dash:

                    StartDash();

                    break;

                case AttackSequence.ShootBullet:

                    ShootBullet();

                    break;

                case AttackSequence.AuraSpell:

                    AuraSpell();

                    break;

                case AttackSequence.ThrowEnemy:

                    ShootEnemy();

                    break;
            }
        }
    }

    /// <summary>
    /// 画面内に入ったかを調べる関数
    /// </summary>
    /// <returns></returns>
    private bool CheckInScreen()
    {
        return _sprite.InScreen();
    }

    public override void OnDead()
    {
        IsDead = true;

        _anim.SetFlag("IsBossDie", true);

        Time.timeScale = 0.5f;

        PlayerPrefs.SetInt("NowStage", 5);

        //破棄処理を予約
        _timer.CreateTask(() => { Destroy(this.gameObject); Time.timeScale = 1.0f; SceneManager.LoadScene("End"); }, 3.0f);

        caramel.EnemyManager.Instance.CreateBombAura(transform.position);
    }
}
