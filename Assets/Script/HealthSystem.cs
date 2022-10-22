using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace chia
{
    public class HealthSystem : MonoBehaviour
    {
        #region �ݩ�
        protected Role scriptRole;

        [SerializeField,Header("��q���")]
        protected DataHealth datahealth;
        [SerializeField, Header("UI���")]
        protected Image imageHP;
        [SerializeField, Header("Image_�C����������")]
        protected CanvasGroup GameObject_GameOver;
       
        protected float hp;//�C������q
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

        //public float Hp=>hp;//���~�ɩI�s���o
        public float Hp//���~�ɩI�s���o
        {
            get => hp;//���~�ɩI�s���o
            //protected set { hp = value; }//�p��
        }
        protected string parInjuried = "Injuried";//�������
        protected string parInjuriedUp = "Injuried_up";//���ˤW��
        protected Animator animator; //deep���ʵe
        protected string parDeath = "Death";//���`
        protected AttackSystem attackSystem;
        protected float WaitInjuried_Millisecond = 0.5f;//���ˡB�W�����ݮɶ�
        protected float WaitInjuriedUp_Millisecond = 0.5f;//���ˡB�W�����ݮɶ�
        #endregion

        #region unity��k
        private void Awake()
        {
            hp = datahealth.hp;
            animator = GetComponent<Animator>();
            attackSystem = GetComponent<AttackSystem>();
            scriptRole = GetComponent<Role>();
            
            
        }
        #endregion






        #region �ۭq��k
        /// <summary>
        /// ��a����
        /// </summary>
        /// <param name="damage"></param>
        internal  void Injuried(float damage)
        {
            if (scriptRole.stateDefense || scriptRole.stateRoll) return;//���m��½�u���A�������X�A���n�Q��

                this.hp -= damage;
                imageHP.fillAmount = hp / datahealth.hpMax;//UI������

                if (hp <= 0)
                {
                    Dead();//���`
                }
                else
                {
                    this.animator.SetBool(parInjuried, true);//�}�Ҩ��˰ʵe
                    scriptRole.canmove = false;//���ಾ��

                    scriptRole.stateInjuried = true;//���˪��A�}��
                    scriptRole.stateInjuried_Insitu = true;//��a���˪��A�}��
                    //print("<Color=red> stateInjuried_Insitu true </Color>" + Time.time);
                    StartCoroutine(WaitInjuried());
                }
        }

        /// <summary>
        /// �W������
        /// </summary>
        /// <param name="damage"></param>
        internal  void InjuriedUp(float damage)
        {
            if (scriptRole.stateDefense || scriptRole.stateRoll) return;//���m��½�u���A�������X�A���n�Q��

            if (!scriptRole.stateInjuriedUp)//���O�W�����A�~��Q�W����������
            {
                this.hp -= damage;
                imageHP.fillAmount = hp / datahealth.hpMax;//UI������
                if (hp <= 0)
                {
                    Dead();//���`
                }
                else
                {
                    //print("�Q�����٥�");
                    scriptRole.stateInjuried = true;//���˪��A�}��
                    scriptRole.stateInjuriedUp = true;//���ˤW�����A
                    this.animator.SetBool(parInjuriedUp, true);//�}�Ҩ��ˤW���ʵe
                    scriptRole.moveShadow = false;//�v�l���n�첾
                    scriptRole.originalY = transform.position.y;//�O�����_����m
                    scriptRole.rig2D.velocity = new Vector2(0, scriptRole.speedInjuriedUp);//���W�t��
                    scriptRole.canmove = false;
                    StartCoroutine(WaitInjuriedUp());

                }
            }
        }
        //���˵���
        IEnumerator WaitInjuried()
        {

            yield return new WaitForSeconds(WaitInjuried_Millisecond);//���ݴX��
            this.animator.SetBool(parInjuried, false);//�������˰ʵe
            scriptRole.canmove = true;//�i�H����
            scriptRole.stateInjuried = false;//���˪��A����
            scriptRole.stateInjuried_Insitu = false;//�D���˪��A
            //print("<Color=blue> �������˰ʵe </Color>" + Time.time);
        }

        //�W������
        protected IEnumerator WaitInjuriedUp()
        {
            yield return new WaitForSeconds(WaitInjuriedUp_Millisecond);//���ݴX��
            scriptRole.stateInjuriedUp_falling = true;//���ˤW����̰��I���A�C�]������|�}�Ҧa�ߤޤO�A�ҥH�o�̥i�H�]���̰��I���A
            scriptRole.rig2D.gravityScale = 1;//���_��a�ߤޤO�]1
            //�bRole.InjuriedUp_FallBackInToPlace �|�P�_�O�_���a�A�~��stateInjuriedUp�]false
        }

        /// <summary>
        /// ���`
        /// </summary>
        protected virtual void Dead()
        {
            hp = 0;
            scriptRole.stateDead = true;
            animator.SetBool(parDeath, true);//���`�ʵe
            print("���`�ʵe���C��m");

            if (!scriptRole.stateInjuriedUp_falling)//�o�̭n�P�_�A���M�W�����A�Q���𥴦��A�H���|�@�����U�h
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 2.57f);//���`�ʵe���C��m
            }

            //print("�C�������e��");
            scriptRole.shadow.SetActive(false);//���`�������v�l
            scriptRole.GetComponent<Collider2D>().enabled=false;//�����I��
            scriptRole.enabled = false;
            if (!scriptRole.stateOpenGameOverImage)//���OGameOver���A�~��}��
            {
                scriptRole.stateOpenGameOverImage = true;//�e���}�Ҥ�
                StartCoroutine(CavasGroupGameOver());
            }

        }

       

        /// <summary>
        /// �C������Canvasgroup��ܲH�J
        /// </summary>
        /// <returns></returns>
        IEnumerator CavasGroupGameOver(bool fadeIn = true)
        {
            GameObject_GameOver.gameObject.SetActive(true);//�}�Ҫ���
            //�T���B��l
            //���L��?���L�Ȭ� true:���L�Ȭ� false
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