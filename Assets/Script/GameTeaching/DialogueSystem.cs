using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;//引用系統集合，資料結構與偕同程序
namespace chia
{


    /// <summary>
    /// 1.對話系統、2.淡入對話框、3.更新NPC資料名稱、內容、音效、淡出
    /// </summary>
    //RequireComponent:加添加腳本時(加入此Scipt時)，自動加入元件
    [RequireComponent(typeof(AudioSource))]
    public class DialogueSystem : MonoBehaviour
    {
        #region 資料

        //委派簽名，無傳回與無參數
        public delegate void DelegateFinishDialogue();

        [SerializeField, Header("畫布對話系統")]
        private CanvasGroup canvasGroupDialogueSystem;

        [SerializeField, Header("說話者名稱")]
        private Text textName;

        [SerializeField, Header("對話內容")]
        private Text textContent;

        [SerializeField, Header("三角形")]
        private GameObject goTriangle;

        private AudioSource aud;//音效
        private DataNPC dataNPC;

        [SerializeField, Header("淡入時間")]
        private float intervalFadIn = 0.1f;
        [SerializeField, Header("打字間隔")]
        private float intervalType = 0.05f;
        [SerializeField, Header("下一頁音效")]
        public AudioClip nextPageSound;

        #endregion
        #region unity方法
        private void Awake()
        {
            aud = GetComponent<AudioSource>();
            //StartCoroutine(CanvasFade());//啟動偕同程序
            //StartCoroutine(StartDialogue());//啟動偕同程序_對話欄測試
        }
        private void Update()
        {
           
        }
        #endregion

        #region 自訂方法

        #region 公開資料與方法
        public bool stateDialogue;//是否在對話中

        public IEnumerator StartDialogue(DataNPC dataNPC,DelegateFinishDialogue callback)
        {
            stateDialogue = true;//對話狀態開啟

            this.dataNPC = dataNPC;
            textContent.text = "";//清空對話欄

            yield return StartCoroutine(CanvasFade());//CanvasFade淡入效果 yield return跑完這行才會跑下一行

            for (int i=0;i<dataNPC.dataDialoge.Length;i++)
            {
                aud.PlayOneShot(nextPageSound);
                yield return StartCoroutine(TypeEffect(i));//淡入文字

                //不是按下空白鑑或是滑鼠左鍵，就一直迴圈
                while (!(Input.GetKeyDown(KeyCode.Space)|| Input.GetMouseButtonDown(0)) ){
                    yield return null;//傳回null，是等待1個影格時間(1/60)秒
                }
            }
            StartCoroutine(CanvasFade(false));//畫布淡出
            stateDialogue = false;//對話狀態關閉
            callback();//執行回呼涵式
        }
        #endregion

        /// <summary>
        /// canvas淡入效果
        /// </summary>
        /// <param name="fadeIn"></param>
        /// <returns></returns>
        private IEnumerator CanvasFade(bool fadeIn = true)//給預設值
        {
            //三元運算子
            //布林直?布林值為 true:布林值為 false
            float increase = fadeIn ? 0.1f : -0.1f;
            
            for (int i = 0; i < 10; i++)
            {
                canvasGroupDialogueSystem.alpha += increase;
                //print(i);
                yield return new WaitForSeconds(intervalFadIn);
                print("fadefinish");
            }
        }
        /// <summary>
        /// 打字效果淡入、撥放對話音效、顯示三角形
        /// </summary>
        /// <param name="indexDialogue"></param>
        /// <returns></returns>
        private IEnumerator TypeEffect(int indexDialogue)
        {
            textContent.text = "";//清空對話內容
            aud.PlayOneShot(dataNPC.dataDialoge[indexDialogue].sound);//播放對話音效
            string content = dataNPC.dataDialoge[indexDialogue].content;//對話內容
            for (int i=0;i<content.Length;i++)
            {
                textContent.text += content[i];//content[i]會變字元
                yield return new WaitForSeconds(intervalType);
            }
            goTriangle.SetActive(true);
        }

        #endregion
    }
}