using DG.Tweening;
using Enumrator;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace caramel
{
    public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
    {
        /// <summary>
        /// 生成した敵につけるID
        /// </summary>
        int _enemyID = 0;

        /// <summary>
        /// 現在登場している敵リスト
        /// </summary>
        public EnemyList ActiveEnemys
        { get; private set; }

        [SerializeField]
        [Tooltip("生成用の敵のプレハブ")]
        public GameObject _enemyPrefab;

        [SerializeField]
        [Tooltip("生成用の敵のプレハブ")]
        public SPBoss _spBossPrefab;

        [SerializeField]
        private GameObject _auraBombPrefab;

        /// <summary>
        /// 敵のパラメーター
        /// </summary>
        private Dictionary<EnemyType, EnemyParameter> _enemyParameters;

        public void Initialize()
        {
            ActiveEnemys = new();
            _enemyID = 0;
            LoadEnemyParameter();
        }

        /// <summary>
        /// 全ての敵と渡された矩形とで当たり判定をとる関数
        /// </summary>
        /// <param name="rect">判定をとりたい矩形</param>
        /// <returns>当たっていればtrue</returns>
        public bool HitJudgeByEnemys(SelfMade.Rectangle rect)
        {
            if (ActiveEnemys == null)
                return false;

            foreach (var enemy in ActiveEnemys.List)
            {
                if (enemy.Rect.HitJudge(rect))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 敵のパラメーターテーブルを取り込む関数
        /// </summary>
        private void LoadEnemyParameter()
        {
            //ファイルを取得
            var dataAsset = Addressables.LoadAssetAsync<EnemyParameterTableAsset>(SummarizeResourceDirectory.ENEMYGENERALPARAMTABLE_ASSET_PATH).WaitForCompletion();

            dataAsset.ReloadParameter();

            //入れ物を用意
            _enemyParameters = new();

            //パラメーターを格納する
            foreach (var data in dataAsset.EnemyGeneralParamTable)
            {
                var param = new EnemyParameter(data);
                _enemyParameters.Add(param.Name, param);
            }
        }

        // Update is called once per frame
        void Update()
        {
            DestroyDeadEnemys();
            ActiveEnemys?.Update();
        }

        private void FixedUpdate()
        {
            CheckDiscoverPlayer();
            CheckHitBeam();
        }

        /// <summary>
        /// 各敵がプレイヤーを見つけたか
        /// </summary>
        public void CheckDiscoverPlayer()
        {
            //現在隠密態ならば新たに発見されない
            if (!PlayerManager.Instance.Player.IsPerfectMode)
                return;

            //リストを取得
            var enemys = ActiveEnemys.List;

            if (enemys.Count < 1)
                return;

            foreach (var enemy in enemys)
            {
                if (enemy == null)
                    continue;

                //オーラ内にいたら
                if (HitManager.Instance.HitJudgePlayerAura(enemy))
                {
                    enemy.DiscoverPlayer();
                }
            }
        }

        /// <summary>
        /// 浄化が行われた際に走る処理
        /// </summary>
        public void OnClarification()
        {
            //リストを取得
            var enemys = ActiveEnemys.List;

            if (enemys.Count < 1)
                return;

            foreach (var enemy in enemys)
            {
                if (enemy == null)
                    continue;

                //オーラ内にいたら
                if (HitManager.Instance.HitJudgePlayerAuraOnClearFication(enemy))
                {
                    enemy.OnClarification();
                }
            }
        }

        /// <summary>
        /// ビームが放たれたときに走る処理
        /// </summary>
        public void CheckHitBeam()
        {
            //現在ビーム売打ってなければ当たらない
            if (!PlayerManager.Instance.Player.IsBeamMode)
                return;

            //リストを取得
            var enemys = ActiveEnemys.List;

            if (enemys.Count < 1)
                return;

            foreach (var enemy in enemys)
            {
                if (enemy == null)
                    continue;

                //ビームに当たったら浄化処理
                if (HitManager.Instance.HitJudgePlayerBeam(enemy.Rect))
                {
                    enemy.OnClarification();
                }
            }
        }

        /// <summary>
        /// 死亡フラグが立っている敵を削除する関数
        /// </summary>
        private void DestroyDeadEnemys()
        {
            if (ActiveEnemys == null)
                return;

            //リストを取得
            var enemys = ActiveEnemys.List;

            if (enemys.Count < 1)
                return;

            foreach (var enemy in enemys)
            {
                if (enemy == null) 
                    continue;

                //死亡フラグが立っていれば破棄する
                if (enemy.IsDead)
                {
                    ActiveEnemys.Remove(enemy);
                    return;
                }
            }
        }

        /// <summary>
        /// 指定されたタイプの敵を出現させる関数
        /// </summary>
        /// <param name="type">生成する敵のタイプ</param>
        /// <param name="position">生成座標</param>
        public static EnemyBase SpawnEnemy(EnemyType type, Vector3 position)
        {
            //enumから指定の型の敵を生成
            var enemy = CreateOrderedEnemy();

            //パラメーターをセット
            enemy.SetParameter(GetParameterFromType());
            //アニメーションをセット
            enemy.SetAnimation(GetAnimationFromType());
            //初期化
            enemy.Initialize();
            //リストに追加
            Instance.ActiveEnemys.Add(enemy);

            return enemy;

            //enumに応じた派生型で敵を生成
            EnemyBase CreateOrderedEnemy()
            {
                return type switch
                {
                    EnemyType.NGEnemy => CreateEnemy<NEnemy>(position),
                    EnemyType.NOEnemy => CreateEnemy<NEnemy>(position),
                    EnemyType.NPEnemy => CreateEnemy<NEnemy>(position),
                    EnemyType.SAGEnemy => CreateEnemy<SAEnemy>(position),
                    EnemyType.SAPEnemy => CreateEnemy<SAEnemy>(position),
                    EnemyType.SBGEnemy => CreateEnemy<SBEnemy>(position),
                    EnemyType.SBOEnemy => CreateEnemy<SBEnemy>(position),
                    EnemyType.SCOEnemy => CreateEnemy<SCEnemy>(position),
                    EnemyType.SCPEnemy => CreateEnemy<SCEnemy>(position),
                    EnemyType.NGBoss => CreateEnemy<NGBoss>(position),
                    EnemyType.NOBoss => CreateEnemy<NOBoss>(position),
                    EnemyType.NPBoss => CreateEnemy<NPBoss>(position),
                    EnemyType.SPBoss => CreateSPBoss(position),
                    _ => null
                };
            }

            //生成する敵のタイプに応じたパラメーターを返す
            EnemyParameter GetParameterFromType()
            {
                return Instance._enemyParameters[type];
            }

            //生成する敵のタイプに応じたアニメーションを返す
            RuntimeAnimatorController GetAnimationFromType()
            {
                return type switch
                {
                    EnemyType.NGEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NGENEMY_ANIM).WaitForCompletion(),
                    EnemyType.NOEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NOENEMY_ANIM).WaitForCompletion(),
                    EnemyType.NPEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NPENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SAGEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SAGENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SAPEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SAPENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SBGEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SBGENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SBOEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SBOENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SCOEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SCOENEMY_ANIM).WaitForCompletion(),
                    EnemyType.SCPEnemy => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SCPENEMY_ANIM).WaitForCompletion(),
                    EnemyType.NGBoss => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NGBOSS_ANIM).WaitForCompletion(),
                    EnemyType.NOBoss => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NOBOSS_ANIM).WaitForCompletion(),
                    EnemyType.NPBoss => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.NPBOSS_ANIM).WaitForCompletion(),
                    EnemyType.SPBoss => Addressables.LoadAssetAsync<RuntimeAnimatorController>(SummarizeResourceDirectory.SPBOSS_ANIM).WaitForCompletion(),
                    _ => null
                };
            }
        }

        /// <summary>
        /// 敵の生成を行う関数
        /// </summary>
        /// <typeparam name="TEnemy">生成する敵の種類</typeparam>
        /// <returns>生成した敵のインスタンス</returns>
        private static TEnemy CreateEnemy<TEnemy>(Vector3 position) where TEnemy : EnemyBase
        {
            //オブジェクトを生成
            var enemyObj = Instantiate(Instance._enemyPrefab, position, Quaternion.identity);
            //敵の描画を行う用のオブジェクトを生成
            var enemySpriteObj = Instantiate(Instance._enemyPrefab, enemyObj.transform);
            var enemySprite = enemySpriteObj.AddComponent<SpriteRendererWrapper>();
            enemySprite.Initialize();
            enemySprite.SetSortingLayer("Actor");
            //敵を動かすスクリプトをアタッチ
            var enemy = enemyObj.AddComponent<TEnemy>();
            //描画クラスをセット
            enemy.SetSprite(enemySprite);
            //生成ID付与
            SetID();
            //生成したインスタンスを返却
            return enemy;

            //生成した敵にIDを付ける
            void SetID()
            {
                //ID振り分け
                enemy.CreateID = Instance._enemyID;

                //番号は連番にするのでインクリメント
                ++Instance._enemyID;
            }
        }

        /// <summary>
        /// 敵の弾の生成を行う関数
        /// </summary>
        /// <typeparam name="TEnemyBullet">生成する敵の種類</typeparam>
        /// <returns>生成した敵のインスタンス</returns>
        public static TEnemyBullet CreateEnemyBullet<TEnemyBullet>(EnemyType enemyType, Vector3 position, Vector3 moveDir) where TEnemyBullet : EnemyBullet
        {
            //オブジェクトを生成
            var enemyObj = Instantiate(Instance._enemyPrefab, position, Quaternion.identity);
            //敵の描画を行う用のオブジェクトを生成
            var enemySpriteObj = Instantiate(Instance._enemyPrefab, enemyObj.transform);
            var enemySprite = enemySpriteObj.AddComponent<SpriteRendererWrapper>();
            enemySprite.Initialize();
            enemySprite.SetSortingLayer("Actor");
            enemySprite.SetSprite(GetBulletSprite());
            //敵を動かすスクリプトをアタッチ
            var enemyBullet = enemyObj.AddComponent<TEnemyBullet>();
            //描画クラスをセット
            enemyBullet.SetSprite(enemySprite);
            //移動方向を設定
            enemyBullet.SetMoveDir(moveDir);
            //生成ID付与
            SetID();
            //初期化
            enemyBullet.Initialize();
            //プレイヤーに向く
            enemyBullet.TurnToPlayer(BulletRotateBy());
            //リストに追加
            Instance.ActiveEnemys.Add(enemyBullet);
            //生成したインスタンスを返却
            return enemyBullet;

            //生成した敵にIDを付ける
            void SetID()
            {
                //ID振り分け
                enemyBullet.CreateID = Instance._enemyID;

                //番号は連番にするのでインクリメント
                ++Instance._enemyID;
            }

            Sprite GetBulletSprite()
            {
                var path = GetSpritePath();

                try
                {
                    //アセットをTexture2Dとしてロード
                    Texture2D texture2D = Addressables.LoadAssetAsync<Texture2D>(path).WaitForCompletion();
                    //Texture2DからSpriteクラスを生成
                    var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                        new Vector2(0.5f, 0.5f), 250f);
                    //生成した画像を返却
                    return sprite;
                }
                catch (Exception e)
                {
                    //ログにエラーを書いて終了
                    Debug.LogError(e.Message);
                    return null;
                }

                string GetSpritePath()
                {
                    return enemyType switch
                    {
                        EnemyType.SAGEnemy => SummarizeResourceDirectory.SAGENEMYBULLET_SPRITE,
                        EnemyType.SAPEnemy => SummarizeResourceDirectory.SAPENEMYBULLET_SPRITE,
                        EnemyType.NGBoss => SummarizeResourceDirectory.NGBOSSBULLET_SPRITE,
                        EnemyType.NPBoss => SummarizeResourceDirectory.NPBOSSBULLET_SPRITE,
                        EnemyType.SPBoss => SummarizeResourceDirectory.SPBOSSBULLET_SPRITE,
                        _ => null
                    };
                }
            }

            Vector3 BulletRotateBy()
            {
                return enemyType switch
                {
                    EnemyType.SAGEnemy => -Vector3.right,
                    EnemyType.SAPEnemy => -Vector3.right,
                    EnemyType.NGBoss => -Vector3.up,
                    EnemyType.NPBoss => -Vector3.up,
                    _ => Vector3.zero
                };
            }
        }

        /// <summary>
        /// 敵の生成を行う関数
        /// </summary>
        /// <typeparam name="TEnemy">生成する敵の種類</typeparam>
        /// <returns>生成した敵のインスタンス</returns>
        private static SPBoss CreateSPBoss(Vector3 position)
        {
            //オブジェクトを生成
            var spBoss = Instantiate(Instance._spBossPrefab, position, Quaternion.identity);
            //生成ID付与
            SetID();
            //生成したインスタンスを返却
            return spBoss;

            //生成した敵にIDを付ける
            void SetID()
            {
                //ID振り分け
                spBoss.CreateID = Instance._enemyID;

                //番号は連番にするのでインクリメント
                ++Instance._enemyID;
            }
        }

        public void CreateBombAura(Vector3 postion)
        {
            var bomb = Instantiate(_auraBombPrefab, postion, Quaternion.identity);

            bomb.transform.DOScale(50, 3).SetUpdate(true).SetEase(Ease.InCubic);

            Camera.main.transform.DOShakePosition(2, 1, 20);
        }

        public void DestroyAllEnemy()
        {
            if (ActiveEnemys.List.Count < 1)
                return;

            foreach (var enemy in ActiveEnemys.List.ToArray())
                Destroy(enemy.gameObject);

            ActiveEnemys = new();
        }
    }
}