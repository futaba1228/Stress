using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵アニメーション管理クラス
/// </summary>
public class EnemyAnimationController
{
    /// <summary>
    /// アニメーション管理クラス
    /// </summary>
    private Animator _anim;

    /// <summary>
    /// 名前とハッシュ値を結びつけて管理する連想配列
    /// </summary>
    private Dictionary<string, int> _parameters;

    /// <summary>
    /// 攻撃フラグのハッシュ値
    /// </summary>
    private int _isEnemyAttackHash;

    /// <summary>
    /// 死亡フラグ用のハッシュ値
    /// </summary>
    private int _isEnemyDieHash;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="animator"></param>
    public EnemyAnimationController(Animator animator)
    {
        //アニメーター登録
        _anim = animator;
        //ハッシュ値を取得
        GetHash();

        void GetHash()
        {
            var parameters = _anim.parameters;

            _parameters = new();

            foreach (var param in parameters)
                _parameters.Add(param.name, Animator.StringToHash(param.name));
        }
    }

    /// <summary>
    /// フラグをセットする関数
    /// </summary>
    public void SetFlag(string name, bool isAttack)
    {
        if (!_parameters.ContainsKey(name))
            return;

        _anim.SetBool(_parameters[name], isAttack);
    }

    /// <summary>
    /// フラグをセットする関数
    /// </summary>
    public void SetTrigger(string name)
    {
        if (!_parameters.ContainsKey(name))
            return;

        _anim.SetTrigger(_parameters[name]);
    }

    /// <summary>
    /// 実数をセットする関数
    /// </summary>
    public void SetFloat(string name, float value)
    {
        if (!_parameters.ContainsKey(name))
            return;

        _anim.SetFloat(_parameters[name], value);
    }
}
