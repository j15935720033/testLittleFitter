using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;//�ޥ�AudioMixer�{���w
using System;//���ζü�
using UnityEngine.UI;
using BehaviorTree;
namespace chia
{


    public class Role : MonoBehaviour
    {
        #region �ݩ� �Q��

        [SerializeField, Header("�Q���W��")]
        internal float speedInjuriedUp = 3;
        [SerializeField, Header("�H�J�ɶ�")]
        internal float intervalFadIn = 0.5f;
        internal bool stateOpenGameOverImage;//���`�e�����A

        internal bool stateInjuried;//true:���ˤ� false:�S����
        internal bool stateInjuried_Insitu;//true:��a���ˤ� false:�S����
        internal bool stateInjuriedUp;//true:���ˤW����
        internal bool stateInjuriedUp_falling;////true:���ˤW�����A�̰��I�츨�a�e
        internal bool stateDead;////true:���`

        protected string parInjuried = "Injuried";//��a����
        protected string parInjuriedUp = "Injuried_up";//��a����
        protected string parDeath = "death";//���`


        #endregion

        #region  �ݩ�
       
        
        [SerializeField, Header("½�u�t��"), Tooltip("��velocivy����")]//��velocivy����
        protected float speedRoll = 100;
        [SerializeField, Header("�ˬd�a�O�ؤo")]
        protected Vector3 v3CheckGroundSize = new Vector3(3.61f, 0.27f, 0);
        [SerializeField, Header("�ˬd�a�O�첾")]
        protected Vector3 v3CheckGroundOffset = new Vector3(0.02f, -1.7f, 0);
        [SerializeField, Header("�ˬdShadow�첾")]
        protected Vector3 v3CheckGroundOffsetShadow = new Vector3(0f, -3.71f, 0);
        [SerializeField, Header("�ˬd�a�O�C��")]
        protected Color colorCheckGround = new Color(1, 0, 0.2f, 0.5f);
        [SerializeField, Header("�ˬd�a�O�ϼh")]
        protected LayerMask layerGround;
        [SerializeField, Header("�v�l")]
        internal GameObject shadow;

        
        [SerializeField, Header("����_����")]
        protected AudioClip deep_sf0;
        [SerializeField, Header("����_�}�ű�")]
        protected AudioClip deep_sf1;
        [SerializeField, Header("����_������")]
        protected AudioClip deep_sf2;
        [SerializeField, Header("����")]
        protected GameObject role;
        protected Animator animator; //deep���ʵe
        internal Rigidbody2D rig2D;
        protected Transform trans;//deep��trans
        protected Transform transShadow;//�v�l��trans
        protected Collider2D coll2D;

        protected Transform[] tempTrasforms; //���o�񦹸}����GameObject�A�Ҧ��l����(�]�t�ۤv)
        protected AudioSource aud;//������

        protected string parWalk = "Walk";
        protected string parRun = "Run";
        protected string parJump = "Jump";
        protected String parDefense = "Defense";
        protected String parTriggerRoll = "TriggerRoll";
        protected String parRoll = "Roll";
        protected string parAttack1 = "Attack1";
        protected string parAttack2 = "Attack2";
        protected String parRunAtack = "RunAttack";
        protected String parSkill01 = "������";
        
        internal bool canmove = true;//�O�_�ಾ��
        internal bool moveShadow = true;//���ʼv�l

        
        internal bool stateJump;//���D���A�Ctrue:���D���A(������D),false:�D���D���A(����D)
        protected bool stateWalk;//true:������  false:�S����
        protected bool stateRun;//true:�]�B��  false:�S�]�B
        internal bool stateRoll;//true:½�u��  false:�S½�u
        protected bool stateAttack;//true:������  false:�S����
        protected bool stateRunAttack;//true:�]�B�ɧ������A  false:�S�]�B�ɧ������A
        internal bool stateDefense;//true:���m���A  
        //���ʬO��getKey�P�_
        //�����ޯ౵�ޥ�getKeyDown�P�_
        protected float getKey_UpTime;//����������W��
        protected float getKeyUp_UpTime;//�P�W��
        protected float getKeyDown_UpTime;//���@���W��
        protected bool pressUp;//�O�_���W��

        protected float getKey_DownTime;//����������U��
        protected float getKeyUp_DownTime;//�P�U��
        protected float getKeyDown_DownTime;//���@���U��
        protected bool pressDown;//�O�_���U��

        protected float getKey_LeftTime;//�������������
        protected float getKeyUp_LeftTime;//�P����
        protected float getKeyDown_LeftTime;//���@������
        protected bool pressLeft;//�O�_������

        protected float getKey_RightTime;//����������k��
        protected float getKeyUp_RightTime;//�P�k��
        protected float getKeyDown_RightTime;//���@���k��
        protected bool pressRight;//�O�_���k��

        protected float getKeyUp_EnterTime;//�PEnter��
        protected float getKeyDown_EnterTime;//���@���PEnter����
        protected bool pressEnter;//�O�_���PEnter����

        protected float getKeyUp_RightControlTime;//�PRightControl��
        protected float getKeyDown_RightControlTime;//���@���PRightControl��
        protected bool pressRightControl;//�O�_���PRightControl��

        protected float getKeyUp_RightShifTime;//�PRightShif��
        protected float getKeyDown_RightShifTime;//���@���PRightShif��
        protected bool pressRightShif;//�O�_���PRightShif��



        


        protected float pressInterval = 0.1f;//�϶����0.1
        protected float pressInterval3 = 0.3f;//�϶����0.3
        protected float pressInterval5 = 0.5f;//�϶����0.2




        internal float timeIntervalSkill = 0.5f;//�ɶ����j
        internal float wait100 = 0.2f;//0.1��
        internal float wait200 = 0.2f;//0.2��
        internal float wait300 = 0.3f;//0.3��
        internal float wait500 = 0.5f;//0.5��
        internal float wait1000 = 0.5f;//1��
        internal float originalY;//�������_½�u�ɡA�쥻y����m
        internal System.Random random;
        internal int i01;//random����
        

        
        //���ʽd�򭭨�
        [SerializeField, Header("minX")]
        protected float minX = -41.1f;
        [SerializeField, Header("maxX")]
        protected float maxX = 44f;
        [SerializeField, Header("minY")]
        protected float minY = -11.3f;
        [SerializeField, Header("maxY")]
        protected float maxY = 21f;
        #endregion

        #region �ƥ�:�{���J�f
        //����ƥ�:�}�l�ƥ�e����@���A����B�}�Ҥ]�|����A���o���󵥵�
        protected virtual void Awake()
        {
            animator = role.GetComponent<Animator>();
            rig2D = role.GetComponent<Rigidbody2D>();
            trans = role.GetComponent<Transform>();
            transShadow = shadow.GetComponent<Transform>();
            //print(LayerMask.NameToLayer("Ground"));
            layerGround.value = LayerMask.GetMask("Ground");//�]�wLayerMask
                                                            //deep.layer=LayerMask.NameToLayer("Ground");//�]�wLayerMask
            tempTrasforms = this.gameObject.GetComponentsInChildren<Transform>(true);//���o�񦹸}����GameObject�A�Ҧ��l����(�]�t�ۤv)
            aud = role.GetComponent<AudioSource>();

        }
        // Start is called before the first frame update
        protected virtual void Start()
        {
            random = new System.Random();

        }

        //��s�ƥ�:�C������60���A60FPS Frame per second
         protected virtual void Update()
        {
            ShadowOffset();
            
            #region �Q��
            //�P�_�Q��W���A�n�����^��a
            InjuriedUp_FallBackInToPlace();
            #endregion
        }
        #endregion

        #region unity��k

        /// <summary>
        /// OnDrawGizmos:ø�s�ϥ�
        /// </summary>
        private void OnDrawGizmos()
        {
            //1.�M�w�C��
            Gizmos.color = colorCheckGround;

            //2.ø�s�ϥ�
            //trans.position ��e����y��trans
            Gizmos.DrawCube(transform.position + v3CheckGroundOffset, v3CheckGroundSize);
            //Gizmos.DrawCube(trans.position + v3CheckGroundOffset, v3CheckGroundSize);
        }
        #region �Q��



        #endregion
        #endregion

        #region �ۭq��k
        /// <summary>
        /// ����
        /// </summary>
        protected virtual void Walk2()
        {

        }




        
        /// <summary>
        /// ���D��n���^��a�P�_
        /// </summary>
        protected void Jump_FallBackInToPlace()
        {
            //print(temeV3);
            //print("transform" + transform.position);
            if (transform.position.y <= originalY && stateJump && !stateRoll)//���U�ɪ���m�A�p�󵥩�_���I�� && ���D���A && ��½�u�C�����a�ߤޤO�My�t��
            {
                //print("HI");
                canmove = true;
                rig2D.gravityScale = 0;
                rig2D.velocity = new Vector2(0, 0);//�I�켲�ɤ]�|���ϦV�@�ΤO���t�סA�]�������O��y�b���[�t�ץΡA�|�~�򱼸��A�ҥHy�b�t�׭n��0
                stateJump = false;
                animator.SetBool(parJump, stateJump);//�������D
                moveShadow = true;//�v�l����
            }

        }

        /// <summary>
        /// ����circle(�v�l)�A��ۤH������
        /// </summary>
        private void ShadowOffset()
        {
            if (moveShadow)
            {
                transShadow.position = transform.position + v3CheckGroundOffsetShadow;
            }
            else//���_�Ӽv�ly�b���n��
            {
                transShadow.position = new Vector2(transform.position.x + v3CheckGroundOffsetShadow.x, originalY + v3CheckGroundOffsetShadow.y);
            }

        }
        /// <summary>
        /// ����
        /// </summary>
        protected virtual void Attack()
        {

        }

        //���m�۰ʸѰ�
        protected IEnumerator waitDefense()
        {
            yield return new WaitForSeconds(wait1000);//���ݴX��
            animator.SetBool(parDefense, false);////�������m�ʵe
            stateDefense = false;
            canmove = true;//��_�ಾ��
        }
        //��ı_����
        protected IEnumerator waitAttack(int index)
        {
            yield return new WaitForSeconds(wait500);//���ݴX��
            stateAttack = false;//���O���q�������A
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//����Deep_���𪫥�
            animator.SetBool(parAttack1, false);////��������1�ʵe
            animator.SetBool(parAttack2, false);////��������2�ʵe
        }
        //��ı_����
        protected IEnumerator waitSkill01(int index)
        {
            yield return new WaitForSeconds(wait500);//���ݴX��
            stateAttack = false;//���O���q�������A
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//����Deep_���𪫥�
            animator.SetBool(parSkill01, false);////��������1�ʵe
        }

        //����_�]�B�ɧ���
        protected IEnumerator waitRunAttack(int index)
        {
            yield return new WaitForSeconds(wait300);//���ݴX��
            stateRunAttack = false;//���O�]�B���q�������A
            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//����Deep_���𪫥�
            animator.SetBool(parRunAtack, false);
        }
        //½�u��ı
        protected IEnumerator waitRoll()
        {
            print("½�u��ı");
            yield return new WaitForSeconds(wait500);//���ݴX��
            rig2D.velocity = new Vector2(0, 0);
            stateRun = false;//���O�]�B���A
            animator.SetBool(parRun, false);////�����]�B�ʵe
            animator.SetBool(parRoll, false);////����½�u�ʵe
            transform.position = new Vector2(transform.position.x, transform.position.y + 1.2f);//��_�H��½�u�ey��m
            moveShadow = true;//�v�l��_
            canmove = true;//��_�ಾ��
            stateRoll = false;//��½�u
        }
        //�Χޯ�}�ҸI���A��0.5�����I��
        protected IEnumerator WaitCloseGameObject(int index)
        {
            //print("����collider");
            yield return new WaitForSeconds(timeIntervalSkill);//���ݴX��
                                                               //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;//�}�ҤM���I��
                                                               //this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//�����l����

            this.gameObject.GetComponentsInChildren<Transform>(true)[index].gameObject.active = false;//���������٪���

        }

        #region  �Q��
        /// <summary>
        /// �Q���W���A�P�_���^�쥻�a��
        /// </summary>
        protected void InjuriedUp_FallBackInToPlace()
        {
            if (Mathf.Abs(transform.position.y - originalY) < 0.2 && stateInjuriedUp_falling && !stateRoll)//���U�ɪ���m�A�p�󵥩�_���I�� && �b�Ť� && ��½�u�C�����a�ߤޤO�My�t�� && ���ˤW����
            {
                //print("�P�_���^�쥻�a�� if��");
                rig2D.gravityScale = 0;
                rig2D.velocity = new Vector2(0, 0);//�I�켲�ɤ]�|���ϦV�@�ΤO���t�סA�]�������O��y�b���[�t�ץΡA�|�~�򱼸��A�ҥHy�b�t�׭n��0
                transform.position = new Vector2(transform.position.x, originalY);//�����ɰ��׳]�w�����D�e����
                moveShadow = true;//�v�l����
                animator.SetBool(parInjuriedUp, false);//�������ˤW���ʵe
                canmove = true;//�i�H����
                stateInjuried = false;//��������
                stateInjuriedUp = false;//�������ˤW�����A
                stateInjuriedUp_falling = false;

                if (stateDead)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 2.57f);//���`�ʵe���C��m
                }
            }
        }

        #endregion



        #endregion
    }
}