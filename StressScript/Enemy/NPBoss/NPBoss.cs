using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPBoss : EnemyBase
{
    /// <summary>
    /// 弾発射のインターバル中か？
    /// </summary>
    private bool _isBulletInterval = false;

    /// <summary>
    /// 敵スポーンのインターバル中か？
    /// </summary>
    private bool _isSpawnInterval = false;

    private void Start()
    {
        _isInvincible = true;
        SwitchBarrierMode();
    }

    private void FixedUpdate()
    {
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

        TurnToPlayer();

        ShootBullet();

        ShootEnemy();
    }

    /// <summary>
    /// バリア状態を切り替える関数
    /// </summary>
    private void SwitchBarrierMode()
    {
        _isInvincible = !_isInvincible;
        _anim.SetFlag("IsBarrier", _isInvincible);
        _timer.CreateTask(SwitchBarrierMode, 2.0f);
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
        if (_isBulletInterval)
            return;

        //体力が1,3の時は撃ってくる
        if (_life.CurrentLife == 2)
            return;

        SoundManager.Instance.PlaySE(SESoundData.SE.NPBossAttack);

        _anim.SetFlag("IsEnemyAttack", true);

        var shootDir = (PlayerManager.Instance.Player.transform.position - this.transform.position).normalized;

        var bullet = caramel.EnemyManager.CreateEnemyBullet<EnemyBullet>(_param.Name, this.transform.position, shootDir);

        bullet.transform.localScale = new(_param.Width * 0.5f, _param.Height * 0.5f);

        _isBulletInterval = true;

        _timer.CreateTask(() => _isBulletInterval = false, _life.CurrentLife == 1 ? _param.AttackInterval * 2.0f : _param.AttackInterval);
    }

    /// <summary>
    /// プレイヤーに向けて弾を飛ばす関数
    /// </summary>
    private void ShootEnemy()
    {
        if (_isSpawnInterval)
            return;

        //体力が1,2の時は撃ってくる
        if (_life.CurrentLife == 3)
            return;

        SoundManager.Instance.PlaySE(SESoundData.SE.NPBossAttack);

        _anim.SetFlag("IsEnemyAttack", true);

        var enemy = caramel.EnemyManager.SpawnEnemy(EnemyType.NPEnemy, this.transform.position);

        enemy.DiscoverPlayer();

        _isSpawnInterval = true;

        _timer.CreateTask(() => _isSpawnInterval = false, _param.SerchedAttackInterval);
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

        SoundManager.Instance.PlaySE(SESoundData.SE.BossDeath);

        GameManager.Instance.isNPBoss = true;

        //破棄処理を予約
        _timer.CreateTask(() => { Time.timeScale = 1.0f; CallFinalStage(); }, 3.0f);

        caramel.EnemyManager.Instance.CreateBombAura(transform.position);
    }

    private void CallFinalStage()
    {
        StageManager.LoadStageNumber = 4;

        PlayerPrefs.SetInt("NowStage", 4);

        FadeManager.Instance.StartFade(isFadeIn: true, () => SceneManager.LoadScene("Stage1"));
    }
}
