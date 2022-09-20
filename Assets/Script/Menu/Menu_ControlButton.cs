using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//SceneManager.LoadScene(sceneName);
public class Menu_ControlButton : MonoBehaviour
{
   
    /// <summary>
    /// 開始遊戲
    /// </summary>
    /// <param name="sceneName">Game</param>
    public void ButtonStartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// 遊戲教學
    /// </summary>
    /// <param name="sceneName">遊戲教學</param>
    public void ButtonGameTeaching(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonQuitGame()
    {
        //只在發布執行檔 電腦與手機裝置上使用
        Application.Quit();
    }
}
