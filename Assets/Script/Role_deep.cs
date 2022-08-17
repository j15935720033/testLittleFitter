using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Role_deep : MonoBehaviour
{
    #region  屬性

    [SerializeField, Header("走路速度"), Tooltip("用velocivy控制")]//用velocivy控制
    private float speedWalk = 1500;
    [SerializeField, Header("跳躍力量")]
    private float jumpForce = 250;
    [SerializeField, Header("翻滾速度"), Tooltip("用velocivy控制")]//用velocivy控制
    private float speedRoll = 100;
    [SerializeField, Header("檢查地板尺寸")]
    private Vector3 v3CheckGroundSize = new Vector3(3.61f, 0.27f, 0);
    [SerializeField, Header("檢查地板位移")]
    private Vector3 v3CheckGroundOffset = new Vector3(0.02f, -1.7f, 0);
    [SerializeField, Header("檢查Shadow位移")]
    private Vector3 v3CheckGroundOffsetShadow = new Vector3(-0.19f, -1.91f, 0);
    [SerializeField, Header("檢查地板顏色")]
    private Color colorCheckGround = new Color(1, 0, 0.2f, 0.5f);
    [SerializeField, Header("檢查地板圖層")]
    private LayerMask layerGround;
    [SerializeField, Header("影子")]
    private GameObject shadow;
    [SerializeField, Header("prefabe_破空斬")]
    private GameObject prefabBullet;//破空斬	

    private Animator animator;
    private Rigidbody2D rig2D;
    private Transform trans;
    private Transform transShadow;
    private Collider2D coll2D;
    private GameObject deep;


    private string parWalk = "Walk";
    private string parRun = "Run";
    private string parJump = "Jump";
    private String parDefense = "Defense";
    private String parTriggerRoll = "TriggerRoll";
    private String parRoll = "Roll";

    private bool clickJump;
    private bool isGround;//是否在地面上,預設是false
    private bool isWalk;//是否走路,預設是false
    private bool canJump = true;//是否能做跳躍。動畫:[true:不做跳躍動畫,false:做跳躍動畫]
    private bool moveShadow = true;//移動影子
    private bool canmove = true;//是否能移動
    private bool stateRun;//true:跑步中  false:沒跑步
    private bool stateRoll;//true:翻滾中  false:沒翻滾
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

    private float timeIntervalSkill=0.5f;//時間間隔

    private float originalY;//紀錄跳起翻滾時，原本y的位置
    private float originalRollY;//紀翻滾時時，原本y的位置
    System.Random random;
    private int i01;//random產生
    private int hp;//血量


    //測試
    private int twoJump;
    #endregion

    #region 事件:程式入口
    //喚醒事件:開始事件前執行一次，物件步開啟也會執行，取得元件等等
    private void Awake()
    {
        deep = GameObject.Find("Deep");

        animator = deep.GetComponent<Animator>();
        rig2D = deep.GetComponent<Rigidbody2D>();
        trans = deep.GetComponent<Transform>();
        transShadow = shadow.GetComponent<Transform>();
        //print(LayerMask.NameToLayer("Ground"));
        layerGround.value = LayerMask.GetMask("Ground");//設定LayerMask
        //deep.layer=LayerMask.NameToLayer("Ground");//設定LayerMask

    }
    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        hp = 300;//設定血量
    }

    //更新事件:每秒執行約60次，60FPS Frame per second
    void Update()
    {
        ShadowOffset();
        //CheckGround();
        JumpKey();
        //Jump();
        //Walk();
        Walk2();

        UpdateJumpAnimator();
        
        Attack();
    }
    //一秒固定50次，物理移動放這裡
    private void FixedUpdate()
    {
        JumpForce();

    }
    private void OnGUI()//called several times per frame
    {
        if (Event.current.rawType == EventType.KeyDown)
        {

            //EventCallBack(Event.current);
            //Debug.Log(Event.current.keyCode);
        }
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
   
    #region 2個要碰撞物件都沒勾選 is Trigger
    //2個物件碰撞執行一次
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print("碰到物體名子" + collision.gameObject.name);//David
        //print("碰到tag名子" + collision.gameObject.tag);//David

        if (collision.gameObject.tag=="Enemy")
        {
            print("碰到物體名子" + collision.gameObject.name);//David
        }

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
    /// 用Input.GetAxisRaw
    /// </summary>
    protected void Walk()
    {

        //****************水平*******************//
        float moveH = Input.GetAxis("Horizontal");//取得-1~1
        float moveHDir = Input.GetAxisRaw("Horizontal");//取得-1、0、1


        //****************垂直走*******************//
        float moveV = Input.GetAxis("Vertical");//取得-1、0、1
        float moveVDir = Input.GetAxisRaw("Vertical");//取得-1、0、1



        // Time.deltaTime:Make it move 10 meters per second instead of 10 meters per frame...
        //****************人物加速度*******************//
        //rig2D.AddForce(new Vector2(moveDir * speedWalk * Time.deltaTime, rig2D.velocity.y));
        //rig2D.velocity = new Vector2(moveDir * speedWalk * Time.deltaTime, rig2D.velocity.y);
        //***************人物加速度上下左右*******************//
        rig2D.velocity = new Vector2(moveH * speedWalk * Time.deltaTime, moveVDir * speedWalk * Time.deltaTime);

        //****************走路動畫*******************//
        //print($"velocity={rig2D.velocity.x}");
        if (Mathf.Abs(moveHDir) > 0)
        {
            animator.SetBool(parWalk, true);
        }
        else
        {
            animator.SetBool(parWalk, false);
        }

        //****************人物轉向*******************//
        if (moveHDir > 0)
        {
            //trans.localScale = new Vector2(1f, trans.localScale.y);//向左改变图像朝向左
            trans.rotation = new Quaternion(trans.rotation.x, 0, trans.rotation.z, trans.rotation.w);
        }
        else if (moveHDir < 0)
        {
            //trans.localScale = new Vector2(-1f, trans.localScale.y);//向左改变图像朝向左
            trans.rotation = new Quaternion(trans.rotation.x, 180, trans.rotation.z, trans.rotation.w);
        }
    }
    /// <summary>
    /// 用Input.GetKey控制上下左右
    /// </summary>
    protected  void Walk2()
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
    /// 判斷是否在地板
    /// </summary>
    private void CheckGround()
    {
        //2D碰撞器=物理.覆蓋型區域(中心點,尺寸,角度,圖層)。transform.position+v3CheckGroundOffset:代表DrawCube位置
        Collider2D hit = Physics2D.OverlapBox(transform.position + v3CheckGroundOffset, v3CheckGroundSize, 0, layerGround);


        //isGround=hit //簡寫hit有東西，就是trues
        if (hit != null)
        {
            //print($"hit是否有撞到東西={hit.name}");
            isGround = true;//在地板
        }
        else
        {
            //print($"不在地板hit是否有撞到東西={hit.name}");
            isGround = false;
        }
    }
    protected  void Jump()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) && isGround)
        {
            print("跳躍");
            //print(gameObject.transform.position);
            gameObject.transform.position += new Vector3(0, pSpeedJump, 0);
            //rig2D.gravityScale = 1;
        }
    }
    /// <summary>
    ///紀錄玩家是否按RightShift(跳躍)，並且落下時把地心引力、y速度都歸0
    /// </summary>
    protected  void JumpKey()
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
    protected  void JumpForce()//案跳躍&&在地板時給向上的力量
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
        //↑ ↓ → ←
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pressEnterTime = Time.time;
            //ctrl↓Enter
            //print($"pressRightControlTime:{pressRightControlTime}");//案右鍵時間
            //print($"pressDownTime:{pressDownTime}");//案下鍵時間
            //print($"pressEnterTime:{pressEnterTime}");//案enter鍵時間
            if (Math.Abs(pressDownTime - pressRightControlTime) < pressInterval3 && Math.Abs(pressEnterTime - pressRightControlTime) < pressInterval5)
            {
                //this.gameObject.GetComponent<CircleCollider2D>().enabled=true;//開啟刀的碰撞
                //this.gameObject.transform.GetChild(0).gameObject.active=true;//開啟子物件
                
                this.gameObject.transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = true;//開啟子物件刀的碰撞
               
                
                /*
                 Transform[] tempTrasforms= this.gameObject.GetComponentsInChildren<Transform>(true);//取得所有子物件
                 int tempIndex=0;//存放找到的GameObject
                 for (int i=0;i<tempTrasforms.Length;i++)
                 {
                     if (tempTrasforms[i].name== "鬼哭斬" )
                     {
                         this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.active = true;//開啟鬼哭斬物件
                         tempIndex = i;
                     }
                 }
                */
                PlayerPrefs.SetInt("鬼哭斬", 1);//1開啟技能，0關閉技能
                Debug.Log("ctrl↓Enter");
                animator.SetTrigger("鬼哭斬");
                //StartCoroutine(WaitCloseGameObject(tempIndex));//等0.5秒關閉GameObject

                
                StartCoroutine(WaitCloseGameObject(1));//等0.5秒關閉GameObject
               
            }
            //ctrl→Enter
            else if (Math.Abs(pressRightTime - pressRightControlTime) < pressInterval3 && Math.Abs(pressEnterTime - pressRightControlTime) < pressInterval5)
            {
                Debug.Log("ctrl→Enter");
                animator.SetTrigger("破空斬");
                //Instantiate(要產生的物體,生成位置
                Instantiate(prefabBullet,this.gameObject.transform.position,Quaternion.identity);//this.gameObject:掛在在此腳本上的物件
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
                StartCoroutine(waitRoll());//等待幾秒

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
                StartCoroutine(waitRoll());//等待幾秒
            }
            else//走路按防禦
            {
                canmove = false;//不能移動
                animator.SetBool(parDefense, true);//開啟防禦動畫
                StartCoroutine(waitDefense());//等待幾秒
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
    //防禦自動解除
    IEnumerator waitDefense()
    {
        yield return new WaitForSeconds(0.5f);//等待幾秒
        animator.SetBool(parDefense, false);////關閉防禦動畫
        canmove = true;//恢復能移動
    }

    //翻滾睡覺
    IEnumerator waitRoll()
    {
        print("翻滾睡覺");
        yield return new WaitForSeconds(0.5f);//等待幾秒
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
    IEnumerator WaitCloseGameObject(int index)
    {
        //print("關閉collider");
        yield return new WaitForSeconds(timeIntervalSkill);//等待幾秒
        //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;//開啟刀的碰撞
        //this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//關閉子物件
       
        this.gameObject.transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = false;//關閉子物件刀的碰撞
       
       // this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//關閉鬼哭斬物件
       // PlayerPrefs.SetInt("鬼哭斬",0);//1開啟技能，0關閉技能
    }




    #region 组合键
    //***********unity 组合键******************//
    private void EventCallBack(Event e)
    {
        //print(e.modifiers & EventModifiers.Control);//Control
        bool eventDown = (e.modifiers & EventModifiers.Control) != 0;

        if (!eventDown)
        {
            Walk2();
        }
        else
        {
            e.Use();        //使用这个事件

            switch (e.keyCode)
            {
                case KeyCode.UpArrow:
                    Debug.Log("按下组合键:ctrl+↑");
                    break;
                case KeyCode.DownArrow:
                    Debug.Log("按下组合键:ctrl+↓");
                    break;
                case KeyCode.LeftArrow:
                    Debug.Log("按下组合键:ctrl+←");
                    break;
                case KeyCode.RightArrow:
                    Debug.Log("按下组合键:ctrl+→");
                    break;
            }
        }
    }

    #endregion


    //**********測試********************//
    void TwoJump()
    {
        if (isGround)//在地面上
        {
            twoJump = 2;
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && twoJump > 0)//能跳躍次數
        {
            rig2D.velocity = Vector2.up * jumpForce;//Vector2.up 等於new Vector2(0,1)
            twoJump -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && twoJump == 0 && isGround)
        {
            rig2D.velocity = Vector2.up * jumpForce;//Vector2.up 等於new Vector2(0,1)

        }
    }
    #endregion
}
