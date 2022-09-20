using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;//�ޥ�AudioMixer�{���w

public class Game_Setting : MonoBehaviour
{
    [Header("prefabe_BGM")]
    public GameObject BGM;
    //��������n���}��
    bool controlAudio;
    [Header("�n���}�Ϥ�")]
    public Sprite OpenSound;
    [Header("�n�����Ϥ�")]
    public Sprite CloseSound;
    [Header("�n�����s")]
    public Image ButtonSound;
    [SerializeField,Header("�n���Ա�")]
    private Slider changeAudioSlider;
    //[Header("AudioMixer")]
    public AudioMixer audioMixerObj;
    [SerializeField, Header("�Ȱ�����")]
    private GameObject pauseUI;
    private bool statePause;//�C���Ȱ�
    private void Start()
    {
        //�ˬd�����W��BGM�ƶq�O�_<=0
        if (GameObject.FindGameObjectsWithTag("BGM").Length<=0)
        {
            //�ʺA�ͦ��@�ӭI�����֪���
            Instantiate(BGM);
            AudioListener.pause = controlAudio;
        }
    }

    /// <summary>
    /// �ϥ�Slider�����
    /// //�k1.Camera��AudioListener ����Slider�C
    /// //�k2.��AudioMixer ����Slider�C
    /// </summary>
    public void ChangAudioSlider()
    {
        //�k1.Camera��AudioListener ����Slider�C
        AudioListener.volume = changeAudioSlider.value;

        //�k2.��AudioMixer ����Slider�C
        //audioMixerObj.SetFloat("BGM", changeAudioSlider.value);
    }

    /// <summary>
    /// ����main Camera���n��
    /// </summary>
    public void ControlAudio()
    {
        
        controlAudio = !controlAudio;
        //�����n���Ϥ�
        if (controlAudio)//true:�R��
        {
            //Ū���Ϥ��k1.
            ButtonSound.sprite = CloseSound;
            //Ū���Ϥ��k2.
            //ButtonSound.sprite = Resources.Load<Sprite>("Sprite/VoiceOpen");
            //Ū���Ϥ��k3.
            //StreamingAssetsLoadTexture(0);
        }
        else
        {
            //Ū���Ϥ��k1:
            ButtonSound.sprite = OpenSound;
            //Ū���Ϥ��k2.Resources���|���O��Sprite��
            //ButtonSound.sprite = Resources.Load<Sprite>("Sprite/VoiceClose");
            //Ū���Ϥ��k3.StreamingAssets���|���O��Texture2D�Ϥ���
            //StreamingAssetsLoadTexture(1);
        }

        //AudioListener.pause = true;//�����ٸg�n���R��
        //AudioListener.pause = false;//�����ٸg�n���}��
        AudioListener.pause = controlAudio;//AudioListener:����main Camera���n��
    }
    /// <summary>
    /// �ɶ��Ȱ�
    /// </summary>
    /// <param name="statePause"></param>
    public void GamePause()
    {
        statePause = true;
        //Time.timeScale = 0;//����ɶ��Ȱ��A�ƹ��I��Input�Ȱ����F
        //Time.timeScale = 1;//����ɶ���_
        //�k1.
            Time.timeScale = 0;
            pauseUI.SetActive(statePause);//�}�ҩ������Ȱ����Ϫ���
            FindObjectOfType<Role_deep>().enabled = !statePause;//����Role_deep.CS
            AudioListener.pause = statePause;//��������  AudioListener:����main Camera���n��

    }
    public void GameResume()
    {
        statePause = false;
        //Time.timeScale = 0;//����ɶ��Ȱ��A�ƹ��I��Input�Ȱ����F
        //Time.timeScale = 1;//����ɶ���_
            Time.timeScale = 1;
            pauseUI.SetActive(statePause);//�}�ҩ������Ȱ����Ϫ���
            FindObjectOfType<Role_deep>().enabled = !statePause;//����Role_deep.CS
            AudioListener.pause = statePause;//��������  AudioListener:����main Camera���n��
    }

}
