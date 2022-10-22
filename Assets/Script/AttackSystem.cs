using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField, Header("�������")]
        internal DataAttack dataAttack;
        private ObjectPoolBullet_Deep objectPoolBullet;


        private void Awake()
        {
            objectPoolBullet = FindObjectOfType<ObjectPoolBullet_Deep>();
        }
        #region 2�ӭn�I������@�Ӥ� is Trigger
        //2�Ӫ���I������@��
        private void OnTriggerEnter2D(Collider2D collision)
        {

            //print("�I�쪫��W�l" + collision.gameObject.name);
            //print("�I��tag�W�l" + collision.gameObject.tag);

            switch (dataAttack.whoAttack)
            {
                case WhoAttack.playerAttack://���a����
                    //�p�G�I��ĤH 
                    if (collision.gameObject.tag == "Enemy")
                    {

                        switch (dataAttack.attackKind)
                        {
                            case AttackKind.attack:
                                //print("����");
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);//���o�I�쪫��W��HealthSystem�A�öǧ����O
                                break;
                            case AttackKind.skill01:
                                //print("���W������");
                                collision.gameObject.GetComponent<HealthSystem>().InjuriedUp(dataAttack.skill01);
                                break;
                            case AttackKind.skill02:
                                //print("�l�u����");
                                //��BulletController�g
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);
                                print("���a�l�u����ĤH");
                                objectPoolBullet.ReleasePoolObject(this.gameObject);
                                break;
                            case AttackKind.skill03:
                                //�]�B����
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.skill03);
                                break;
                            default:
                                break;
                        }

                    }
                    break;
                case WhoAttack.enemyAttack://�ĤH����
                    //�p�G�I�쪱�a
                    if (collision.gameObject.tag == "Player")
                    {

                        switch (dataAttack.attackKind)
                        {
                            case AttackKind.attack:
                                //print("����");
                                //print("attackSystem_����  " + Time.time);
                                collision.gameObject.GetComponent<HealthSystem>().Injuried(dataAttack.attack);//���o�I�쪫��W��HealthSystem�A�öǧ����O
                                break;
                            case AttackKind.skill01:
                                //print("���W������");
                                collision.gameObject.GetComponent<HealthSystem>().InjuriedUp(dataAttack.skill01);
                                break;
                            case AttackKind.skill02:
                                //print("�l�u����");
                                //��BulletController�g
                                break;
                            case AttackKind.skill03:
                                //�]�B����
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