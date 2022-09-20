using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class Role_davis : MonoBehaviour
{
    #region  屬性

    [SerializeField, Header("走路速度"), Tooltip("用velocivy控制")]//用velocivy控制
    private float speedWalk = 1500;
    [SerializeField, Header("跳躍力量")]
    private float jumpForce = 250;
    [SerializeField, Header("被打上飛")]
    private float speedInjuriedUp = 3;

    [SerializeField, Header("翻滾速度"), Tooltip("用velocivy控制")]//用velocivy控制
    private float speedRoll = 100;
    [SerializeField, Header("檢查地板尺寸")]
    private Vector3 v3CheckGroundSize = new Vector3(3.61f, 0.27f, 0);
    [SerializeField, Header("檢查地板位移")]
    private Vector3 v3CheckGroundOffset = new Vector3(0.02f, -3.62f, 0);
    [SerializeField, Header("檢查Shadow位移")]
    private Vector3 v3CheckGroundOffsetShadow = new Vector3(0f, -3.8f, 0);
    [SerializeField, Header("檢查地板顏色")]
    private Color colorCheckGround = new Color(1, 0, 0.2f, 0.5f);
    [SerializeField, Header("檢查地板圖層")]
    private LayerMask layerGround;
    [SerializeField, Header("總血量")]
    private float totalHp = 100;//血量
    [SerializeField, Header("遊戲中血量")]
    private float scriptHp;//血量
    [SerializeField, Header("UI血條")]
    private Image imageHP;
    private Animator animator;
    private Rigidbody2D rig2D;
    private Transform trans;
    private Transform transShadow;
    private Collider2D coll2D;

    private GameObject davis;
    [SerializeField, Header("影子")]
    private GameObject shadow;


    private bool clickJump;
    private bool isGround;//是否在地面上,預設是false
    private bool isWalk;//是否走路,預設是false
    private bool canJump = true;//是否能做跳躍。動畫:[true:不做跳躍動畫,false:做跳躍動畫]
    private bool moveShadow = true;//移動影子
    private bool canmove = true;//是否能移動
    private bool stateRun;//true:跑步中  false:沒跑步
    private bool stateRoll;//true:翻滾中  false:沒翻滾
    private bool stateInjuried;//true:受傷中 false:沒受傷
    private bool stateInjuriedUp;//true:受傷上飛中

    private string parWalk = "Walk";
    private string parRun = "Run";
    private string parJump = "Jump";
    private String parDefense = "Defense";
    private String parTriggerRoll = "TriggerRoll";
    private String parRoll = "Roll";
    private string parInjuried = "Injuried";//原地受傷
    private string parInjuriedUp = "Injuried_up";//原地受傷
    private string parDeath = "death";//死亡
    private bool pressRight;//是否按右鍵
    private bool pressLeft;//是否按左鍵
    private float pressRightTime;//按下右鍵時間
    private float releaseRightTime;//放開右鍵時間
    private float pressLeftTime;//左鍵
    private float releaseLeftTime;
    private float pressUpTime;//上鍵
    private float releaseUpTime;
    private float pressDownTime;//下鍵
    private float releaseDownTime;
    private float pressRightControlTime;//control
    private float releaseRightControlTime;
    private float pressRightShifTime;//shift
    private float releaseShiftTime;
    private float pressEnterTime;//enter
    private float releaseEnterTime;

    private float pressInterval = 0.1f;//區間秒數0.1
    private float pressInterval3 = 0.3f;//區間秒數0.3
    private float pressInterval5 = 0.5f;//區間秒數0.2
    private float pSpeedWalk = 0.01f;//用position走路
    private float pSpeedRun = 0.05f;//用position跑步路
    private float pSpeedJump = 1f;

    private float waitMillisecond = 0.5f;//等待時間

    private float originalY;//紀錄跳起翻滾時，原本y的位置
    private float originalRollY;//紀翻滾時時，原本y的位置

    System.Random random;
    private int i01;//random產生
    [SerializeField,Header("Image_遊戲結束底圖")]
    private CanvasGroup GameObject_GameOver;
    [SerializeField, Header("淡入時間")]
    private float intervalFadIn = 0.5f;
    private bool stateOpenGameOverImage;//死亡畫布狀態
    //測試
    private int twoJump;
    #endregion

    #region 事件:程式入口
    //喚醒事件:開始事件前執行一次，物件步開啟也會執行，取得元件等等
    private void Awake()
    {
        davis = GameObject.Find("Davis");
        animator = davis.GetComponent<Animator>();
        rig2D = davis.GetComponent<Rigidbody2D>();
        trans = davis.GetComponent<Transform>();
        transShadow = shadow.GetComponent<Transform>();
        //print(LayerMask.NameToLayer("Ground"));
        layerGround.value = LayerMask.GetMask("Ground");//設定LayerMask
        //deep.layer=LayerMask.NameToLayer("Ground");//設定LayerMask

    }
    // Start is called before the first frame update
    void Start()
    {

        random = new System.Random();
        scriptHp = totalHp;//遊戲血量初始化
    }

    //更新事件:每秒執行約60次，60FPS Frame per second
    void Update()
    {

        ShadowOffset();
        /*
        JumpKey();
        Walk2();
        UpdateJumpAnimator();
        Attack();
        */
        //判斷被砍上飛，要掉落回原地
        if (stateInjuriedUp)
        {
            InjuriedUp();
        }
        GameOver();
    }
    //一秒固定50次，物理移動放這裡
    private void FixedUpdate()
    {
        //JumpForce();

    }
    #endregion
    #region unity方法
    private void OnDrawGizmos()
    {
        //1.決定顏色
        Gizmos.color = colorCheckGround;

        //2.繪製圖示
        //trans.position 當前物件座標trans
        Gizmos.DrawCube(transform.position + v3CheckGroundOffset, v3CheckGroundSize);
        //Gizmos.DrawCube(trans.position + v3CheckGroundOffset, v3CheckGroundSize);
    }

    #region 2個要碰撞物件一個勾 is Trigger

    //2個物件碰撞執行一次
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //print("碰到物體名子" + collision.gameObject.name);
        //print("碰到tag名子" + collision.gameObject.tag);

        //被破空斬打
        if (collision.gameObject.tag == "Bullet" && collision.gameObject.name == "deep_ball(Clone)")//如果碰到子彈
        {
            print("被破空斬打");
            this.animator.SetBool(parInjuried, true);//開啟受傷動畫
            canmove = false;//不能移動
            stateInjuried = true;
            //print("碰到物體名子" + collision.gameObject.name);
            this.DeductBlood(40);//扣40滴血
            Destroy(collision.gameObject);//子彈打到人就消失
            StartCoroutine(WaitInjuried());
        }

        //被鬼哭斬打
        if (collision.gameObject.tag == "Player" && PlayerPrefs.GetString("Deep_鬼哭斬") == "鬼哭斬" && !stateInjuriedUp)//如果碰到玩家 && 被鬼哭展打到 && 狀態不是打飛(不然打飛中還能再打)
        {
            print("被鬼哭斬打");
            animator.SetBool(parInjuriedUp, true);//上飛動畫
            this.DeductBlood(20);//扣20滴血
            moveShadow = false;//影子不要位移
            originalY = transform.position.y;//記錄跳起的位置
            //rig2D.AddForce(new Vector2(0, speedInjuriedUp));//往上力量
            rig2D.velocity = new Vector2(0, speedInjuriedUp);//往上速度
            clickJump = false;
            canJump = false;

            StartCoroutine(WaitInjuriedUp());
        }
        //被普攻打
        if (collision.gameObject.tag == "Player" && PlayerPrefs.GetString("Deep_普攻") == "Deep_普攻" )//如果碰到玩家 && 被鬼哭展打到 
        {
            print("被普攻打");
            this.animator.SetBool(parInjuried, true);//開啟受傷動畫
            this.DeductBlood(20);//扣20滴血
            canmove = false;//不能移動
            stateInjuried = true;
            clickJump = false;
            canJump = false;

            StartCoroutine(WaitInjuried());
        }
    }
    //2個物件碰撞離開執行一次
    private void OnTriggerExit2D(Collider2D collision)
    {

    }
    //兩個物件碰撞重疊時持續執行
    private void OnTriggerStay2D(Collider2D collision)
    {

    }
    #endregion

    #region 2個要碰撞物件都沒勾選 is Trigger
    //2個物件碰撞執行一次
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print("碰到物體名子" + collision.gameObject.name);
        //print("碰到tag名子" + collision.gameObject.tag);

    }
    //2個物件碰撞離開執行一次
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
    //兩個物件碰撞重疊時持續執行
    private void OnCollisionStay2D(Collision2D collision)
    {

    }
    #endregion
    #endregion

    #region 自訂方法


    
    /// <summary>
    /// 用Input.GetKey控制上下左右
    /// </summary>
    protected void Walk2()
    {
        //************************GetKey*************************************************//
        //右鍵
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pressRight = true;//是否右按下右鍵
            pressRightTime = Time.time;
            //print($"pressRightTime:{pressRightTime}");
            if (pressRightTime - releaseRightTime <= pressInterval && canmove)//跑步
            {
                print("<Color=yellow>右跑</Color>");
                stateRun = true;//跑步狀態
                animator.SetBool(parRun, true);//開啟跑步動畫
                gameObject.transform.position += new Vector3(pSpeedRun, 0, 0);
                releaseRightTime = Time.time;//跑步狀態中要持續更新鬆鍵時間
            }
            else if (canmove)//走路
            {
                print("<Color=red>走路</Color>");
                gameObject.transform.position += new Vector3(pSpeedWalk, 0, 0);
                animator.SetBool(parWalk, true);
            }
            //向右轉向
            gameObject.transform.rotation = new Quaternion(trans.rotation.x, 0, trans.rotation.z, trans.rotation.w);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) && canmove)
        {
            pressRight = false;//是否右按下右鍵
            stateRun = false;//關掉跑步狀態
            animator.SetBool(parWalk, false);//關閉走路動畫
            animator.SetBool(parRun, false);//關閉跑步動畫
            releaseRightTime = Time.time;
            //print($"releaseRightTime:{releaseRightTime}");
        }
        //左鍵
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pressLeft = true;//是否右按下左鍵
            pressLeftTime = Time.time;
            if (pressLeftTime - releaseLeftTime <= pressInterval && canmove)//跑步
            {
                print("<Color=yellow>左跑</Color>");
                stateRun = true;//跑步狀態
                animator.SetBool(parRun, true);//開啟跑步動畫
                gameObject.transform.position += new Vector3(-pSpeedRun, 0, 0);
                releaseLeftTime = Time.time;
            }
            else if (canmove)//走路
            {
                print("<Color=red>走路</Color>");
                gameObject.transform.position += new Vector3(-pSpeedWalk, 0, 0);
                animator.SetBool(parWalk, true);
            }
            //向左轉向
            gameObject.transform.rotation = new Quaternion(trans.rotation.x, 180, trans.rotation.z, trans.rotation.w);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            pressLeft = false;//是否右按下左鍵
            stateRun = false;//關掉跑步狀態
            animator.SetBool(parWalk, false);
            animator.SetBool(parRun, false);
            releaseLeftTime = Time.time;
            //print($"releaseRightTime:{releaseRightTime}");
        }

        //上鍵
        if (Input.GetKey(KeyCode.UpArrow) && canmove)
        {
            pressUpTime = Time.time;
            if (canmove)//走路
            {
                gameObject.transform.position += new Vector3(0, pSpeedWalk, 0);
                animator.SetBool(parWalk, true);
            }
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) && canmove)
        {
            if (canmove)//走路
            {
                animator.SetBool(parWalk, false);
                releaseUpTime = Time.time;
                //print($"releaseRightTime:{releaseRightTime}");
            }
        }
        //下鍵
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pressDownTime = Time.time;
            if (canmove)//走路
            {
                gameObject.transform.position += new Vector3(0, -pSpeedWalk, 0);
                animator.SetBool(parWalk, true);
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (canmove)
            {
                animator.SetBool(parWalk, false);
                releaseDownTime = Time.time;
                //print($"releaseRightTime:{releaseRightTime}");
            }

        }

    }


   
    /// <summary>
    ///紀錄玩家是否按RightShift(跳躍)，並且落下時把地心引力、y速度都歸0
    /// </summary>
    protected void JumpKey()
    {

        if (Input.GetKeyDown(KeyCode.RightShift) && canJump)
        {
            print("跳躍");
            clickJump = true;
        }

        //print(temeV3);
        //print("transform" + transform.position);
        if (transform.position.y <= originalY && !canJump && !stateRoll)//落下時的位置，小於等於起跳點時 && 在空中 && 不翻滾。取消地心引力和y速度
        {
            //print("HI");
            rig2D.gravityScale = 0;
            rig2D.velocity = new Vector2(0, 0);//碰到撞時也會有反向作用力的速度，因為有重力讓y軸有加速度用，會繼續掉落，所以y軸速度要用0
            canJump = true;
            moveShadow = true;//影子移動
        }

    }
    /// <summary>
    /// 有按下RightShift(跳躍鍵)，給向上推力，並把地心引力設為1
    /// </summary>
    protected void JumpForce()//案跳躍&&在地板時給向上的力量
    {
        if (clickJump && canJump && !stateRoll)//有按rightShift && 能跳 && 不翻滾   能跳
        {
            moveShadow = false;//影子不要位移
            originalY = transform.position.y;//記錄跳起的位置
            //print(temeV3);
            rig2D.AddForce(new Vector2(0, jumpForce));
            rig2D.gravityScale = 1;//跳起後地心引力設1
            clickJump = false;
            canJump = false;
        }
    }
    /// <summary>
    /// 更新跳躍動畫
    /// </summary>
    private void UpdateJumpAnimator()
    {
        animator.SetBool(parJump, canJump);
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
    private void Attack()
    {
        //*********************Enter******************************//
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pressEnterTime = Time.time;
            //ctrl↓Enter
            //print($"pressRightControlTime:{pressRightControlTime}");//案右鍵時間
            //print($"pressDownTime:{pressDownTime}");//案下鍵時間
            //print($"pressEnterTime:{pressEnterTime}");//案enter鍵時間
            if (Math.Abs(pressDownTime - pressRightControlTime) < pressInterval3 && Math.Abs(pressEnterTime - pressRightControlTime) < pressInterval5)
            {
                Debug.Log("ctrl↓Enter");
                animator.SetTrigger("鬼哭斬");
            }
            //ctrl→Enter
            else if (Math.Abs(pressRightTime - pressRightControlTime) < pressInterval3 && Math.Abs(pressEnterTime - pressRightControlTime) < pressInterval5)
            {
                Debug.Log("ctrl→Enter");
            }
            //Enter(攻擊)
            else
            {
                //print(random.Next(2));//0、1
                //print(random.Next(1,3));//1、2
                i01 = random.Next(1, 3);
                //print(i01);
                if (i01 == 1)
                {
                    //Debug.Log("Attack1"+temp);
                    animator.SetTrigger("TriggerAttack1");
                }
                else
                {
                    //Debug.Log("Attack2"+temp);
                    animator.SetTrigger("TriggerAttack2");
                }
            }
        }
        //鬆Enter
        if (Input.GetKeyUp(KeyCode.Return))
        {
            //print(random.Next(2));//0、1
            //print(random.Next(1,3));//1、2
            //print(i01);
            if (i01 == 1)
            {
                //Debug.Log("Attack1"+temp);
                //animator.SetBool("Attack1", false);
            }
            else
            {
                //Debug.Log("Attack2"+temp);
                //animator.SetBool("Attack2", false);
            }
        }
        //*********************RightControlTime******************************//
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            pressRightControlTime = Time.time;
            if (stateRun && pressRight)//跑步時向右翻滾
            {
                print("右翻滾");
                stateRoll = true;//翻滾中
                moveShadow = false;//影子y軸不要動
                originalY = transform.position.y;//紀錄人物位置，因為翻滾會變低
                animator.SetBool(parRoll, true);
                transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                canmove = false;//不能移動
                rig2D.velocity = new Vector2(speedRoll * Time.deltaTime, rig2D.velocity.y);
                StartCoroutine(WaitRoll());//等待幾秒

            }
            else if (stateRun && pressLeft)//跑步時向左翻滾
            {
                print("左翻滾");
                stateRoll = true;//翻滾中
                moveShadow = false;//影子y軸不要動
                originalY = transform.position.y;//紀錄人物位置，因為翻滾會變低
                animator.SetBool(parRoll, true);
                transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                canmove = false;//不能移動
                rig2D.velocity = new Vector2(-speedRoll * Time.deltaTime, rig2D.velocity.y);
                StartCoroutine(WaitRoll());//等待幾秒
            }
            else//走路按防禦
            {
                canmove = false;//不能移動
                animator.SetBool(parDefense, true);//開啟防禦動畫
                StartCoroutine(WaitDefense());//等待幾秒
            }




        }
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            releaseRightControlTime = Time.time;
            animator.SetBool(parDefense, false);//關閉防禦動畫


        }
        //*********************RightShiftTime******************************//
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            pressRightShifTime = Time.time;
        }
    }
    /// <summary>
    /// 扣血
    /// </summary>
    /// <param name="deductBlood"></param>
    private void DeductBlood(int deductBlood)
    {
        this.scriptHp -= deductBlood;//程式中扣血
        imageHP.fillAmount = scriptHp / totalHp;//UI中扣血
        if (imageHP.fillAmount <= 0)
        {
            //print("死亡動畫");
            animator.SetBool(parDeath, true);//死亡動畫
            transform.position = new Vector2(transform.position.x, transform.position.y - 2.57f);//死亡動畫降低位置
        }
    }
    /// <summary>
    ///血量歸0就遊戲結束
    /// </summary>
    private void GameOver()
    {
        if (imageHP.fillAmount <= 0)
        {
            //print("遊戲結束畫面");
            shadow.SetActive(false);//死亡時關閉影子

            if(!stateOpenGameOverImage)
            {
                stateOpenGameOverImage = true;//畫布開啟中
                StartCoroutine(CavasGroupGameOver());
            }
                

        }

    }
    /// <summary>
    /// 被打上飛，判斷掉回原本地方
    /// </summary>
    private void InjuriedUp()
    {
        if (Mathf.Abs(transform.position.y - originalY) < 0.2 && !canJump && !stateRoll)//落下時的位置，小於等於起跳點時 && 在空中 && 不翻滾。取消地心引力和y速度 && 受傷上飛中
        {
            rig2D.gravityScale = 0;
            rig2D.velocity = new Vector2(0, 0);//碰到撞時也會有反向作用力的速度，因為有重力讓y軸有加速度用，會繼續掉落，所以y軸速度要用0
            transform.position = new Vector2(transform.position.x, originalY);//掉落時高度設定成跳躍前高度
            canJump = true;
            moveShadow = true;//影子移動
            animator.SetBool(parInjuriedUp, false);//關閉受傷上飛動畫
            canmove = true;//可以移動
            stateInjuriedUp = false;//關閉受傷上飛狀態
        }
    }
    /// <summary>
    /// 遊戲結束Canvasgroup顯示淡入
    /// </summary>
    /// <returns></returns>
    IEnumerator CavasGroupGameOver(bool fadeIn=true)
    {
        //三元運算子
        //布林直?布林值為 true:布林值為 false
        float increase = fadeIn ? 0.1f : -0.1f;
        for (int i=0;i<10;i++)
        {
            GameObject_GameOver.alpha += increase;
            //print(i);
            yield return new WaitForSeconds(intervalFadIn);
        }
    }
   
    //防禦等待_防禦自動解除
    IEnumerator WaitDefense()
    {
        yield return new WaitForSeconds(waitMillisecond);//等待幾秒
        animator.SetBool(parDefense, false);////關閉防禦動畫
        canmove = true;//恢復能移動
    }

    //翻滾等待
    IEnumerator WaitRoll()
    {
        print("翻滾等待");
        yield return new WaitForSeconds(waitMillisecond);//等待幾秒
        rig2D.velocity = new Vector2(0, 0);
        stateRun = false;//不是跑步狀態
        animator.SetBool(parRun, false);////關閉跑步動畫
        animator.SetBool(parRoll, false);////關閉翻滾動畫
        transform.position = new Vector2(transform.position.x, transform.position.y + 1.2f);//恢復人物翻滾前y位置
        moveShadow = true;//影子恢復
        canmove = true;//恢復能移動
        stateRoll = false;//不翻滾
    }
    //受傷等待
    IEnumerator WaitInjuried()
    {
        yield return new WaitForSeconds(waitMillisecond);//等待幾秒
        animator.SetBool(parInjuried, false);//關閉受傷動畫
        canmove = true;//可以移動

    }

    //上飛等待
    IEnumerator WaitInjuriedUp()
    {
        yield return new WaitForSeconds(waitMillisecond);//等待幾秒
        stateInjuriedUp = true;//受傷上飛狀態
        rig2D.gravityScale = 1;//跳起後地心引力設1
    }

 



    #endregion
}
