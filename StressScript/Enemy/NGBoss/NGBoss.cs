using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NGBoss : EnemyBase
{
    /// <summary>
    /// 弾発射状態か？
    /// </summary>
    private bool _isBulletShootMode;

    /// <summary>
    /// 弾発射のインターバル中か？
    /// </summary>
    private bool _isBulletInterval = false;

    private void FixedUpdate()
    {
        if (IsOutMoveArea())
            return;

        if (!_isDiscoveredPlayer)
        {
            if (CheckInScreen())
                DiscoverPlayer();
            else
                return;
        }

        _timer.Update();

        _mover.Fall();
        _mover.Move();

        if (_mover.IsNeedTurnBack)
            TurnBack();

        if (!_isDiscoveredPlayer || IsDead)
            return;

        if (!_isBulletShootMode)
            TurnToPlayer();

        if (_isBulletInterval)
            return;

        ShootBullet();

        _isBulletInterval = true;

        _timer.CreateTask(() => _isBulletInterval = false, _param.SerchedAttackInterval);
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

        _anim.SetTrigger("IsBossDamaged");

        SoundManager.Instance.PlaySE(SESoundData.SE.BossDamege);

        if (_life.IsArrive)
            return;

        OnDead();
    }

    public override void DiscoverPlayer()
    {
        //プレイヤー発見フラグを立てる
        _isDiscoveredPlayer = true;
    }

    /// <summary>
    /// プレイヤーに向けて弾を飛ばす関数
    /// </summary>
    private void ShootBullet()
    {
        _anim.SetFlag("IsEnemyAttack", true);

        _isBulletShootMode = true;

        _timer.CreateTask(() => _isBulletShootMode = false, _param.AttackInterval);

        var shootDir = (PlayerManager.Instance.Player.transform.position - this.transform.position).normalized;

        var bullet = caramel.EnemyManager.CreateEnemyBullet<EnemyBullet>(_param.Name, this.transform.position, shootDir);

        bullet.transform.localScale = new(_param.Width, _param.Height);

        SoundManager.Instance.PlaySE(SESoundData.SE.NGBossAttack);
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
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDeath);

        IsDead = true;

        _anim.SetFlag("IsBossDie", true);

        int nowStage = PlayerPrefs.GetInt("NowStage");

        if (nowStage < 2)
            PlayerPrefs.SetInt("NowStage", 2);

        Time.timeScale = 0.5f;

        //破棄処理を予約
        _timer.CreateTask(() => { Destroy(this.gameObject); Time.timeScale = 1.0f; SceneManager.LoadScene("GameClear"); }, 3.0f);
        
        caramel.EnemyManager.Instance.CreateBombAura(transform.position);
    }
}
