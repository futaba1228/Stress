using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一旦仮作成
/// </summary>
public class EnemyBullet : EnemyBase
{
    private float _lifeTime = 5.0f;

    private float _speed = 4.0f;

    private Vector3 _moveDir;

    private void Start()
    {
        _life = new(1);
        _timer.CreateTask(OnDead, _lifeTime);        
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// プレイヤーの方を向く関数
    /// </summary>
    public void TurnToPlayer(Vector3 rotateBy)
    {
        //プレイヤーの方を向く
        Vector3 toDirection = PlayerManager.Instance.Player.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(rotateBy, toDirection);
    }

    public void SetMoveDir(Vector3 moveDir)
        => _moveDir = moveDir;

    public void Move()
    {
        this.transform.position += _moveDir * _speed * Time.fixedDeltaTime;
    }

    public override void DiscoverPlayer()
    {
        
    }

    public override void OnClarification()
    {
        _life.TakeDamage();

        if (_life.IsArrive)
            return;

        OnDead();

        this.gameObject.SetActive(false);
    }

}
