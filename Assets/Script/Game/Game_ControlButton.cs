using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Game_ControlButton : MonoBehaviour
{

    /// <summary>
    /// 重新遊戲按鈕
    /// </summary>
    /// <param name="sceneName">Game</param>
    public void ButtonReplayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// 回Menu
    /// </summary>
    public void ButtonGoMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonQuitGame()
    {
        //只在發布執行檔 電腦與手機裝置上使用
        Application.Quit();
    }
  
}
