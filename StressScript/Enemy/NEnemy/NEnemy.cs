using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通常敵
/// </summary>
public class NEnemy : EnemyBase
{
    private void FixedUpdate()
    {
        if (IsOutMoveArea())
            return;

        _timer.Update();

        if (IsDead)
            return;

        _mover.Walk(_currentDir);
        _mover.Fall();
        _mover.Move();

        if (_mover.IsNeedTurnBack || (CheckGroundState() && _mover.GroundCheck(out var hit)))
            TurnBack();

        if (!_isDiscoveredPlayer)
            return;

        TurnToPlayer();
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

        //攻撃フラグをセット
        _anim.SetFlag("IsEnemyAttack", true);
    }
}
