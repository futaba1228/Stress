using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBEnemy : EnemyBase
{
    /// <summary>
    /// 突進用の移動クラス
    /// </summary>
    private EnemyMover _dashMover;

    /// <summary>
    /// 突進中か？
    /// </summary>
    private bool _isDash;

    private void Start()
    {
        _dashMover = new(this.transform, _param.MoveSpeed * 3.0f);
    }

    private void FixedUpdate()
    {
        if (IsOutMoveArea())
            return;

        _timer.Update();

        if (IsDead)
            return;

        //発見時と未発見時で移動の挙動を変える
        if (_isDiscoveredPlayer)
        {
            if (_isDash)
            {
                _dashMover.Walk(_currentDir);
                _dashMover.Fall();
                _dashMover.Move();

                if (_dashMover.IsNeedTurnBack)
                    TurnBack();
            }
            else //ダッシュのクールダウン中はプレイヤーに向くだけ
                TurnToPlayer();
        }
        else
        {
            _mover.Walk(_currentDir);
            _mover.Fall();
            _mover.Move();

            if (_mover.IsNeedTurnBack || (CheckGroundState() && _mover.GroundCheck(out var hit)))
                TurnBack();
        }
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
        //初回呼び出し時は突進開始の関数を呼ぶ
        if (!_isDiscoveredPlayer)
            StartDash();

        //プレイヤー発見フラグを立てる
        _isDiscoveredPlayer = true;

        //攻撃フラグをセット
        _anim.SetFlag("IsEnemyAttack", true);
    }

    private void StartDash()
    {
        _isDash = true;

        _timer.CreateTask(SwitchDash, _param.SerchedAttackInterval);
    }

    private void SwitchDash()
    {
        _isDash = !_isDash;
        _timer.CreateTask(SwitchDash, _param.SerchedAttackInterval);
    }
}
