using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;//引用AudioMixer程式庫
using System;//有用亂數
using UnityEngine.UI;
using BehaviorTree;
namespace chia
{


    public class Role : MonoBehaviour
    {
        #region 屬性 被打

        [SerializeField, Header("被打上飛")]
        internal float speedInjuriedUp = 3;
        [SerializeField, Header("淡入時間")]
        internal float intervalFadIn = 0.5f;
        internal bool stateOpenGameOverImage;//死亡畫布狀態

        internal bool stateInjuried;//true:受傷中 false:沒受傷
        internal bool stateInjuried_Insitu;//true:原地受傷中 false:沒受傷
        internal bool stateInjuriedUp;//true:受傷上飛中
        internal bool stateInjuriedUp_falling;////true:受傷上飛中，最高點到落地前
        internal bool stateDead;////true:死亡

        protected string parInjuried = "Injuried";//原地受傷
        protected string parInjuriedUp = "Injuried_up";//原地受傷
        protected string parDeath = "death";//死亡


        #endregion

        #region  屬性
       
        
        [SerializeField, Header("翻滾速度"), Tooltip("用velocivy控制")]//用velocivy控制
        protected float speedRoll = 100;
        [SerializeField, Header("檢查地板尺寸")]
        protected Vector3 v3CheckGroundSize = new Vector3(3.61f, 0.27f, 0);
        [SerializeField, Header("檢查地板位移")]
        protected Vector3 v3CheckGroundOffset = new Vector3(0.02f, -1.7f, 0);
        [SerializeField, Header("檢查Shadow位移")]
        protected Vector3 v3CheckGroundOffsetShadow = new Vector3(0f, -3.71f, 0);
        [SerializeField, Header("檢查地板顏色")]
        protected Color colorCheckGround = new Color(1, 0, 0.2f, 0.5f);
        [SerializeField, Header("檢查地板圖層")]
        protected LayerMask layerGround;
        [SerializeField, Header("影子")]
        internal GameObject shadow;

        
        [SerializeField, Header("音效_普攻")]
        protected AudioClip deep_sf0;
        [SerializeField, Header("音效_破空斬")]
        protected AudioClip deep_sf1;
        [SerializeField, Header("音效_鬼哭斬")]
        protected AudioClip deep_sf2;
        [SerializeField, Header("角色")]
        protected GameObject role;
        protected Animator animator; //deep的動畫
        internal Rigidbody2D rig2D;
        protected Transform trans;//deep的trans
        protected Transform transShadow;//影子的trans
        protected Collider2D coll2D;

        protected Transform[] tempTrasforms; //取得放此腳本的GameObject，所有子物件(包含自己)
        protected AudioSource aud;//撥音樂

        protected string parWalk = "Walk";
        protected string parRun = "Run";
        protected string parJump = "Jump";
        protected String parDefense = "Defense";
        protected String parTriggerRoll = "TriggerRoll";
        protected String parRoll = "Roll";
        protected string parAttack1 = "Attack1";
        protected string parAttack2 = "Attack2";
        protected String parRunAtack = "RunAttack";
        protected String parSkill01 = "鬼哭斬";
        
        internal bool canmove = true;//是否能移動
        internal bool moveShadow = true;//移動影子

        
        internal bool stateJump;//跳躍狀態。true:跳躍狀態(不能跳躍),false:非跳躍狀態(能跳躍)
        protected bool stateWalk;//true:走路中  false:沒走路
        protected bool stateRun;//true:跑步中  false:沒跑步
        internal bool stateRoll;//true:翻滾中  false:沒翻滾
        protected bool stateAttack;//true:攻擊中  false:沒攻擊
        protected bool stateRunAttack;//true:跑步時攻擊狀態  false:沒跑步時攻擊狀態
        internal bool stateDefense;//true:防禦狀態  
        //移動是用getKey判斷
        //攻擊技能接技用getKeyDown判斷
        protected float getKey_UpTime;//偵測持續按上鍵
        protected float getKeyUp_UpTime;//鬆上鍵
        protected float getKeyDown_UpTime;//按一次上鍵
        protected bool pressUp;//是否按上鍵

        protected float getKey_DownTime;//偵測持續按下鍵
        protected float getKeyUp_DownTime;//鬆下鍵
        protected float getKeyDown_DownTime;//按一次下鍵
        protected bool pressDown;//是否按下鍵

        protected float getKey_LeftTime;//偵測持續按左鍵
        protected float getKeyUp_LeftTime;//鬆左鍵
        protected float getKeyDown_LeftTime;//按一次左鍵
        protected bool pressLeft;//是否按左鍵

        protected float getKey_RightTime;//偵測持續按右鍵
        protected float getKeyUp_RightTime;//鬆右鍵
        protected float getKeyDown_RightTime;//按一次右鍵
        protected bool pressRight;//是否按右鍵

        protected float getKeyUp_EnterTime;//鬆Enter鍵
        protected float getKeyDown_EnterTime;//按一次鬆Enter鍵鍵
        protected bool pressEnter;//是否按鬆Enter鍵鍵

        protected float getKeyUp_RightControlTime;//鬆RightControl鍵
        protected float getKeyDown_RightControlTime;//按一次鬆RightControl鍵
        protected bool pressRightControl;//是否按鬆RightControl鍵

        protected float getKeyUp_RightShifTime;//鬆RightShif鍵
        protected float getKeyDown_RightShifTime;//按一次鬆RightShif鍵
        protected bool pressRightShif;//是否按鬆RightShif鍵



        


        protected float pressInterval = 0.1f;//區間秒數0.1
        protected float pressInterval3 = 0.3f;//區間秒數0.3
        protected float pressInterval5 = 0.5f;//區間秒數0.2




        internal float timeIntervalSkill = 0.5f;//時間間隔
        internal float wait100 = 0.2f;//0.1秒
        internal float wait200 = 0.2f;//0.2秒
        internal float wait300 = 0.3f;//0.3秒
        internal float wait500 = 0.5f;//0.5秒
        internal float wait1000 = 0.5f;//1秒
        internal float originalY;//紀錄跳起翻滾時，原本y的位置
        internal System.Random random;
        internal int i01;//random產生
        

        
        //移動範圍限制
        [SerializeField, Header("minX")]
        protected float minX = -41.1f;
        [SerializeField, Header("maxX")]
        protected float maxX = 44f;
        [SerializeField, Header("minY")]
        protected float minY = -11.3f;
        [SerializeField, Header("maxY")]
        protected float maxY = 21f;
        #endregion

        #region 事件:程式入口
        //喚醒事件:開始事件前執行一次，物件步開啟也會執行，取得元件等等
        protected virtual void Awake()
        {
            animator = role.GetComponent<Animator>();
            rig2D = role.GetComponent<Rigidbody2D>();
            trans = role.GetComponent<Transform>();
            transShadow = shadow.GetComponent<Transform>();
            //print(LayerMask.NameToLayer("Ground"));
            layerGround.value = LayerMask.GetMask("Ground");//設定LayerMask
                                                            //deep.layer=LayerMask.NameToLayer("Ground");//設定LayerMask
            tempTrasforms = this.gameObject.GetComponentsInChildren<Transform>(true);//取得放此腳本的GameObject，所有子物件(包含自己)
            aud = role.GetComponent<AudioSource>();

        }
        // Start is called before the first frame update
        protected virtual void Start()
        {
            random = new System.Random();

        }

        //更新事件:每秒執行約60次，60FPS Frame per second
         protected virtual void Update()
        {
            ShadowOffset();
            
            #region 被打
            //判斷被砍上飛，要掉落回原地
            InjuriedUp_FallBackInToPlace();
            #endregion
        }
        #endregion

        #region unity方法

        /// <summary>
        /// OnDrawGizmos:繪製圖示
        /// </summary>
        private void OnDrawGizmos()
        {
            //1.決定顏色
            Gizmos.color = colorCheckGround;

            //2.繪製圖示
            //trans.position 當前物件座標trans
            Gizmos.DrawCube(transform.position + v3CheckGroundOffset, v3CheckGroundSize);
            //Gizmos.DrawCube(trans.position + v3CheckGroundOffset, v3CheckGroundSize);
        }
        #region 被打



        #endregion
        #endregion

        #region 自訂方法
        /// <summary>
        /// 走路
        /// </summary>
        protected virtual void Walk2()
        {

        }




        
        /// <summary>
        /// 跳躍後要落回原地判斷
        /// </summary>
        protected void Jump_FallBackInToPlace()
        {
            //print(temeV3);
            //print("transform" + transform.position);
            if (transform.position.y <= originalY && stateJump && !stateRoll)//落下時的位置，小於等於起跳點時 && 跳躍狀態 && 不翻滾。取消地心引力和y速度
            {
                //print("HI");
                canmove = true;
                rig2D.gravityScale = 0;
                rig2D.velocity = new Vector2(0, 0);//碰到撞時也會有反向作用力的速度，因為有重力讓y軸有加速度用，會繼續掉落，所以y軸速度要用0
                stateJump = false;
                animator.SetBool(parJump, stateJump);//關閉跳躍
                moveShadow = true;//影子移動
            }

        }

        /// <summary>
        /// 控制circle(影子)，跟著人物移動
        /// </summary>
        private void ShadowOffset()
        {
            if (moveShadow)
            {
                transShadow.position = transform.position + v3CheckGroundOffsetShadow;
            }
            else//跳起來影子y軸不要動
            {
                transShadow.position = new Vector2(transform.position.x + v3CheckGroundOffsetShadow.x, originalY + v3CheckGroundOffsetShadow.y);
            }

        }
        /// <summary>
        /// 攻擊
        /// </summary>
        protected virtual void Attack()
        {

        }

        //防禦自動解除
        protected IEnumerator waitDefense()
        {
            yield return new WaitForSeconds(wait1000);//等待幾秒
            animator.SetBool(parDefense, false);////關閉防禦動畫
            stateDefense = false;
            canmove = true;//恢復能移動
        }
        //睡覺_攻擊
        protected IEnumerator waitAttack(int index)
        {
            yield return new WaitForSeconds(wait500);//等待幾秒
            stateAttack = false;//不是普通攻擊狀態
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉Deep_普攻物件
            animator.SetBool(parAttack1, false);////關閉攻擊1動畫
            animator.SetBool(parAttack2, false);////關閉攻擊2動畫
        }
        //睡覺_攻擊
        protected IEnumerator waitSkill01(int index)
        {
            yield return new WaitForSeconds(wait500);//等待幾秒
            stateAttack = false;//不是普通攻擊狀態
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉Deep_普攻物件
            animator.SetBool(parSkill01, false);////關閉攻擊1動畫
        }

        //等待_跑步時攻擊
        protected IEnumerator waitRunAttack(int index)
        {
            yield return new WaitForSeconds(wait300);//等待幾秒
            stateRunAttack = false;//不是跑步普通攻擊狀態
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉Deep_普攻物件
            animator.SetBool(parRunAtack, false);
        }
        //翻滾睡覺
        protected IEnumerator waitRoll()
        {
            print("翻滾睡覺");
            yield return new WaitForSeconds(wait500);//等待幾秒
            rig2D.velocity = new Vector2(0, 0);
            stateRun = false;//不是跑步狀態
            animator.SetBool(parRun, false);////關閉跑步動畫
            animator.SetBool(parRoll, false);////關閉翻滾動畫
            transform.position = new Vector2(transform.position.x, transform.position.y + 1.2f);//恢復人物翻滾前y位置
            moveShadow = true;//影子恢復
            canmove = true;//恢復能移動
            stateRoll = false;//不翻滾
        }
        //用技能開啟碰撞，等0.5關閉碰撞
        protected IEnumerator WaitCloseGameObject(int index)
        {
            //print("關閉collider");
            yield return new WaitForSeconds(timeIntervalSkill);//等待幾秒
                                                               //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;//開啟刀的碰撞
                                                               //this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//關閉子物件

            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉鬼哭斬物件

        }

        #region  被打
        /// <summary>
        /// 被打上飛，判斷掉回原本地方
        /// </summary>
        protected void InjuriedUp_FallBackInToPlace()
        {
            if (Mathf.Abs(transform.position.y - originalY) < 0.2 && stateInjuriedUp_falling && !stateRoll)//落下時的位置，小於等於起跳點時 && 在空中 && 不翻滾。取消地心引力和y速度 && 受傷上飛中
            {
                //print("判斷掉回原本地方 if後");
                rig2D.gravityScale = 0;
                rig2D.velocity = new Vector2(0, 0);//碰到撞時也會有反向作用力的速度，因為有重力讓y軸有加速度用，會繼續掉落，所以y軸速度要用0
                transform.position = new Vector2(transform.position.x, originalY);//掉落時高度設定成跳躍前高度
                moveShadow = true;//影子移動
                animator.SetBool(parInjuriedUp, false);//關閉受傷上飛動畫
                canmove = true;//可以移動
                stateInjuried = false;//關閉受傷
                stateInjuriedUp = false;//關閉受傷上飛狀態
                stateInjuriedUp_falling = false;

                if (stateDead)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 2.57f);//死亡動畫降低位置
                }
            }
        }

        #endregion



        #endregion
    }
}