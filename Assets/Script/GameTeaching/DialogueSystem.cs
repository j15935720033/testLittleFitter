using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;//�ޥΨt�ζ��X�A��Ƶ��c�P���P�{��
namespace chia
{


    /// <summary>
    /// 1.��ܨt�ΡB2.�H�J��ܮءB3.��sNPC��ƦW�١B���e�B���ġB�H�X
    /// </summary>
    //RequireComponent:�[�K�[�}����(�[�J��Scipt��)�A�۰ʥ[�J����
    [RequireComponent(typeof(AudioSource))]
    public class DialogueSystem : MonoBehaviour
    {
        #region ���

        //�e��ñ�W�A�L�Ǧ^�P�L�Ѽ�
        public delegate void DelegateFinishDialogue();

        [SerializeField, Header("�e����ܨt��")]
        private CanvasGroup canvasGroupDialogueSystem;

        [SerializeField, Header("���ܪ̦W��")]
        private Text textName;

        [SerializeField, Header("��ܤ��e")]
        private Text textContent;

        [SerializeField, Header("�T����")]
        private GameObject goTriangle;

        private AudioSource aud;//����
        private DataNPC dataNPC;

        [SerializeField, Header("�H�J�ɶ�")]
        private float intervalFadIn = 0.1f;
        [SerializeField, Header("���r���j")]
        private float intervalType = 0.05f;
        [SerializeField, Header("�U�@������")]
        public AudioClip nextPageSound;

        #endregion
        #region unity��k
        private void Awake()
        {
            aud = GetComponent<AudioSource>();
            //StartCoroutine(CanvasFade());//�Ұʰ��P�{��
            //StartCoroutine(StartDialogue());//�Ұʰ��P�{��_��������
        }
        private void Update()
        {
           
        }
        #endregion

        #region �ۭq��k

        #region ���}��ƻP��k
        public bool stateDialogue;//�O�_�b��ܤ�

        public IEnumerator StartDialogue(DataNPC dataNPC,DelegateFinishDialogue callback)
        {
            stateDialogue = true;//��ܪ��A�}��

            this.dataNPC = dataNPC;
            textContent.text = "";//�M�Ź����

            yield return StartCoroutine(CanvasFade());//CanvasFade�H�J�ĪG yield return�]���o��~�|�]�U�@��

            for (int i=0;i<dataNPC.dataDialoge.Length;i++)
            {
                aud.PlayOneShot(nextPageSound);
                yield return StartCoroutine(TypeEffect(i));//�H�J��r

                //���O���U�ť�Ų�άO�ƹ�����A�N�@���j��
                while (!(Input.GetKeyDown(KeyCode.Space)|| Input.GetMouseButtonDown(0)) ){
                    yield return null;//�Ǧ^null�A�O����1�Ӽv��ɶ�(1/60)��
                }
            }
            StartCoroutine(CanvasFade(false));//�e���H�X
            stateDialogue = false;//��ܪ��A����
            callback();//����^�I�[��
        }
        #endregion

        /// <summary>
        /// canvas�H�J�ĪG
        /// </summary>
        /// <param name="fadeIn"></param>
        /// <returns></returns>
        private IEnumerator CanvasFade(bool fadeIn = true)//���w�]��
        {
            //�T���B��l
            //���L��?���L�Ȭ� true:���L�Ȭ� false
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
        /// ���r�ĪG�H�J�B�����ܭ��ġB��ܤT����
        /// </summary>
        /// <param name="indexDialogue"></param>
        /// <returns></returns>
        private IEnumerator TypeEffect(int indexDialogue)
        {
            textContent.text = "";//�M�Ź�ܤ��e
            aud.PlayOneShot(dataNPC.dataDialoge[indexDialogue].sound);//�����ܭ���
            string content = dataNPC.dataDialoge[indexDialogue].content;//��ܤ��e
            for (int i=0;i<content.Length;i++)
            {
                textContent.text += content[i];//content[i]�|�ܦr��
                yield return new WaitForSeconds(intervalType);
            }
            goTriangle.SetActive(true);
        }

        #endregion
    }
}