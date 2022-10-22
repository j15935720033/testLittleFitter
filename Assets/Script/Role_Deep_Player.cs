using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;//�ޥ�AudioMixer�{���w
using System;//���ζü�
using UnityEngine.UI;
namespace chia
{
    public class Role_Deep_Player : Role
    {
        //��velocity�����
        [SerializeField, Header("X�bvectory�����t��")]
        private float xSpeedWalk = 1000;
        [SerializeField, Header("X�bvector�]�B�t��")]
        private float xSpeedRun = 1300;
        [SerializeField, Header("Y�bvectory�����t��")]
        private float ySpeedWalk = 1000;

        //��position�����
        [SerializeField, Header("�����t��"), Tooltip("��position����")]
        protected float pSpeedWalk = 20f;//��position����
        [SerializeField, Header("�]�B�t��"), Tooltip("��position����")]
        protected float pSpeedRun = 40f;//��position�]�B��
        //��addforce����
        [SerializeField, Header("���D�O�q")]
        protected float jumpForce = 400;
        [SerializeField, Header("�������O")]
        protected float coefficientOfDrag = 1f;
        [SerializeField, Header("�a�ߤޤO")]
        protected float gravityScale = 1f;
        
        internal bool clickJump;//�O�_�����D

        protected ObjectPoolBullet_Deep objectPoolBullet;

        
        protected override void Update()
        {
            //print($"xSpeedWalk: {xSpeedWalk}");
            base.Update();
            GetKeyDownTime();
            Walk2();

            //���ʦ�m����
            gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
            
            Attack();
            JumpKey();
            Jump_FallBackInToPlace();

            #region position �M localposeion����
            //������   position ����  localPosition
            //�l����   position:�|�H�����󲾰�   localPosition:�T�w
            //print("tempTrasforms[1].name : " + tempTrasforms[1].name);
            //print("postion: " + tempTrasforms[1].position);
            //print("local: " + tempTrasforms[1].��m �M);
            #endregion

        }
        //�@��T�w50���A���z���ʩ�o��
        protected  void FixedUpdate()
        {
            JumpForce();
        }
        protected override void Awake()
        {
            base.Awake();
            objectPoolBullet = FindObjectOfType<ObjectPoolBullet_Deep>();//���o�l�u�����
            rig2D.drag = coefficientOfDrag;//�������O
        }
        #region �ۭq��k

        /// <summary>
        /// ��Input.GetKey����W�U���k
        /// </summary>
        protected override void Walk2()
        {
            //************************GetKey*************************************************//
            //���k��
            if (Input.GetKey(KeyCode.RightArrow))
            {
                getKey_RightTime = Time.time;
                pressRight = true;
                //�k�]�B
                if (getKey_RightTime - getKeyUp_RightTime <= pressInterval && canmove && !stateJump)//������2�U && �ಾ�� && ���O���D���A
                {
                    print("<Color=yellow>�k�]</Color>");
                    stateRun = true;//�]�B���A
                    animator.SetBool(parRun, true);//�}�Ҷ]�B�ʵe
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x + pSpeedRun*Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(xSpeedRun * Time.deltaTime, rig2D.velocity.y);
                    getKeyUp_RightTime = Time.time;//�]�B���A���n�����s�P��ɶ�
                }
                else if (canmove && !stateJump)//����: �ಾ��&&���O���D���A
                {
                    stateWalk = true;//�������A
                    print("<Color=red>�k��</Color>");
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x + pSpeedWalk * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(xSpeedWalk * Time.deltaTime, rig2D.velocity.y);
                    animator.SetBool(parWalk, true);
                }
               

                //�V�k��V
                gameObject.transform.rotation = new Quaternion(trans.rotation.x, 0, trans.rotation.z, trans.rotation.w);
            }
            //�P�k��
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                pressRight = false;
                getKeyUp_RightTime = Time.time;
                
                    
                if (canmove && !stateJump)//���O���D���A�P�k��:�ಾ�� && ���O���D���A
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(0, rig2D.velocity.y);//������X�t���k�s,Y�b�O����t
                }
                else//���D���A�P�k��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//X�b�O����t,Y�b�O����t
                }
                    stateRun = false;//�����]�B���A
                    stateWalk = false;//�������A
                    animator.SetBool(parWalk, false);//���������ʵe
                    animator.SetBool(parRun, false);//�����]�B�ʵe
            }

            //������
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pressLeft = true;//�O�_�k���U����
                getKey_LeftTime = Time.time;
                //���]�B
                if (getKey_LeftTime - getKeyUp_LeftTime <= pressInterval && canmove && !stateJump)
                {
                    print("<Color=yellow>���]</Color>");
                    stateRun = true;//�]�B���A
                    animator.SetBool(parRun, true);//�}�Ҷ]�B�ʵe
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x - pSpeedRun * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(-xSpeedRun * Time.deltaTime, rig2D.velocity.y);
                    getKeyUp_LeftTime = Time.time;//�����s�P��
                }
                else if (canmove && !stateJump)//����
                {
                    stateWalk = true;//�������A
                    print("<Color=red>����</Color>");
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x - pSpeedWalk * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(-xSpeedWalk * Time.deltaTime, rig2D.velocity.y);
                    animator.SetBool(parWalk, true);
                }
                //�V����V
                gameObject.transform.rotation = new Quaternion(trans.rotation.x, 180, trans.rotation.z, trans.rotation.w);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                pressLeft = false;//�O�_�k���U����
                getKeyUp_LeftTime = Time.time;

                    
                if (canmove && !stateJump)//�ಾ�� && ���O���D���A:���O���D���A�P�k��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(0, rig2D.velocity.y);
                }
                else//���D���A���W��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);
                }
                stateRun = false;//�����]�B���A
                    stateWalk = false;//�������A
                    animator.SetBool(parWalk, false);
                    animator.SetBool(parRun, false);

                getKeyUp_LeftTime = Time.time;
            }

            //�W��
            if (Input.GetKey(KeyCode.UpArrow))
            {
                getKey_UpTime = Time.time;
                pressUp = true;
                if (canmove && !stateJump)//�ಾ�� && ���O���D���A
                {
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + pSpeedWalk * Time.deltaTime, minY, maxY), transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, ySpeedWalk * Time.deltaTime);
                    animator.SetBool(parWalk, true);
                }
                else//���D���A:���W�䤣�n�A�[Y�b�t��
                {
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);
                }
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) )
            {
                pressUp = false;
                getKeyUp_UpTime = Time.time;
                if (canmove && !stateJump)//�ಾ�� && ���O���D���A:���O���D���A���W��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, 0);//Y�b�t���k0
                }
                else//���D���A�P�W��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//�]�����D�ɷ|��y�baddforced�A�����Y�t�׳]��0�A�G�έ�y�b�t��
                }
                animator.SetBool(parWalk, false);

            }
            //�U��
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pressDown = true;
                getKey_DownTime = Time.time;
                //print($"getKey_DownTime: {getKey_DownTime}");
                if (canmove && !stateJump)//�ಾ�� && ���O���D���A
                {
                    #region ��position�����
                    //gameObject.transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y - pSpeedWalk * Time.deltaTime, minY, maxY), transform.position.z);
                    #endregion
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, -ySpeedWalk * Time.deltaTime);
                    animator.SetBool(parWalk, true);
                }  
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                pressDown = false;
                getKeyUp_DownTime = Time.time;
                if (canmove && !stateJump)//�ಾ�� && ���O���D���A:���O���D���A���W��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, 0);//Y�b�t���k0
                }
                else//���D���A���W��
                {
                    //��velocity�����
                    rig2D.velocity = new Vector2(rig2D.velocity.x, rig2D.velocity.y);//�]�����D�ɷ|��y�baddforced�A�M�[�a�ߤޤO�A�G�έ�y�b�t��
                }
                animator.SetBool(parWalk, false);
                    //getKeyUp_DownTime = Time.time;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        protected override void Attack()
        {
            //*********************Enter******************************//
            //�� �� �� ��
            if (Input.GetKeyDown(KeyCode.Return))//���UEnter
            {
                getKeyDown_EnterTime = Time.time;
                //print($"pressEnterTime: {getKeyDown_EnterTime}");

                //ctrl��Enter
                //print($"getKeyDown_RightControlTime:{getKeyDown_RightControlTime}");//�ץk��ɶ�
                //print($"getKeyDown_DownTime:{getKeyDown_DownTime}");//�פU��ɶ�
                //print($"getKeyDown_EnterTime:{getKeyDown_EnterTime}");//��enter��ɶ�

                if (!stateInjuried)//���O���˪��A�C�~�����
                {


                    //Deep_������  ������
                    //Math.Abs(getKeyDown_DownTime - pressRightControlTime) < pressInterval3�C�P�_���䶡��ɶ�
                    //getKeyDown_DownTime - pressRightControlTime>0�C�P�_���ᶶ��(�U��n���)
                    if (Math.Abs(getKeyDown_DownTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_DownTime - getKeyDown_RightControlTime > 0 && getKeyDown_EnterTime - getKeyDown_DownTime > 0)//���U��MCtrl����ɶ�<pressInterval3 && ��Enter�MCtrl�ɶ� <pressInterval5 && �U��n�bCtrl�᭱ && Enter�n�b�U��᭱
                    {
                        //this.gameObject.GetComponent<CircleCollider2D>().enabled=true;//�}�ҤM���I��
                        //this.gameObject.transform.GetChild(0).gameObject.active=true;//�}�Ҥl����
                        //this.gameObject.transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = true;//�}�Ҥl����M���I��
                        int tempIndex = 0;//�s���쪺GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)
                        {
                            if (tempTrasforms[i].name == "Deep_������")
                            {
                                tempTrasforms[i].gameObject.SetActive(true);//�}��Deep_�����٪���
                                tempIndex = i;
                            }
                        }
                        Debug.Log("ctrl��Enter");
                        animator.SetBool(parSkill01,true);
                        aud.PlayOneShot(deep_sf2);//����
                        StartCoroutine(waitSkill01(tempIndex));//��0.5������GameObject
                        
                    }
                    //ctrl��Enter
                    //Deep_�}�ű�
                    //�k��@�w�n�O�̫��getKey_RightTime - getKey_LeftTime >0
                    else if (Math.Abs(getKeyDown_RightTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_RightTime - getKeyDown_RightControlTime > 0 && getKeyDown_EnterTime - getKeyDown_RightTime > 0 && !stateJump)
                    {
                        Debug.Log("ctrl��Enter");
                        animator.SetTrigger("�}�ű�");
                        aud.PlayOneShot(deep_sf1);//����
                        PlayerPrefs.SetInt("Deep_�}�ű٤�V", 1);
                        //Instantiate(�n���ͪ�����,�ͦ���m,����)
                        //Instantiate(prefabBullet, this.gameObject.transform.position, Quaternion.identity);//this.gameObject:���b�b���}���W������

                        //�ϥΪ����
                        GameObject tempobjectPoolBullet = objectPoolBullet.GetPoolObject(WhoAttack.playerAttack);//�Ǥl�u�O�֪�����
                        tempobjectPoolBullet.transform.position = this.gameObject.transform.position;
                        tempobjectPoolBullet.transform.eulerAngles = new Vector3(0, 0, 0);//�ਤ��
                        tempobjectPoolBullet.GetComponent<BulletController>().direction = 1;//���w��������ͦ�����V
                    }
                    //ctrl��Enter
                    //Deep_�}�ű�
                    //����@�w�n�O�̫��getKey_RightTime - getKey_LeftTime < 0
                    else if (Math.Abs(getKeyDown_LeftTime - getKeyDown_RightControlTime) < pressInterval3 && Math.Abs(getKeyDown_EnterTime - getKeyDown_RightControlTime) < pressInterval5 && getKeyDown_LeftTime- getKeyDown_RightControlTime>0 && getKeyDown_EnterTime - getKeyDown_LeftTime > 0 && !stateJump)
                    {
                        Debug.Log("ctrl��Enter");
                        animator.SetTrigger("�}�ű�");
                        aud.PlayOneShot(deep_sf1);//����
                        PlayerPrefs.SetInt("Deep_�}�ű٤�V", -1);
                        //Instantiate(�n���ͪ�����,�ͦ���m,����
                        //Instantiate(prefabBullet, this.gameObject.transform.position, Quaternion.Euler(0, 180, 0));//�ਤ�סA�¦V����

                        //�ϥΪ����
                        GameObject tempobjectPoolBullet = objectPoolBullet.GetPoolObject(WhoAttack.playerAttack);//�l�u�O�֪�����
                        tempobjectPoolBullet.transform.position = this.gameObject.transform.position;//��m
                        tempobjectPoolBullet.transform.eulerAngles = new Vector3(0, 180, 0);//�ਤ��
                        tempobjectPoolBullet.GetComponent<BulletController>().direction = -1;//���w��������ͦ�����V
                    }
                    else if (stateRun && !stateRoll)//�]�B�ɧ���:�]�B���A && ���A½�u���A
                    {
                        animator.SetBool(parRunAtack, true);
                        stateRunAttack = true;
                        int tempIndex = 0;//�s���쪺GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)//��l����
                        {
                            if (tempTrasforms[i].name == "Deep_�]�B�ɧ���")
                            {
                                this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//�}��Deep_�]�B�ɧ���
                                tempIndex = i;
                            }
                        }
                        StartCoroutine(waitRunAttack(tempIndex));//��0.3�������]�B�����ʵe

                    }
                    else //����Enter(����)
                    {
                        //print(random.Next(2));//0�B1
                        //print(random.Next(1,3));//1�B2
                        i01 = random.Next(1, 3);//�H�������ʧ@
                        int tempIndex = 0;//�s���쪺GameObject
                        for (int i = 0; i < tempTrasforms.Length; i++)//��l����
                        {
                            if (tempTrasforms[i].name == "Deep_����")
                            {
                                this.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject.SetActive(true);//�}��Deep_���𪫥�
                                tempIndex = i;
                            }
                        }
                        if (!stateAttack)//���O���q�������A(���M�@�����|�@�������A�����ʵe�٨S�����N�S����)
                        {
                            stateAttack = true;//���q�������A
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
                            aud.PlayOneShot(deep_sf0);//����
                            StartCoroutine(waitAttack(tempIndex));//��0.3�����������ʵe
                        }

                    }
                }
            }
            //�PEnter
            if (Input.GetKeyUp(KeyCode.Return))
            {
                //print(random.Next(2));//0�B1
                //print(random.Next(1,3));//1�B2
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
                //�k½�u
                if (stateRun && pressRight && !stateJump)//�]�B��&&�V�k&&���O���D���A
                {
                    //print("�k½�u");
                    stateRoll = true;//½�u��
                    moveShadow = false;//�v�ly�b���n��
                    originalY = transform.position.y;//�����H����m�A�]��½�u�|�ܧC
                    animator.SetBool(parRoll, true);
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                    canmove = false;//���ಾ��
                    rig2D.velocity = new Vector2(speedRoll * Time.deltaTime, rig2D.velocity.y);
                    StartCoroutine(waitRoll());//���ݴX��

                }
                //��½�u
                else if (stateRun && pressLeft && !stateJump)//�]�B��&&�V��&&���O���D���A
                {
                    //print("��½�u");
                    stateRoll = true;//½�u��
                    moveShadow = false;//�v�ly�b���n��
                    originalY = transform.position.y;//�����H����m�A�]��½�u�|�ܧC
                    animator.SetBool(parRoll, true);
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);

                    canmove = false;//���ಾ��
                    rig2D.velocity = new Vector2(-speedRoll * Time.deltaTime, rig2D.velocity.y);
                    StartCoroutine(waitRoll());//���ݴX��
                }
                else if(!stateRun && !stateRoll && !stateJump)//�����M��a�����m
                {

                        canmove = false;//���ಾ��
                        stateDefense = true;
                        //��velocity�����
                        rig2D.velocity = new Vector2(0, rig2D.velocity.y);
                        animator.SetBool(parDefense, true);//�}�Ҩ��m�ʵe
                        StartCoroutine(waitDefense());//���ݴX��
                }
            }
            if (Input.GetKeyUp(KeyCode.RightControl))
            {
                getKeyUp_RightControlTime = Time.time;
                animator.SetBool(parDefense, false);//�������m�ʵe
            }
            //*********************RightShiftTime******************************//
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                getKeyDown_RightShifTime = Time.time;
            }
        }
        /// <summary>
        /// GetKeyDown����ɶ�
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
        ///�������a�O�_��RightShift(���D)�A�åB���U�ɧ�a�ߤޤO�By�t�׳��k0
        /// </summary>
        protected void JumpKey()
        {

            if (Input.GetKeyDown(KeyCode.RightShift) && !stateJump)
            {
                print("���D");
                clickJump = true;
            }
        }
        /// <summary>
        /// �����URightShift(���D��)�A���V�W���O�A�ç�a�ߤޤO�]��1
        /// </summary>
        protected void JumpForce()//�׸��D&&�b�a�O�ɵ��V�W���O�q
        {
            if (clickJump && !stateJump && !stateRoll)//����rightShift && ���O���D���A && ��½�u   
            {

                moveShadow = false;//�v�l���n�첾
                canmove = false;
                originalY = transform.position.y;//�O�����_����m
                stateJump = true;
                animator.SetBool(parJump, stateJump);

                //���D�ɧ�Y�b�t�׳]��0�A�]�� if(Input.GetKey(KeyCode.UpArrow))���W�䨺��A�t�צV�W�k�[���O�|���ܰ�
                rig2D.velocity = new Vector2(rig2D.velocity.x,0);
                rig2D.AddForce(new Vector2(0, jumpForce));//�����D�O�q
                rig2D.gravityScale = gravityScale;//���_��a�ߤޤO�]1
                clickJump = false;
            }
        }
        #endregion
    }
}