using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の体力管理クラス
/// </summary>
public class EnemyLife
{
    /// <summary>
    /// 現在の体力
    /// </summary>
    public int CurrentLife
    { get; private set; }

    /// <summary>
    /// 体力の最大値
    /// </summary>
    private int _maxLife;

    /// <summary>
    /// 生きているか
    /// </summary>
    public bool IsArrive
        => CurrentLife > 0;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="life">この敵の体力値</param>
    public EnemyLife(int life)
    {
        CurrentLife = life;
        _maxLife = life;
    }

    /// <summary>
    /// ダメージをもらう処理
    /// </summary>
    public void TakeDamage(int value = 1)
    {
        CurrentLife -= value;

        //最低値(0)を下回ったら補正
        if (CurrentLife < 0)
            CurrentLife = 0;

        //最大値を上回ったら補正
        if (CurrentLife > _maxLife)
            CurrentLife = _maxLife;
    }
}
