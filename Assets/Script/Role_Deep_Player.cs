using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;//引用AudioMixer程式庫
using System;//有用亂數
using UnityEngine.UI;
namespace chia
{
    public class Role_Deep_Player : Role
    {
        //用velocity控制走路
        [SerializeField, Header("X軸vectory走路速度")]
        private float xSpeedWalk = 1000;
        [SerializeField, Header("X軸vector跑步速度")]
        private float xSpeedRun = 1300;
        [SerializeField, Header("Y軸vectory走路速度")]
        private float ySpeedWalk = 1000;

        //用position控制走路
        [SerializeField, Header("走路速度"), Tooltip("用position控制")]
        protected float pSpeedWalk = 20f;//用position走路
        [SerializeField, Header("跑步速度"), Tooltip("用position控制")]
        protected float pSpeedRun = 40f;//用position跑步路
        //用addforce控制
        [SerializeField, Header("跳躍力量")]
        protected float jumpForce = 400;
        [SerializeField, Header("水平阻力")]
        protected float coefficientOfDrag = 1f;
        [SerializeField, Header("地心引力")]
        protected float gravityScale = 1f;
        
        internal bool clickJump;//是否按跳躍

        protected ObjectPoolBullet_Deep objectPoolBullet;

        
        protected override void Update()
        {
            //print($"xSpeedWalk: {xSpeedWalk}");
            base.Update();
            GetKeyDownTime();
            Walk2();

            //移動位置限制
            gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
            
            Attack();
            JumpKey();
            Jump_FallBackInToPlace();

            #region position 和 localposeion測試
            //父物件   position 等於  localPosition
            //子物件   position:會隨父物件移動   localPosition:固定
            //print("tempTrasforms[1].name : " + tempTrasforms[1].name);
            //print("postion: " + tempTrasforms[1].position);
            //print("local: " + tempTrasforms[1].位置 和);
            #endregion

        }
        //一秒固定50次，物理移動放這裡
        protected  void FixedUpdate()
        {
            JumpForce();
        }
        protected override void Awake()
        {
            base.Awake();
            objectPoolBullet = FindObjectOfType<ObjectPoolBullet_Deep>();//取得子彈物件池
            rig2D.drag = coefficientOfDrag;//水平阻力
        }
        #region 自訂方法

        /// <summary>
        /// 用Input.GetKey控制上下左右
        /// </summary>
        protected override void Walk2()
        {
            //************************GetKey*************************************************//
            //按右鍵
            if (Input.GetKey(KeyCode.RightArrow))
            {
                getKey_RightTime = Time.time;
                pressRight = true;
                //右跑步
                if (getKey_RightTime - getKeyUp_RightTime <= pressInterval && canmove && !stateJump)//偵測案2下 && 能移動 && 不是跳躍狀態
                {
                    print("<Color=yellow>右跑</Color>");
                    stateRun = true;//跑步狀態
                    animator.SetBool(parRun, true);//開啟跑步動畫
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x + pSpeedRun*Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(xSpeedRun * Time.deltaTime, rig2D.velocity.y);
                    getKeyUp_RightTime = Time.time;//跑步狀態中要持續更新鬆鍵時間
                }
                else if (canmove && !stateJump)//走路: 能移動&&不是跳躍狀態
                {
                    stateWalk = true;//走路狀態
                    print("<Color=red>右走</Color>");
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x + pSpeedWalk * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(xSpeedWalk * Time.deltaTime, rig2D.velocity.y);
                    animator.SetBool(parWalk, true);
                }
               

                //向右轉向
                gameObject.transform.rotation = new Quaternion(trans.rotation.x, 0, trans.rotation.z, trans.rotation.w);
            }
            //鬆右鍵
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                pressRight = false;
                getKeyUp_RightTime = Time.time;
                
                    
                if (canmove && !stateJump)//不是跳躍狀態鬆右鍵:能移動 && 不是跳躍狀態
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(0, rig2D.velocity.y);//直接把X速度歸零,Y軸保持原速
                }
                else//跳躍狀態鬆右鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//X軸保持原速,Y軸保持原速
                }
                    stateRun = false;//關掉跑步狀態
                    stateWalk = false;//走路狀態
                    animator.SetBool(parWalk, false);//關閉走路動畫
                    animator.SetBool(parRun, false);//關閉跑步動畫
            }

            //按左鍵
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pressLeft = true;//是否右按下左鍵
                getKey_LeftTime = Time.time;
                //左跑步
                if (getKey_LeftTime - getKeyUp_LeftTime <= pressInterval && canmove && !stateJump)
                {
                    print("<Color=yellow>左跑</Color>");
                    stateRun = true;//跑步狀態
                    animator.SetBool(parRun, true);//開啟跑步動畫
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x - pSpeedRun * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(-xSpeedRun * Time.deltaTime, rig2D.velocity.y);
                    getKeyUp_LeftTime = Time.time;//持續更新鬆鍵
                }
                else if (canmove && !stateJump)//走路
                {
                    stateWalk = true;//走路狀態
                    print("<Color=red>左走</Color>");
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x - pSpeedWalk * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(-xSpeedWalk * Time.deltaTime, rig2D.velocity.y);
                    animator.SetBool(parWalk, true);
                }
                //向左轉向
                gameObject.transform.rotation = new Quaternion(trans.rotation.x, 180, trans.rotation.z, trans.rotation.w);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                pressLeft = false;//是否右按下左鍵
                getKeyUp_LeftTime = Time.time;

                    
                if (canmove && !stateJump)//能移動 && 不是跳躍狀態:不是跳躍狀態鬆右鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(0, rig2D.velocity.y);
                }
                else//跳躍狀態按上鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);
                }
                stateRun = false;//關掉跑步狀態
                    stateWalk = false;//走路狀態
                    animator.SetBool(parWalk, false);
                    animator.SetBool(parRun, false);

                getKeyUp_LeftTime = Time.time;
            }

            //上鍵
            if (Input.GetKey(KeyCode.UpArrow))
            {
                getKey_UpTime = Time.time;
                pressUp = true;
                if (canmove && !stateJump)//能移動 && 不是跳躍狀態
                {
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + pSpeedWalk * Time.deltaTime, minY, maxY), transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, ySpeedWalk * Time.deltaTime);
                    animator.SetBool(parWalk, true);
                }
                else//跳躍狀態:按上鍵不要再加Y軸速度
                {
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);
                }
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) )
            {
                pressUp = false;
                getKeyUp_UpTime = Time.time;
                if (canmove && !stateJump)//能移動 && 不是跳躍狀態:不是跳躍狀態按上鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, 0);//Y軸速度歸0
                }
                else//跳躍狀態鬆上鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//因為跳躍時會有y軸addforced，不能把Y速度設成0，故用原y軸速度
                }
                animator.SetBool(parWalk, false);

            }
            //下鍵
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pressDown = true;
                getKey_DownTime = Time.time;
                //print($"getKey_DownTime: {getKey_DownTime}");
                if (canmove && !stateJump)//能移動 && 不是跳躍狀態
                {
                    #region 用position控制走路
                    //gameObject.transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y - pSpeedWalk * Time.deltaTime, minY, maxY), transform.position.z);
                    #endregion
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, -ySpeedWalk * Time.deltaTime);
                    animator.SetBool(parWalk, true);
                }  
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                pressDown = false;
                getKeyUp_DownTime = Time.time;
                if (canmove && !stateJump)//能移動 && 不是跳躍狀態:不是跳躍狀態按上鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, 0);//Y軸速度歸0
                }
                else//跳躍狀態按上鍵
                {
                    //用velocity控制走路
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//因為跳躍時會有y軸addforced，和加地心引力，故用原y軸速度
                }
                animator.SetBool(parWalk, false);
                    //getKeyUp_DownTime = Time.time;
            }
        }

        /// <summary>
        /// 攻擊
        /// </summary>
        protected override void Attack()
        {
            //*********************Enter******************************//
            //↑ ↓ → ←
            if (Input.GetKeyDown(KeyCode.Return))//按下Enter
            {
                getKeyDown_EnterTime = Time.time;
                //print($"pressEnterTime: {getKeyDown_EnterTime}");

                //ctrl↓Enter
                //print($"getKeyDown_RightControlTime:{getKeyDown_RightControlTime}");//案右鍵時間
                //print($"getKeyDown_DownTime:{getKeyDown_DownTime}");//案下鍵時間
                //print($"getKeyDown_EnterTime:{getKeyDown_EnterTime}");//案enter鍵時間

                if (!stateInjuried)//不是受傷狀態。才能攻擊
                {


                    //Deep_鬼哭斬  防↓攻
                    //Math.Abs(getKeyDown_DownTime - pressRightControlTime) < pressInterval3。判斷按鍵間格時間
                    //getKeyDown_DownTime - pressRightControlTime>0。判斷先後順序(下鍵要後按)
                    if (Math.Abs(getKeyDown_DownTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_DownTime - getKeyDown_RightControlTime > 0 && getKeyDown_EnterTime - getKeyDown_DownTime > 0)//按下鍵和Ctrl間格時間<pressInterval3 && 按Enter和Ctrl時間 <pressInterval5 && 下鍵要在Ctrl後面 && Enter要在下鍵後面
                    {
                        //this.gameObject.GetComponent<CircleCollider2D>().enabled=true;//開啟刀的碰撞
                        //this.gameObject.transform.GetChild(0).gameObject.active=true;//開啟子物件
                        //this.gameObject.transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = true;//開啟子物件刀的碰撞
                        int tempIndex = 0;//存放找到的GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)
                        {
                            if (tempTrasforms[i].name == "Deep_鬼哭斬")
                            {
                                tempTrasforms[i].gameObject.SetActive(true);//開啟Deep_鬼哭斬物件
                                tempIndex = i;
                            }
                        }
                        Debug.Log("ctrl↓Enter");
                        animator.SetBool(parSkill01,true);
                        aud.PlayOneShot(deep_sf2);//音效
                        StartCoroutine(waitSkill01(tempIndex));//等0.5秒關閉GameObject
                        
                    }
                    //ctrl→Enter
                    //Deep_破空斬
                    //右鍵一定要是最後案getKey_RightTime - getKey_LeftTime >0
                    else if (Math.Abs(getKeyDown_RightTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_RightTime - getKeyDown_RightControlTime > 0 && getKeyDown_EnterTime - getKeyDown_RightTime > 0 && !stateJump)
                    {
                        Debug.Log("ctrl→Enter");
                        animator.SetTrigger("破空斬");
                        aud.PlayOneShot(deep_sf1);//音效
                        PlayerPrefs.SetInt("Deep_破空斬方向", 1);
                        //Instantiate(要產生的物體,生成位置,角度)
                        //Instantiate(prefabBullet, this.gameObject.transform.position, Quaternion.identity);//this.gameObject:掛在在此腳本上的物件

                        //使用物件池
                        GameObject tempobjectPoolBullet = objectPoolBullet.GetPoolObject(WhoAttack.playerAttack);//傳子彈是誰的攻擊
                        tempobjectPoolBullet.transform.position = this.gameObject.transform.position;
                        tempobjectPoolBullet.transform.eulerAngles = new Vector3(0, 0, 0);//轉角度
                        tempobjectPoolBullet.GetComponent<BulletController>().direction = 1;//指定物件池中生成物方向
                    }
                    //ctrl←Enter
                    //Deep_破空斬
                    //左鍵一定要是最後案getKey_RightTime - getKey_LeftTime < 0
                    else if (Math.Abs(getKeyDown_LeftTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_LeftTime- getKeyDown_RightControlTime>0 && getKeyDown_EnterTime - getKeyDown_LeftTime > 0 && !stateJump)
                    {
                        Debug.Log("ctrl←Enter");
                        animator.SetTrigger("破空斬");
                        aud.PlayOneShot(deep_sf1);//音效
                        PlayerPrefs.SetInt("Deep_破空斬方向", -1);
                        //Instantiate(要產生的物體,生成位置,角度
                        //Instantiate(prefabBullet, this.gameObject.transform.position, Quaternion.Euler(0, 180, 0));//轉角度，朝向左邊

                        //使用物件池
                        GameObject tempobjectPoolBullet = objectPoolBullet.GetPoolObject(WhoAttack.playerAttack);//子彈是誰的攻擊
                        tempobjectPoolBullet.transform.position = this.gameObject.transform.position;//位置
                        tempobjectPoolBullet.transform.eulerAngles = new Vector3(0, 180, 0);//轉角度
                        tempobjectPoolBullet.GetComponent<BulletController>().direction = -1;//指定物件池中生成物方向
                    }
                    else if (stateRun && !stateRoll)//跑步時攻擊:跑步狀態 && 不適翻滾狀態
                    {
                        animator.SetBool(parRunAtack, true);
                        stateRunAttack = true;
                        int tempIndex = 0;//存放找到的GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)//找子物件
                        {
                            if (tempTrasforms[i].name == "Deep_跑步時攻擊")
                            {
                                this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//開啟Deep_跑步時攻擊
                                tempIndex = i;
                            }
                        }
                        StartCoroutine(waitRunAttack(tempIndex));//等0.3秒關閉跑步攻擊動畫

                    }
                    else //普攻Enter(攻擊)
                    {
                        //print(random.Next(2));//0、1
                        //print(random.Next(1,3));//1、2
                        i01 = random.Next(1, 3);//隨機攻擊動作
                        int tempIndex = 0;//存放找到的GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)//找子物件
                        {
                            if (tempTrasforms[i].name == "Deep_普攻")
                            {
                                this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//開啟Deep_普攻物件
                                tempIndex = i;
                            }
                        }
                        if (!stateAttack)//不是普通攻擊狀態(不然一直按會一直攻擊，攻擊動畫還沒做完就又攻擊)
                        {
                            stateAttack = true;//普通攻擊狀態
                            if (i01 == 1)
                            {
                                //Debug.Log("Attack1"+temp);
                                //animator.SetTrigger(parTriggerAttack1);
                                animator.SetBool(parAttack1, true);
                            }
                            else
                            {
                                //Debug.Log("Attack2"+temp);
                                //animator.SetTrigger(parTriggerAttack2);
                                animator.SetBool(parAttack2, true);
                            }
                            aud.PlayOneShot(deep_sf0);//音效
                            StartCoroutine(waitAttack(tempIndex));//等0.3秒關閉攻擊動畫
                        }

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
                getKeyDown_RightControlTime = Time.time;
                //print($"getKeyDown_RightControlTime: {getKeyDown_RightControlTime}");
                //右翻滾
                if (stateRun && pressRight && !stateJump)//跑步時&&向右&&不是跳躍狀態
                {
                    //print("右翻滾");
                    stateRoll = true;//翻滾中
                    moveShadow = false;//影子y軸不要動
                    originalY = transform.position.y;//紀錄人物位置，因為翻滾會變低
                    animator.SetBool(parRoll, true);
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                    canmove = false;//不能移動
                    rig2D.velocity = new Vector2(speedRoll * Time.deltaTime, rig2D.velocity.y);
                    StartCoroutine(waitRoll());//等待幾秒

                }
                //左翻滾
                else if (stateRun && pressLeft && !stateJump)//跑步時&&向左&&不是跳躍狀態
                {
                    //print("左翻滾");
                    stateRoll = true;//翻滾中
                    moveShadow = false;//影子y軸不要動
                    originalY = transform.position.y;//紀錄人物位置，因為翻滾會變低
                    animator.SetBool(parRoll, true);
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                    canmove = false;//不能移動
                    rig2D.velocity = new Vector2(-speedRoll * Time.deltaTime, rig2D.velocity.y);
                    StartCoroutine(waitRoll());//等待幾秒
                }
                else if(!stateRun && !stateRoll && !stateJump)//走路和原地按防禦
                {

                        canmove = false;//不能移動
                        stateDefense = true;
                        //用velocity控制走路
                        rig2D.velocity = new Vector2(0, rig2D.velocity.y);
                        animator.SetBool(parDefense, true);//開啟防禦動畫
                        StartCoroutine(waitDefense());//等待幾秒
                }
            }
            if (Input.GetKeyUp(KeyCode.RightControl))
            {
                getKeyUp_RightControlTime = Time.time;
                animator.SetBool(parDefense, false);//關閉防禦動畫
            }
            //*********************RightShiftTime******************************//
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                getKeyDown_RightShifTime = Time.time;
            }
        }
        /// <summary>
        /// GetKeyDown按鍵時間
        /// </summary>
        private void GetKeyDownTime()
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))   getKeyDown_UpTime = Time.time;
            if(Input.GetKeyDown(KeyCode.DownArrow)) getKeyDown_DownTime = Time.time;
            if(Input.GetKeyDown(KeyCode.LeftArrow)) getKeyDown_LeftTime = Time.time;
            if(Input.GetKeyDown(KeyCode.RightArrow))getKeyDown_RightTime = Time.time;
            //print($"getKeyDown_RightTime: {getKeyDown_RightTime}");
        }
        /// <summary>
        ///紀錄玩家是否按RightShift(跳躍)，並且落下時把地心引力、y速度都歸0
        /// </summary>
        protected void JumpKey()
        {

            if (Input.GetKeyDown(KeyCode.RightShift) && !stateJump)
            {
                print("跳躍");
                clickJump = true;
            }
        }
        /// <summary>
        /// 有按下RightShift(跳躍鍵)，給向上推力，並把地心引力設為1
        /// </summary>
        protected void JumpForce()//案跳躍&&在地板時給向上的力量
        {
            if (clickJump && !stateJump && !stateRoll)//有按rightShift && 不是跳躍狀態 && 不翻滾   
            {

                moveShadow = false;//影子不要位移
                canmove = false;
                originalY = transform.position.y;//記錄跳起的位置
                stateJump = true;
                animator.SetBool(parJump, stateJump);

                //跳躍時把Y軸速度設成0，因為 if(Input.GetKey(KeyCode.UpArrow))按上鍵那邊，速度向上右加推力會跳很高
                rig2D.velocity = new Vector2(rig2D.velocity.x,0);
                rig2D.AddForce(new Vector2(0, jumpForce));//給跳躍力量
                rig2D.gravityScale = gravityScale;//跳起後地心引力設1
                clickJump = false;
            }
        }
        #endregion
    }
}