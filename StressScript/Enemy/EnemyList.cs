using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 敵管理専用のリストラッパークラス
/// </summary>
public class EnemyList
{
    /// <summary>
    /// 現在アクティブな敵リスト
    /// </summary>
    public List<EnemyBase> List
    { get; private set; }

    /// <summary>
    /// 登録待ちの敵
    /// </summary>
    private Queue<EnemyBase> _reservedEenemys;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EnemyList()
        => Initialize();

    public void Initialize()
    {
        List = new();
        _reservedEenemys = new();
    }

    /// <summary>
    /// 状態の更新を行う関数
    /// </summary>
    public void Update()
    {
        if (_reservedEenemys.Count < 1)
            return;

        //登録待ちをリストに格納する
        AddList();

        // リスト登録待ちしている敵を実際にリストに格納する関数
        void AddList()
        {
            //もし登録待ちがなければ終了
            if (_reservedEenemys.Count < 1)
                return;

            //キューから取り出して格納
            List.Add(_reservedEenemys.Dequeue());

            //予約がなくなるまで繰り返す
            AddList();
        }
    }

    /// <summary>
    /// 要素の追加を行う関数
    /// </summary>
    /// <param name="enemy"></param>
    public void Add(EnemyBase enemy)
        => _reservedEenemys.Enqueue(enemy);

    /// <summary>
    /// 要素の削除を行う関数
    /// </summary>
    /// <param name="enemy">削除したい敵</param>
    public void Remove(EnemyBase enemy)
    {
        //そもそも含まれてないならここで終了
        if (!List.Contains(enemy))
            return;

        //要素を削除
        List.Remove(enemy);
    }

    /// <summary>
    /// 生成IDから敵を返す関数
    /// </summary>
    /// <param name="createID">欲しい敵の生成ID</param>
    public EnemyBase GetEnemy(int createID)
        => List.Single(enemy => enemy.CreateID == createID);
}
