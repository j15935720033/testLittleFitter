using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chia
{

    /// <summary>
    /// 偵測玩家是否走進區域內,顯示提示畫面,按鍵偵測並啟動對話系統
    /// </summary>
    public class NPCSystem : MonoBehaviour
    {
        [SerializeField, Header("NPC 對話系統")]
        private DataNPC dataNPC;
        private Animator ani;
        private string parTipFad = "Trigger_提示淡出淡入";
        private string parHitName = "Deep_Player";//NPC碰到誰要觸發
        private bool isInTrigger;//玩家是否走進對話碰撞區域內
        [SerializeField, Header("NPC照相機")]
        GameObject npcCamera;
        DialogueSystem dialogueSystem;
        private string parDialogue = "isLookUp";
        private Role_Deep_Player scriptDeep;

        #region unity方法
        private void Awake()
        {
            ani = GameObject.Find("Image_對話提示").GetComponent<Animator>();
            dialogueSystem = FindObjectOfType<DialogueSystem>();//找Script，等於要找有初始化的DialogueSystem
            scriptDeep = FindObjectOfType<Role_Deep_Player>();
        }
        private void Update()
        {
            InputKeyAndStarDialogue();
        }

        //碰撞事件
        //1.此腳本所在的GameObject要有rigidbody2D
        //2.有勾選 Trigger 使用OnTrigger 事件:[Enter、Exit、stay]
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //print("進入偵測區域" + collision.name);
            CheckPlayerAndAnimation(collision.name, true);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            //print("離開偵測區域" + collision.name);
            CheckPlayerAndAnimation(collision.name, false);
        }
        #endregion


        /// <summary>
        /// 檢查玩家是否進入或離開並更新動畫
        /// </summary>
        /// <param name="nameHit">碰撞物件名稱</param>
        private void CheckPlayerAndAnimation(string nameHit, bool isInTrigger)
        {
            if (nameHit == parHitName)
            {
                this.isInTrigger = isInTrigger;
                ani.SetTrigger(parTipFad);//開啟提示對話
            }
        }
        /// <summary>
        /// 對話中按空白鍵或是滑鼠左鍵開啟對話
        /// </summary>
        private void InputKeyAndStarDialogue()
        { 
            if (this.isInTrigger && (Input.GetKeyDown(KeyCode.Space)||Input.GetMouseButtonDown(0)))
            {
                //print("按下 空白間或是滑鼠左鍵 開始對話");
                if (dialogueSystem.stateDialogue)//對話中，就return 
                {
                    return;
                }

                npcCamera.SetActive(true);//開啟照相機
                ani.SetTrigger(parTipFad);//要關閉對話題示
                scriptDeep.enabled = false;//停用腳本
                try
                {
                    ani.SetBool(parDialogue, true);//開啟對話動畫
                }
                catch (System.Exception)
                {
                    print("<Color=#993311>缺少元件錯誤,NPC沒有Animation</color>");
                }
                StartCoroutine(dialogueSystem.StartDialogue(dataNPC,ResetControllerAndCloseCamera));
            }
        }

        /// <summary>
        /// 重新設定控制器與關閉攝影機
        /// </summary>
        private void ResetControllerAndCloseCamera()
        {
            npcCamera.SetActive(false);//關閉攝影機GameObject
            scriptDeep.enabled = true;//啟用腳本
            ani.SetTrigger(parTipFad);//淡出提示動畫

            try
            {
                ani.SetBool(parDialogue, false);//關閉對話動畫
            }
            catch (System.Exception)
            {
                print("<Color=#993311>缺少元件錯誤，NPC沒有 Animation</color>");
                //throw
            }
        }
    }
}