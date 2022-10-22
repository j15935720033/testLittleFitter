using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace chia
{
    public class HealthSystem : MonoBehaviour
    {
        #region 屬性
        protected Role scriptRole;

        [SerializeField,Header("血量資料")]
        protected DataHealth datahealth;
        [SerializeField, Header("UI血條")]
        protected Image imageHP;
        [SerializeField, Header("Image_遊戲結束底圖")]
        protected CanvasGroup GameObject_GameOver;
       
        protected float hp;//遊戲中血量
        /*protected float hp
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                
            }
        }*/

        //public float Hp=>hp;//給外界呼叫取得
        public float Hp//給外界呼叫取得
        {
            get => hp;//給外界呼叫取得
            //protected set { hp = value; }//私有
        }
        protected string parInjuried = "Injuried";//普攻受傷
        protected string parInjuriedUp = "Injuried_up";//受傷上飛
        protected Animator animator; //deep的動畫
        protected string parDeath = "Death";//死亡
        protected AttackSystem attackSystem;
        protected float WaitInjuried_Millisecond = 0.5f;//受傷、上飛等待時間
        protected float WaitInjuriedUp_Millisecond = 0.5f;//受傷、上飛等待時間
        #endregion

        #region unity方法
        private void Awake()
        {
            hp = datahealth.hp;
            animator = GetComponent<Animator>();
            attackSystem = GetComponent<AttackSystem>();
            scriptRole = GetComponent<Role>();
            
            
        }
        #endregion






        #region 自訂方法
        /// <summary>
        /// 原地受傷
        /// </summary>
        /// <param name="damage"></param>
        internal  void Injuried(float damage)
        {
            if (scriptRole.stateDefense || scriptRole.stateRoll) return;//防禦或翻滾狀態直接跳出，不要被打

                this.hp -= damage;
                imageHP.fillAmount = hp / datahealth.hpMax;//UI中扣血

                if (hp <= 0)
                {
                    Dead();//死亡
                }
                else
                {
                    this.animator.SetBool(parInjuried, true);//開啟受傷動畫
                    scriptRole.canmove = false;//不能移動

                    scriptRole.stateInjuried = true;//受傷狀態開啟
                    scriptRole.stateInjuried_Insitu = true;//原地受傷狀態開啟
                    //print("<Color=red> stateInjuried_Insitu true </Color>" + Time.time);
                    StartCoroutine(WaitInjuried());
                }
        }

        /// <summary>
        /// 上飛受傷
        /// </summary>
        /// <param name="damage"></param>
        internal  void InjuriedUp(float damage)
        {
            if (scriptRole.stateDefense || scriptRole.stateRoll) return;//防禦或翻滾狀態直接跳出，不要被打

            if (!scriptRole.stateInjuriedUp)//不是上飛狀態才能被上飛攻擊打到
            {
                this.hp -= damage;
                imageHP.fillAmount = hp / datahealth.hpMax;//UI中扣血
                if (hp <= 0)
                {
                    Dead();//死亡
                }
                else
                {
                    //print("被鬼哭斬打");
                    scriptRole.stateInjuried = true;//受傷狀態開啟
                    scriptRole.stateInjuriedUp = true;//受傷上飛狀態
                    this.animator.SetBool(parInjuriedUp, true);//開啟受傷上飛動畫
                    scriptRole.moveShadow = false;//影子不要位移
                    scriptRole.originalY = transform.position.y;//記錄跳起的位置
                    scriptRole.rig2D.velocity = new Vector2(0, scriptRole.speedInjuriedUp);//往上速度
                    scriptRole.canmove = false;
                    StartCoroutine(WaitInjuriedUp());

                }
            }
        }
        //受傷等待
        IEnumerator WaitInjuried()
        {

            yield return new WaitForSeconds(WaitInjuried_Millisecond);//等待幾秒
            this.animator.SetBool(parInjuried, false);//關閉受傷動畫
            scriptRole.canmove = true;//可以移動
            scriptRole.stateInjuried = false;//受傷狀態關閉
            scriptRole.stateInjuried_Insitu = false;//非受傷狀態
            //print("<Color=blue> 關閉受傷動畫 </Color>" + Time.time);
        }

        //上飛等待
        protected IEnumerator WaitInjuriedUp()
        {
            yield return new WaitForSeconds(WaitInjuriedUp_Millisecond);//等待幾秒
            scriptRole.stateInjuriedUp_falling = true;//受傷上飛到最高點狀態。因為之後會開啟地心引力，所以這裡可以設為最高點狀態
            scriptRole.rig2D.gravityScale = 1;//跳起後地心引力設1
            //在Role.InjuriedUp_FallBackInToPlace 會判斷是否落地，才把stateInjuriedUp設false
        }

        /// <summary>
        /// 死亡
        /// </summary>
        protected virtual void Dead()
        {
            hp = 0;
            scriptRole.stateDead = true;
            animator.SetBool(parDeath, true);//死亡動畫
            print("死亡動畫降低位置");

            if (!scriptRole.stateInjuriedUp_falling)//這裡要判斷，不然上飛中，被普攻打死，人物會一直掉下去
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 2.57f);//死亡動畫降低位置
            }

            //print("遊戲結束畫面");
            scriptRole.shadow.SetActive(false);//死亡時關閉影子
            scriptRole.GetComponent<Collider2D>().enabled=false;//關閉碰撞
            scriptRole.enabled = false;
            if (!scriptRole.stateOpenGameOverImage)//不是GameOver狀態才能開啟
            {
                scriptRole.stateOpenGameOverImage = true;//畫布開啟中
                StartCoroutine(CavasGroupGameOver());
            }

        }

       

        /// <summary>
        /// 遊戲結束Canvasgroup顯示淡入
        /// </summary>
        /// <returns></returns>
        IEnumerator CavasGroupGameOver(bool fadeIn = true)
        {
            GameObject_GameOver.gameObject.SetActive(true);//開啟物件
            //三元運算子
            //布林直?布林值為 true:布林值為 false
            float increase = fadeIn ? 0.1f : -0.1f;
            for (int i = 0; i < 10; i++)
            {

                GameObject_GameOver.alpha += increase;
                //print(i);
                yield return new WaitForSeconds(scriptRole.intervalFadIn);
            }
        }
       
        #endregion

    }
}