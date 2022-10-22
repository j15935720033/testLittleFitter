using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chia
{

    /// <summary>
    /// �������a�O�_���i�ϰ줺,��ܴ��ܵe��,���䰻���ñҰʹ�ܨt��
    /// </summary>
    public class NPCSystem : MonoBehaviour
    {
        [SerializeField, Header("NPC ��ܨt��")]
        private DataNPC dataNPC;
        private Animator ani;
        private string parTipFad = "Trigger_���ܲH�X�H�J";
        private string parHitName = "Deep_Player";//NPC�I��֭nĲ�o
        private bool isInTrigger;//���a�O�_���i��ܸI���ϰ줺
        [SerializeField, Header("NPC�Ӭ۾�")]
        GameObject npcCamera;
        DialogueSystem dialogueSystem;
        private string parDialogue = "isLookUp";
        private Role_Deep_Player scriptDeep;

        #region unity��k
        private void Awake()
        {
            ani = GameObject.Find("Image_��ܴ���").GetComponent<Animator>();
            dialogueSystem = FindObjectOfType<DialogueSystem>();//��Script�A����n�䦳��l�ƪ�DialogueSystem
            scriptDeep = FindObjectOfType<Role_Deep_Player>();
        }
        private void Update()
        {
            InputKeyAndStarDialogue();
        }

        //�I���ƥ�
        //1.���}���Ҧb��GameObject�n��rigidbody2D
        //2.���Ŀ� Trigger �ϥ�OnTrigger �ƥ�:[Enter�BExit�Bstay]
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //print("�i�J�����ϰ�" + collision.name);
            CheckPlayerAndAnimation(collision.name, true);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            //print("���}�����ϰ�" + collision.name);
            CheckPlayerAndAnimation(collision.name, false);
        }
        #endregion


        /// <summary>
        /// �ˬd���a�O�_�i�J�����}�ç�s�ʵe
        /// </summary>
        /// <param name="nameHit">�I������W��</param>
        private void CheckPlayerAndAnimation(string nameHit, bool isInTrigger)
        {
            if (nameHit == parHitName)
            {
                this.isInTrigger = isInTrigger;
                ani.SetTrigger(parTipFad);//�}�Ҵ��ܹ��
            }
        }
        /// <summary>
        /// ��ܤ����ť���άO�ƹ�����}�ҹ��
        /// </summary>
        private void InputKeyAndStarDialogue()
        { 
            if (this.isInTrigger && (Input.GetKeyDown(KeyCode.Space)||Input.GetMouseButtonDown(0)))
            {
                //print("���U �ťն��άO�ƹ����� �}�l���");
                if (dialogueSystem.stateDialogue)//��ܤ��A�Nreturn 
                {
                    return;
                }

                npcCamera.SetActive(true);//�}�ҷӬ۾�
                ani.SetTrigger(parTipFad);//�n��������D��
                scriptDeep.enabled = false;//���θ}��
                try
                {
                    ani.SetBool(parDialogue, true);//�}�ҹ�ܰʵe
                }
                catch (System.Exception)
                {
                    print("<Color=#993311>�ʤ֤�����~,NPC�S��Animation</color>");
                }
                StartCoroutine(dialogueSystem.StartDialogue(dataNPC,ResetControllerAndCloseCamera));
            }
        }

        /// <summary>
        /// ���s�]�w����P������v��
        /// </summary>
        private void ResetControllerAndCloseCamera()
        {
            npcCamera.SetActive(false);//������v��GameObject
            scriptDeep.enabled = true;//�ҥθ}��
            ani.SetTrigger(parTipFad);//�H�X���ܰʵe

            try
            {
                ani.SetBool(parDialogue, false);//������ܰʵe
            }
            catch (System.Exception)
            {
                print("<Color=#993311>�ʤ֤�����~�ANPC�S�� Animation</color>");
                //throw
            }
        }
    }
}