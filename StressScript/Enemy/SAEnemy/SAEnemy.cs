using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAEnemy : EnemyBase
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

        _timer.Update();

        if (!_isBulletShootMode)
            _mover.Walk(_currentDir);

        _mover.Fall();
        _mover.Move();

        if (_mover.IsNeedTurnBack || (CheckGroundState() && _mover.GroundCheck(out var hit)))
            TurnBack();

        if (!_isDiscoveredPlayer)
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
        _life.TakeDamage();

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
        SoundManager.Instance.PlaySE(SESoundData.SE.SAEnemyAttack);

        _anim.SetFlag("IsEnemyAttack", true);

        _isBulletShootMode = true;

        _timer.CreateTask(() => _isBulletShootMode = false, _param.AttackInterval);

        var shootDir = (PlayerManager.Instance.Player.transform.position - this.transform.position).normalized;

        caramel.EnemyManager.CreateEnemyBullet<EnemyBullet>(_param.Name, this.transform.position, shootDir);
    }
}
