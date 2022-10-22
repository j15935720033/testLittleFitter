using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField, Header("攻擊資料")]
        internal DataAttack dataAttack;
        private ObjectPoolBullet_Deep objectPoolBullet;


        private void Awake()
        {
            objectPoolBullet = FindObjectOfType<ObjectPoolBullet_Deep>();
        }
        #region 2個要碰撞物件一個勾 is Trigger
        //2個物件碰撞執行一次
        private void OnTriggerEnter2D(Collider2D collision)
        {

            //print("碰到物體名子" + collision.gameObject.name);
            //print("碰到tag名子" + collision.gameObject.tag);

            switch (dataAttack.whoAttack)
            {
                case WhoAttack.playerAttack://玩家攻擊
                    //如果碰到敵人 
                    if (collision.gameObject.tag == "Enemy")
                    {

                        switch (dataAttack.attackKind)
                        {
                            case AttackKind.attack:
                                //print("普攻");
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);//取得碰到物件上的HealthSystem，並傳攻擊力
                                break;
                            case AttackKind.skill01:
                                //print("有上飛攻擊");
                                collision.gameObject.GetComponent<HealthSystem>().InjuriedUp(dataAttack.skill01);
                                break;
                            case AttackKind.skill02:
                                //print("子彈攻擊");
                                //用BulletController寫
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);
                                print("玩家子彈打到敵人");
                                objectPoolBullet.ReleasePoolObject(this.gameObject);
                                break;
                            case AttackKind.skill03:
                                //跑步攻擊
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.skill03);
                                break;
                            default:
                                break;
                        }

                    }
                    break;
                case WhoAttack.enemyAttack://敵人攻擊
                    //如果碰到玩家
                    if (collision.gameObject.tag == "Player")
                    {

                        switch (dataAttack.attackKind)
                        {
                            case AttackKind.attack:
                                //print("普攻");
                                //print("attackSystem_普攻  " + Time.time);
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);//取得碰到物件上的HealthSystem，並傳攻擊力
                                break;
                            case AttackKind.skill01:
                                //print("有上飛攻擊");
                                collision.gameObject.GetComponent<HealthSystem>().InjuriedUp(dataAttack.skill01);
                                break;
                            case AttackKind.skill02:
                                //print("子彈攻擊");
                                //用BulletController寫
                                break;
                            case AttackKind.skill03:
                                //跑步攻擊
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.skill03);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            

        }

        #endregion
    }
}