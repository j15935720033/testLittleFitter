using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;//引用AudioMixer程式庫
using System;//有用亂數
using UnityEngine.UI;
using BehaviorTree;
using System.Collections.Generic;
namespace chia
{

    public class Role_Deep_AI : Role
    {

        //AI_TaskPatrol(隨機遊走)
        private Node _root = null;
        [SerializeField, Header("固定遊走位置")]
        private Transform[] waypoints;
        [SerializeField, Header("AI移動速度")]
        private  float aiSpeed = 12f;
        private int _currentWaypointIndex = 0;
        private float _waitTime = 2f; //站著等待時間
        private float _waitCounter = 0f;
        private bool _waiting = false;
        private Vector3 wp;//隨機走位置
        private int[] arrX = new int[] { -42, -32, -22, -12, -02, 2, 12, 22, 32, 42 };//隨機走X範圍
        private int[] arrY = new int[] { -13, -03, 3, 13, 23 };//隨機走Y範圍
        //AI_CheckEnemyInFOVRange
        private float fovRange = 21f; //FOV(field of view):視野
        private int PlayerMask;
        //AI_TaskAttack
        private float attackRange = 1f; //攻擊範圍
        [SerializeField, Header("攻擊間隔")]
        private float attackTime = 0.5f;
        private float attackCounter = 0f;//攻擊計時器
        private System.Random rand_AttackType;
        private int attackType;//由random產生
        //AI用到
        protected GameObject targetPlayer;//目標玩家
        protected Transform[] targetPlayer_transforms;
        protected PlayerHealth targetPlayerHealth;//玩家腳本
        protected Collider2D targetPlayerCollider2D;
        //子彈用
        protected ObjectPoolBullet_Deep objectPoolBullet;

        protected Dictionary<string, Transform> dictionary_TargetPlayer_Child = new Dictionary<string,Transform>();
        protected override void Awake()
        {
            
            base.Awake();
            PlayerMask = LayerMask.GetMask("Player");//抓取玩家Layer
            //AI用到
            targetPlayer = GameObject.Find("Deep_Player");
            targetPlayer_transforms = targetPlayer.GetComponentsInChildren<Transform>(true);//取得放此腳本的GameObject，所有子物件(包含自己)
            targetPlayerHealth = targetPlayer.GetComponent<PlayerHealth>();
            targetPlayerCollider2D = targetPlayer.GetComponent<Collider2D>();
            objectPoolBullet = FindObjectOfType<ObjectPoolBullet_AI>();//取得子彈物件池
            rand_AttackType = new System.Random();
            for (int i = 0; i < targetPlayer_transforms.Length; i++)
            {
                dictionary_TargetPlayer_Child.Add(targetPlayer_transforms[i].name, targetPlayer_transforms[i]);
            }
        }
        protected override void Start()
        {
            base.Start();
            attackType = rand_AttackType.Next(0, 2);//什麼樣的攻擊
            //AI_建樹
            _root = SetupTree();
        }
        protected override void Update()
        {
            base.Update();
            attackType=rand_AttackType.Next(0,2);//什麼樣的攻擊
            //print("<Color=red>attackType"+ attackType +"</Color>");
            //AI_控制
            if (_root != null)
                _root.Evaluate();
        }
       
        #region 自訂方法_AI
        /// <summary>
        /// 建立樹
        /// </summary>
        /// <returns></returns>
        protected Node SetupTree()
        {
            Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new Leaf("CheckPlayerInAttackCollider",CheckPlayerInAttackCollider),//判斷是否進入攻擊範圍
                new Leaf("CheckCanAttack",CheckCanAttack),
                new Selector(new List<Node>
                {
                     new Leaf("TaskAttack_Skill01",TaskAttack_Skill01),//skill01破空斬
                     new Leaf("TaskAttack",TaskAttack)//普攻
                })
            }),
            new Sequence(new List<Node>
            {   
                new Leaf ("CheckPlayerAlive",CheckPlayerAlive),//如果玩家還活著，設定攻擊目標
                //new Leaf("CheckEnemyInFOVRange",CheckPlayerInFOVRange),//檢查玩家是否在碰撞內，設定攻擊目標
                new Leaf("Check_TaskGoToTarget",Check_TaskGoToTarget),//AI受傷、AI攻擊時、目標上飛中不能移動
                new Leaf("TaskGoToTarget",TaskGoToTarget)//走向玩家
            }),
            new Sequence(new List<Node>
            {
                new Leaf ("Check_TaskPatrol",Check_TaskPatrol),//檢查是否能遊走
                //new Leaf("TaskPatrol",TaskPatrol_FixPosition)//隨機遊走_固定位置
                new Leaf("TaskPatrol",TaskPatrol_RandomPosition)//隨機遊走_隨機位置
            })

        });

            return root;
        }

        /// <summary>
        /// 檢查是否能隨機遊走(被打到時或還在攻擊動畫時不能遊走)
        /// </summary>
        /// <returns></returns>
        private NodeState Check_TaskPatrol(Node node)
        {
            if (stateInjuried_Insitu || stateInjuriedUp)//原地受傷 || 上飛受傷  就不能隨機遊走
            {
                return NodeState.FAILURE;
            }
            else if (animator.GetBool(parAttack1)|| animator.GetBool(parAttack2) ||animator.GetBool(parSkill01))//攻擊動畫  就不能隨機遊走
            {
                return NodeState.FAILURE;
            }
            else
            {
                return NodeState.SUCCESS;
            }
        }
        /// <summary>
        /// 隨機遊走_固定位置
        /// </summary>
        /// <returns></returns>
        private NodeState TaskPatrol_FixPosition(Node node)
        {
            if (_waiting)//站著等待
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                    animator.SetBool("Walk", true);
                }
            }
            else//遊走特定位置
            {
                Transform wp = waypoints[_currentWaypointIndex];
                if (Vector3.Distance(transform.position, wp.position) < 0.01f)//到達指定位置:當前位置 和要移動到的位置<0.01 就站著等待
                {
                    transform.position = wp.position;
                    _waitCounter = 0f;
                    _waiting = true;

                    _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
                    animator.SetBool("Walk", false);
                }
                else//未到達指定位置 繼續走
                {

                    //UnityEngine.MonoBehaviour.print(_transform.rotation);
                    //轉向
                    if (wp.position.x - transform.position.x > 0)
                    {
                        //人物朝右
                        transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                    }
                    else
                    {   //人物朝左
                        transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                    }

                    //移動
                    transform.position = Vector3.MoveTowards(transform.position, wp.position, this.aiSpeed * Time.deltaTime);
                    animator.SetBool("Walk", true);
                }
            }
            return NodeState.RUNNING;
        }

        /// <summary>
        /// 隨機遊走_隨機位置
        /// </summary>
        /// <returns></returns>
        private NodeState TaskPatrol_RandomPosition(Node node)
        {
            if (_waiting)//站著等待
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                    animator.SetBool("Walk", true);
                }
            }
            else//隨機遊走
            {
                //print(random.Next(1,3));//1、2
                //arrX[random.Next(arrX.Length - 1)]//隨機亂數指定值

                if (this.wp == null)
                {
                    this.wp = new Vector3(arrX[random.Next(arrX.Length - 1)], arrY[random.Next(arrY.Length - 1)], transform.position.z);
                }



                if (Vector3.Distance(transform.position, wp) < 0.01f)////到達指定位置:當前位置 和要移動到的位置<0.01 就站著等待
                {
                    this.wp = new Vector3(arrX[random.Next(arrX.Length - 1)], arrY[random.Next(arrY.Length - 1)], transform.position.z);
                    //_transform.position = wp;
                    _waitCounter = 0f;
                    _waiting = true;

                    animator.SetBool("Walk", false);
                }
                else//未到達指定位置 繼續走
                {
                    //UnityEngine.MonoBehaviour.print(_transform.rotation);
                    //轉向
                    if (this.wp.x - transform.position.x > 0)
                    {
                        //人物朝右
                        transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                    }
                    else
                    {   //人物朝左
                        transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                    }
                    //移動
                    animator.SetBool("Walk", true);
                    transform.position = Vector3.MoveTowards(transform.position, this.wp, this.aiSpeed * Time.deltaTime);

                }
            }
            return NodeState.RUNNING;
        }

        /// <summary>
        /// 如果玩家還活著，設定攻擊目標
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState CheckPlayerAlive(Node node)
        {
            object t = node.GetData("targetAlive");//設定攻擊目標
            if (t == null)
            {
                if (targetPlayerHealth.Hp > 0)//如果玩家還活著
                {
                    //print("設定玩家位置"+ this.targetPlayer.transform.position);
                    node.parent.parent.SetData("targetAlive", this.targetPlayer);
                    return NodeState.SUCCESS;
                }
                return NodeState.FAILURE;
            }
            return NodeState.SUCCESS;
        }
        /// <summary>
        /// 用碰撞偵測，偵測玩家位置，並設定攻擊目標
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState CheckPlayerInFOVRange(Node node)
        {
            object t = node.GetData("target");//並設定攻擊目標
            if (t == null)
            {
                //UnityEngine.MonoBehaviour.print("_enemyLayerMask" + _enemyLayerMask);

                //用碰撞偵測，偵測玩家位置
                Collider2D[] colliders = Physics2D.OverlapCircleAll(
                    transform.position, this.fovRange, PlayerMask);

                if (colliders.Length > 0)
                {
                    print("碰到玩家");
                    node.parent.parent.SetData("target", colliders[0].transform);
                    return NodeState.SUCCESS;
                }

                return NodeState.FAILURE;
            }

            return NodeState.SUCCESS;
        }

        /// <summary>
        /// 檢查是否能走向玩家被打或攻擊時不能走路
        /// </summary>
        /// <returns></returns>
        private NodeState Check_TaskGoToTarget(Node node)
        {
                 object t = node.GetData("targetAlive");//設定攻擊目標
                Role targetScriptPlayer=((GameObject)t).GetComponent<Role>();

                if (stateInjuried_Insitu || stateInjuriedUp || targetScriptPlayer.stateInjuriedUp)//自己(AI)原地受傷 || 自己(AI)上飛受傷 || 目標上飛受傷
                {
                    return NodeState.FAILURE; print("判斷受傷" + Time.time);
                }
                else
                {
                    return NodeState.SUCCESS;
                }
        }

        /// <summary>
        /// 朝向目標走過去
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState TaskGoToTarget(Node node)
        {
            //Transform target = (Transform)node.GetData("target");//配合CheckPlayerInFOVRange方法
            GameObject target = (GameObject)node.GetData("targetAlive");//配合CheckPlayerAlive

            //Deep_AI_偵測碰撞
            int tempIndex = 0;//存放找到的GameObject
            GameObject tempGameObject_aiWeapon = null;
            bool bool_TouchPlayer;

            for (int i = 0; i < tempTrasforms.Length; i++)
            {
                if (tempTrasforms[i].name == "Deep_AI_偵測碰撞")
                {
                    tempGameObject_aiWeapon = tempTrasforms[i].gameObject;
                }
            }


          
            


            #region 位置測試
            //print("transform" + transform.position);
            //print("tempGameObject_aiWeapon"+ tempGameObject_aiWeapon.transform.position);
            //print("tempGameObject_localPosition" + tempGameObject_aiWeapon.transform.localPosition);
            //print("相加位置"+(transform.position + tempGameObject_aiWeapon.transform.localPosition));

            //原本子物件世界座標 = 父物件世界座標 + 子物件世界座標
            //因為父物件以Y軸轉180度:
            //所以子物件世界座標.X = 父物件世界座標.X - 子物件世界座標.X
            //所以子物件世界座標.Y = 父物件世界座標.Y + 子物件世界座標.Y
            //所以子物件世界座標.Z = 父物件世界座標.Z - 子物件世界座標.Z
            #endregion



            //移動
            //玩家在AI在左邊
            if (transform.position.x - target.transform.position.x >= 0)
            {
                //AI人物朝左
                transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                //aiWeaponWord:  武器的世界座標 = ai人物的世界座標  +或-(根據有無rotation) ai武器相對位置
                Vector3 WeaponWord = new Vector3(
                     transform.position.x - tempGameObject_aiWeapon.transform.localPosition.x,
                     transform.position.y + tempGameObject_aiWeapon.transform.localPosition.y,
                     transform.position.z - tempGameObject_aiWeapon.transform.localPosition.z
                     );

                //武器的世界座標(WeaponWord )  走向玩家右邊碰撞器(Player_碰撞器右邊)
                Vector3 tempDis = Vector3.MoveTowards(
                     WeaponWord, dictionary_TargetPlayer_Child["Deep_偵測碰撞"].position, this.aiSpeed * Time.deltaTime);

                //aiWord:人物的世界座標 = 移動後的武器的世界座標 +或-(根據有無rotation) ai武器相對位置
                Vector3 aiWord = new Vector3(
                tempDis.x + tempGameObject_aiWeapon.transform.localPosition.x,
                tempDis.y - tempGameObject_aiWeapon.transform.localPosition.y,
                tempDis.z + tempGameObject_aiWeapon.transform.localPosition.z
                );
                //print("移動動畫 " + Time.time);
                animator.SetBool("Walk", true);//走路動畫
                transform.position = aiWord;//aiWord:人物的世界座標更新
            }
            else if (transform.position.x - target.transform.position.x < 0)
            {
                //人物朝右
                transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                ///////-----------
                //aiWeaponWord:  武器的世界座標 = ai人物的世界座標  +或-(根據有無rotation) ai武器相對位置
                Vector3 WeaponWord = new Vector3(
                     transform.position.x + tempGameObject_aiWeapon.transform.localPosition.x,
                     transform.position.y + tempGameObject_aiWeapon.transform.localPosition.y,
                     transform.position.z + tempGameObject_aiWeapon.transform.localPosition.z
                     );

                //武器的世界座標(WeaponWord )  走向玩家右邊碰撞器(Player_碰撞器左邊)
                Vector3 tempDis = Vector3.MoveTowards(
                     WeaponWord, dictionary_TargetPlayer_Child["Deep_偵測碰撞"].position, this.aiSpeed * Time.deltaTime);

                //aiWord: 人物的世界座標 = 移動後的武器的世界座標 +或-(根據有無rotation) ai武器相對位置
                Vector3 aiWord = new Vector3(
                tempDis.x - tempGameObject_aiWeapon.transform.localPosition.x,
                tempDis.y - tempGameObject_aiWeapon.transform.localPosition.y,
                tempDis.z - tempGameObject_aiWeapon.transform.localPosition.z
                );
                //print("移動動畫 " + Time.time);
                animator.SetBool("Walk", true);//走路動畫
                transform.position = aiWord;//aiWord:人物的世界座標更新
            }
            return NodeState.RUNNING;
        }
        /// <summary>
        /// AI武器是否能碰到玩家
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
       private NodeState CheckPlayerInAttackCollider(Node node)
        {
            object t = node.GetData("targetAlive");
            //If it doesn’t find a target, it fails
            if (t == null)
            {
                return NodeState.FAILURE;
            }

            Transform target = ((GameObject)t).transform;

            //Deep_AI_偵測碰撞
            int tempIndex = 0;//存放找到的GameObject
            GameObject tempGameObject_aiWeapon=null;
            bool bool_TouchPlayer;
            Collider2D[] tempCollider2D = null;

            for (int i = 0; i < tempTrasforms.Length; i++)
            {
                if (tempTrasforms[i].name == "Deep_AI_偵測碰撞")
                {
                    tempGameObject_aiWeapon = tempTrasforms[i].gameObject;
                    tempIndex = i;
                    //用攻擊範圍判定有無碰到玩家
                    //tempCollider2D = Physics2D.OverlapCircleAll(tempGameObject_aiWeapon.transform.position, attackRange, PlayerMask);
                }
            }
            

            //用武器Collider判定有無碰到玩家
            //AI子物件_武器物件是否碰到玩家
            bool_TouchPlayer = tempGameObject_aiWeapon.GetComponent<Collider2D>().IsTouching(targetPlayerCollider2D);
            /*if (bool_TouchPlayer)
            {
                return NodeState.SUCCESS;
            }
            */

            //用攻擊範圍判定有無碰到玩家
            /*if (tempCollider2D.Length>0)
            {
                print("攻擊範圍碰到"+ tempCollider2D[0].name);
                return NodeState.SUCCESS;
            }*/

            //用距離判定有無碰到玩家
            /*if (Vector3.Distance(tempGameObject_aiWeapon.transform.position, target.position)< 0.1f)
            {
                return NodeState.SUCCESS;
            }*/
            //用X,Y距離判定距離，能否攻擊
            if (Mathf.Abs(tempGameObject_aiWeapon.transform.position.y - dictionary_TargetPlayer_Child["Deep_偵測碰撞"].position.y) < 0.1f
                &&
                Mathf.Abs(tempGameObject_aiWeapon.transform.position.x - dictionary_TargetPlayer_Child["Deep_偵測碰撞"].position.x) < 1f
                )
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }

        /// <summary>
        /// 非受傷狀態才能進入攻擊
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState CheckCanAttack(Node node)
        {
            if (!stateInjuried)//AI 非受傷狀態才能進入攻擊狀態
            {
               // print("能攻擊");
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }

        /// <summary>
        /// 普攻
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState TaskAttack(Node node)
        {
            //print("普攻"+ attackType);
            if (attackType == 0)//是0才普攻
            {
                animator.SetBool("Walk", false);
                //print(random.Next(2));//0、1
                //print(random.Next(1,3));//1、2
                i01 = random.Next(1, 3);//隨機攻擊動作
                int tempIndex = 0;//存放找到的GameObject

                this.attackCounter += Time.deltaTime;
                if (this.attackCounter >= this.attackTime)//攻擊間隔
                {
                    for (int i = 0; i < tempTrasforms.Length; i++)//找子物件
                    {
                        if (tempTrasforms[i].name == "Deep_AI_普攻")
                        {
                            tempIndex = i;
                            if (!stateAttack)//不是普通攻擊狀態(不然一直按會一直攻擊，攻擊動畫還沒做完就又攻擊)
                            {
                                stateAttack = true;//普通攻擊狀態
                                if (i01 == 1)
                                {
                                    //Debug.Log("Attack1"+temp);
                                    canmove = false;
                                    animator.SetBool(parAttack1, true);//攻擊動畫_開
                                                                       //print("攻擊動畫_開" + Time.time);
                                    this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//開啟Deep_普攻物件
                                }
                                else
                                {
                                    //Debug.Log("Attack2"+temp);
                                    canmove = false;
                                    animator.SetBool(parAttack2, true);//攻擊動畫_開
                                                                       //print("攻擊動畫_開" + Time.time);
                                    this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//開啟Deep_普攻物件
                                }
                                aud.PlayOneShot(deep_sf0);//音效
                                StartCoroutine(waitAttack_AI(tempIndex, node));//等0.5秒關閉攻擊動畫
                                

                            }
                        }
                    }

                }


                return NodeState.RUNNING;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }

        /// <summary>
        /// 防↑攻，skill01
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeState TaskAttack_Skill01(Node node)
        {
            if (attackType == 1)//是1，放技能
            {
                animator.SetBool("Walk", false);

                int tempIndex = 0;//存放找到的GameObject
                this.attackCounter += Time.deltaTime;
                if (this.attackCounter >= this.attackTime)//攻擊間隔
                {
                    for (int i = 0; i < tempTrasforms.Length; i++)//找子物件
                    {
                        if (tempTrasforms[i].name == "Deep_鬼哭斬")
                        {
                            tempIndex = i;
                            if (!stateAttack)//不是攻擊狀態(不然一直按會一直攻擊，攻擊動畫還沒做完就又攻擊)
                            {
                                stateAttack = true;//普通攻擊狀態
                                canmove = false;
                                this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//開啟Deep_鬼哭斬物件

                                //print("攻擊動畫_開" + Time.time);
                                animator.SetBool(parSkill01, true);//開啟動畫
                                aud.PlayOneShot(deep_sf2);//音效
                                StartCoroutine(waitSkill01_AI(tempIndex, node));//等0.5秒關閉GameObject
                            }
                        }
                    }

                }
                return NodeState.RUNNING;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
        #endregion

        #region 自訂方法_一般呼叫

        /// <summary>
        /// //睡覺_攻擊
        /// AI用
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected IEnumerator waitAttack_AI(int index, Node node)
        {
            yield return new WaitForSeconds(wait500);//等待幾秒
            stateAttack = false;//攻擊狀態關閉
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉Deep_普攻物件
            animator.SetBool(parAttack1, false);////關閉攻擊1動畫
            animator.SetBool(parAttack2, false);////關閉攻擊2動畫
            canmove = true;
            this.attackCounter = 0f;//重置攻擊間隔
            //print("攻擊動畫_關"+Time.time);
            if (targetPlayerHealth.Hp <= 0)
            {
                targetPlayer.GetComponent<BoxCollider2D>().enabled = false;
                node.ClearData("targetAlive");//清除目標
            }
        }

        /// <summary>
        /// //睡覺_攻擊
        /// AI用
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected IEnumerator waitSkill01_AI(int index, Node node)
        {
            yield return new WaitForSeconds(wait500);//等待幾秒
            stateAttack = false;//攻擊狀態關閉
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉Deep_鬼哭斬
            animator.SetBool(parSkill01, false);//關閉動畫
            this.attackCounter = 0f;//重置攻擊間隔
            canmove = true;
            //print("攻擊動畫_關"+Time.time);
            if (targetPlayerHealth.Hp <= 0)
            {
                targetPlayer.GetComponent<BoxCollider2D>().enabled = false;
                node.ClearData("targetAlive");//清除目標
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        //用技能開啟碰撞，等0.5關閉碰撞
        protected IEnumerator WaitCloseGameObject_AI(int index, Node node)
        {
            //print("關閉collider");
            yield return new WaitForSeconds(timeIntervalSkill);//等待幾秒
                                                               //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;//開啟刀的碰撞
                                                               //this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//關閉子物件
            stateAttack = false;//攻擊狀態關閉
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉鬼哭斬物件
            this.attackCounter = 0f;//重置攻擊間隔
        }
        #endregion
    }
}