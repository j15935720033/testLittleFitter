using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Game_ControlButton : MonoBehaviour
{

    /// <summary>
    /// ���s�C�����s
    /// </summary>
    /// <param name="sceneName">Game</param>
    public void ButtonReplayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// �^Menu
    /// </summary>
    public void ButtonGoMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonQuitGame()
    {
        //�u�b�o�������� �q���P����˸m�W�ϥ�
        Application.Quit();
    }
  
}
