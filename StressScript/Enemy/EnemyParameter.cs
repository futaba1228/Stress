using Enumrator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 実際に使うパラメータークラス
/// </summary>
public class EnemyParameter
{
    /// <summary>
    /// 識別番号
    /// </summary>
    public int ID
    { get; private set; }

    /// <summary>
    /// 識別名
    /// </summary>
    public EnemyType Name
    { get; private set; }

    /// <summary>
    /// 体力
    /// </summary>
    public int Life
    { get; private set; }

    /// <summary>
    /// 攻撃の頻度(未発見時)
    /// </summary>
    public float AttackInterval
    { get; private set; }

    /// <summary>
    /// 攻撃の頻度(発見時)
    /// </summary>
    public float SerchedAttackInterval
    { get; private set; }

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed
    { get; private set; }

    /// <summary>
    /// 横幅
    /// </summary>
    public float Width
    { get; private set; }

    /// <summary>
    /// 縦幅
    /// </summary>
    public float Height
    { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="setParam">このインスタンスに持たせるパラメータ</param>
    public EnemyParameter(EnemyParameterReciever setParam)
    {
        Encapsulation(setParam);
    }

    /// <summary>
    /// パラメータのカプセル化を行う関数
    /// </summary>
    public void Encapsulation(EnemyParameterReciever setParam)
    {
        ID = setParam.ID;
        Name = Enum.Parse<EnemyType>(setParam.Name);
        Life = setParam.Life;
        AttackInterval = setParam.AttackInterval;
        SerchedAttackInterval = setParam.SerchedAttackInterval;
        MoveSpeed = setParam.MoveSpeed;
        Width = setParam.Width;
        Height = setParam.Height;
    }
}
