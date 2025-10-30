using DG.Tweening;
using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NOBoss : EnemyBase
{
    /// <summary>
    /// 攻撃のシークエンス
    /// </summary>
    private enum AttackSequence
    {
        /// <summary>
        /// 突進
        /// </summary>
        Dash,
        /// <summary>
        /// ジャンプで突っ込む
        /// </summary>
        Jump,
        /// <summary>
        /// 敵を投げる
        /// </summary>
        ThrowEnemy,
        /// <summary>
        /// 疲労
        /// </summary>
        Tired
    }

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
    }

    private void FixedUpdate()
    {
        if (!_isDiscoveredPlayer)
        {
            if (CheckInScreen())
            {
                DiscoverPlayer();
                EnterTiredMode(1.0f, AttackSequence.Dash);
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

        _mover.Fall();
        _mover.Move();

        return;
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

        _timer.CreateTask(() => _isInvincible = false, 2.1f);

        _life.TakeDamage();

        SoundManager.Instance.PlaySE(SESoundData.SE.BossDamege);

        _anim.SetTrigger("IsDamaged");
        _anim.SetFlag("IsJump", false);
        _anim.SetFlag("IsDash", false);

        EnterTiredMode(1.0f, AttackSequence.Jump);

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

        EnterTiredMode(2.0f, AttackSequence.ThrowEnemy);
    }

    /// <summary>
    /// プレイヤーに向かってジャンプする攻撃
    /// </summary>
    private void JumpToPlayer()
    {
        _isInvincible = true;

        _anim.SetFlag("IsJump", true);

        this.transform.DOJump(PlayerManager.Instance.Player.transform.position, 14.0f, 1, 1.5f).SetEase(Ease.Linear).OnComplete(EndJump);
    }

    private void EndJump()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.NOBossAttack);

        _isInvincible = false;

        _anim.SetFlag("IsJump", false);

        EnterTiredMode(1.0f, GetNextSeqence());

        AttackSequence GetNextSeqence()
        {
            return _life.CurrentLife switch
            {
                2 => AttackSequence.Jump,
                1 => AttackSequence.ThrowEnemy,
                _ => AttackSequence.Dash
            };
        }
    }

    /// <summary>
    /// 敵を投げる攻撃
    /// </summary>
    private void ThrowEnemy()
    {
        _anim.SetTrigger("IsThrow");

        SoundManager.Instance.PlaySE(SESoundData.SE.SPBossAttack);

        var enemy = caramel.EnemyManager.SpawnEnemy(EnemyType.NOEnemy, this.transform.position);

        enemy.DiscoverPlayer();

        EnterTiredMode(2.0f, AttackSequence.Jump);
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
                case AttackSequence.Dash:

                    StartDash();

                    break;

                case AttackSequence.Jump:

                    JumpToPlayer();

                    break;

                case AttackSequence.ThrowEnemy:

                    ThrowEnemy();

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

        int nowStage = PlayerPrefs.GetInt("NowStage");

        if (nowStage < 3)
            PlayerPrefs.SetInt("NowStage", 3);

        Time.timeScale = 0.5f;

        //破棄処理を予約
        _timer.CreateTask(() => { Destroy(this.gameObject); Time.timeScale = 1.0f; SceneManager.LoadScene("GameClear"); }, 3.0f);

        caramel.EnemyManager.Instance.CreateBombAura(transform.position);
    }
}
