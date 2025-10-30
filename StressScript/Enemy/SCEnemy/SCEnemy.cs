using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCEnemy : EnemyBase
{
    /// <summary>
    /// 落下突進用の移動クラス
    /// </summary>
    private EnemyMover _fallMover;

    /// <summary>
    /// デフォルトの高さ
    /// </summary>
    private float _defaultHeight;

    /// <summary>
    /// 上昇中か？
    /// </summary>
    private bool _isMoveUp = false;

    /// <summary>
    /// 落下中か？
    /// </summary>
    private bool _isFalling = false;

    /// <summary>
    /// 振り返る動作の予約番号
    /// </summary>
    private int _turnTaskID = -1;

    private void Start()
    {
        _fallMover = new(this.transform, _param.MoveSpeed);

        _turnTaskID = _timer.CreateTask(TurnBack, _param.AttackInterval);

        _defaultHeight = transform.position.y;
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
            if (_isFalling)
            {
                _fallMover.Fall();
                _fallMover.Move();
            }
            else if (_isMoveUp)
                MoveToUp();
            else
            {
                _mover.Walk(_currentDir);
                _mover.Move();

                if (_mover.IsNeedTurnBack)
                    TurnBack();
            }

            TurnToPlayer();
        }
        else
        {
            _mover.Walk(_currentDir);
            _mover.Move();

            if (_mover.IsNeedTurnBack)
                TurnBack();
        }
    }

    private void MoveToUp()
    {
        var pos = this.transform.position;
        pos.y += _param.MoveSpeed * Time.fixedDeltaTime;
        if (pos.y > _defaultHeight)
        {
            pos.y = _defaultHeight;
            _isMoveUp = false;
            _timer.CreateTask(StartFall, _param.SerchedAttackInterval);
        }
        this.transform.position = pos;
    }

    /// <summary>
    /// 反対に向き直る関数
    /// </summary>
    private void TurnBack()
    {
        _currentDir = _currentDir == Direction.Right ? Direction.Left : Direction.Right;
        _sprite.SetFlipX(!_sprite.FlipX);

        if (_timer.GetTaskFromID(_turnTaskID) == null)
            _timer.CanncellTask(_turnTaskID);

        _turnTaskID = _timer.CreateTask(TurnBack, _param.AttackInterval);
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
            StartFall();

        //プレイヤー発見フラグを立てる
        _isDiscoveredPlayer = true;

        //攻撃フラグをセット
        _anim.SetFlag("IsEnemyAttack", true);
    }

    private void StartFall()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SCEnemyAttack);

        _isFalling = true;

        _isMoveUp = true;

        _timer.CreateTask(() => _isFalling = false, _param.SerchedAttackInterval);
    }
}
