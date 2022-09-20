using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;//引用AudioMixer程式庫

public class Game_Setting : MonoBehaviour
{
    [Header("prefabe_BGM")]
    public GameObject BGM;
    //控制整體聲音開關
    bool controlAudio;
    [Header("聲音開圖片")]
    public Sprite OpenSound;
    [Header("聲音關圖片")]
    public Sprite CloseSound;
    [Header("聲音按鈕")]
    public Image ButtonSound;
    [SerializeField,Header("聲音拉條")]
    private Slider changeAudioSlider;
    //[Header("AudioMixer")]
    public AudioMixer audioMixerObj;
    [SerializeField, Header("暫停底圖")]
    private GameObject pauseUI;
    private bool statePause;//遊戲暫停
    private void Start()
    {
        //檢查場景上的BGM數量是否<=0
        if (GameObject.FindGameObjectsWithTag("BGM").Length<=0)
        {
            //動態生成一個背景音樂物件
            Instantiate(BGM);
            AudioListener.pause = controlAudio;
        }
    }

    /// <summary>
    /// 使用Slider控制音樂
    /// //法1.Camera的AudioListener 控制Slider。
    /// //法2.用AudioMixer 控制Slider。
    /// </summary>
    public void ChangAudioSlider()
    {
        //法1.Camera的AudioListener 控制Slider。
        AudioListener.volume = changeAudioSlider.value;

        //法2.用AudioMixer 控制Slider。
        //audioMixerObj.SetFloat("BGM", changeAudioSlider.value);
    }

    /// <summary>
    /// 控制main Camera的聲音
    /// </summary>
    public void ControlAudio()
    {
        
        controlAudio = !controlAudio;
        //控制聲音圖片
        if (controlAudio)//true:靜音
        {
            //讀取圖片法1.
            ButtonSound.sprite = CloseSound;
            //讀取圖片法2.
            //ButtonSound.sprite = Resources.Load<Sprite>("Sprite/VoiceOpen");
            //讀取圖片法3.
            //StreamingAssetsLoadTexture(0);
        }
        else
        {
            //讀取圖片法1:
            ButtonSound.sprite = OpenSound;
            //讀取圖片法2.Resources路徑中是放Sprite檔
            //ButtonSound.sprite = Resources.Load<Sprite>("Sprite/VoiceClose");
            //讀取圖片法3.StreamingAssets路徑中是放Texture2D圖片檔
            //StreamingAssetsLoadTexture(1);
        }

        //AudioListener.pause = true;//整體還經聲音靜音
        //AudioListener.pause = false;//整體還經聲音開啟
        AudioListener.pause = controlAudio;//AudioListener:控制main Camera的聲音
    }
    /// <summary>
    /// 時間暫停
    /// </summary>
    /// <param name="statePause"></param>
    public void GamePause()
    {
        statePause = true;
        //Time.timeScale = 0;//整體時間暫停，滑鼠點擊Input暫停不了
        //Time.timeScale = 1;//整體時間恢復
        //法1.
            Time.timeScale = 0;
            pauseUI.SetActive(statePause);//開啟或關閉暫停底圖物件
            FindObjectOfType<Role_deep>().enabled = !statePause;//關閉Role_deep.CS
            AudioListener.pause = statePause;//關閉音樂  AudioListener:控制main Camera的聲音

    }
    public void GameResume()
    {
        statePause = false;
        //Time.timeScale = 0;//整體時間暫停，滑鼠點擊Input暫停不了
        //Time.timeScale = 1;//整體時間恢復
            Time.timeScale = 1;
            pauseUI.SetActive(statePause);//開啟或關閉暫停底圖物件
            FindObjectOfType<Role_deep>().enabled = !statePause;//關閉Role_deep.CS
            AudioListener.pause = statePause;//關閉音樂  AudioListener:控制main Camera的聲音
    }

}
