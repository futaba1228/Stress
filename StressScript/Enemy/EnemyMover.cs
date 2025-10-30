using Enumrator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の移動を司るクラス
/// </summary>
public class EnemyMover
{
    /// <summary>
    /// トランスフォーム
    /// </summary>
    private Transform _transform;

    /// <summary>
    /// 移動速度
    /// </summary>
    private float _moveSpeed;

    /// <summary>
    /// 移動速度
    /// </summary>
    private Vector3 _velocity;

    /// <summary>
    /// 移動に引き返しが必要
    /// </summary>
    public bool IsNeedTurnBack
    { get; private set; }

    /// <summary>
    /// 落下中か？
    /// </summary>
    private bool _isFalling = false;

    /// <summary>
    /// 地面との判定用のマスク
    /// </summary>
    private readonly LayerMask _GROUNDMASK = 1 << 6;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="transform">このインスタンスが触るトランスフォーム</param>
    /// <param name="moveSpeed">移動速度</param>
    public EnemyMover(Transform transform, float moveSpeed)
    {
        _transform = transform;
        _moveSpeed = moveSpeed;
    }

    /// <summary>
    /// 実際の座標移動を行う関数
    /// </summary>
    public void Move()
    {
        var pos = _transform.position;

        _transform.position += _velocity;

        if (CheckSide(out var hit2D))
        {
            CorrectSide(hit2D.Value);
            _transform.position = pos;
            _velocity.x = 0.0f;
            _transform.position += _velocity;
            IsNeedTurnBack = true;
        }
        else
            IsNeedTurnBack = false;

        return;

        bool CheckSide(out RaycastHit2D? hit2D)
        {
            //足元に近い位置で見るように座標を調整
            var checkPos = _transform.position;
            checkPos.y -= _transform.localScale.x * 0.4f;

            //横方向にRayを飛ばして接地をとる
            hit2D = Physics2D.Raycast(
                checkPos,                                       //原点
                _velocity.x > 0 ? Vector3.right : Vector3.left, //向き
                _transform.localScale.x * 0.5f,                 //長さ
                _GROUNDMASK);                                   //判定をとるマスク

            return hit2D.Value.collider != null;
        }
    }

    /// <summary>
    /// 横方向に速度を与える関数
    /// </summary>
    public void Walk(Direction dir)
    {
        //移動量を取得
        _velocity.x = GetMoveValue();

        return;

        //方向enumから移動量を取得する関数
        float GetMoveValue()
        {
            if (dir == Direction.UpRight || dir == Direction.Right || dir == Direction.DownRight)
                return _moveSpeed * Time.fixedDeltaTime;
            else if ((dir == Direction.UpLeft || dir == Direction.Left || dir == Direction.DownLeft))
                return -_moveSpeed * Time.fixedDeltaTime;
            else
                return 0.0f;
        }
    }

    /// <summary>
    /// 重力を適用する関数
    /// </summary>
    public void Fall()
    {
        //落下フラグを立てる
        _isFalling = true;

        //下方向の速度を与える
        _velocity.y = -_moveSpeed * Time.fixedDeltaTime;

        //接地確認
        if (!GroundCheck(out var hit2D))
            return;

        //座標修正
        CorrectGround((RaycastHit2D)hit2D);

        //縦方向の速度を０に
        _velocity.y = 0.0f;

        //設置したので落下終了
        _isFalling = false;
    }

    /// <summary>
    /// 接地を調べる関数
    /// </summary>
    /// <returns>接地している</returns>
    public bool GroundCheck(out RaycastHit2D? hit2D)
    {
        //変数割り当て
        hit2D = null;

        //縦向きの速度が正の値であれば上に移動しているハズなので
        //trueを返して終了する
        if (_velocity.y > 0)
            return false;

        //下方向にRayを飛ばして接地をとる
        hit2D = Physics2D.Raycast(
            _transform.position,                //原点
            Vector3.down,                       //向き
            _transform.localScale.y * 0.5f,     //長さ
            _GROUNDMASK);                       //判定をとるマスク

        //当たった物がnullかどうかで接地を判断
        return hit2D.Value.collider != null;
    }

    /// <summary>
    /// 左右方向の座標補正をかける関数
    /// </summary>
    public void CorrectSide(RaycastHit2D hit2D)
    {
        //Rayの衝突座標を取得
        var correctPos = _transform.position;
        //場所をを衝突地点に
        correctPos.x = hit2D.point.x;
        //自身の横幅分だけ横に
        correctPos.x +=
            _velocity.x > 0 ? -_transform.localScale.x * 0.5f : _transform.localScale.x * 0.5f;
        //修正座標に移動
        _transform.position = correctPos;
    }

    /// <summary>
    /// 高さの座標補正をかける関数
    /// </summary>
    public void CorrectGround(RaycastHit2D hit2D)
    {
        //Rayの衝突座標を取得
        var correctPos = _transform.position;
        //高さを衝突地点に
        correctPos.y = hit2D.point.y;
        //自身の高さ分だけ上に
        correctPos.y += _transform.localScale.y * 0.5f;
        //修正座標に移動
        _transform.position = correctPos;
    }
}
