using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//SceneManager.LoadScene(sceneName);
public class Menu_ControlButton : MonoBehaviour
{
   
    /// <summary>
    /// �}�l�C��
    /// </summary>
    /// <param name="sceneName">Game</param>
    public void ButtonStartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// �C���о�
    /// </summary>
    /// <param name="sceneName">�C���о�</param>
    public void ButtonGameTeaching(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonQuitGame()
    {
        //�u�b�o�������� �q���P����˸m�W�ϥ�
        Application.Quit();
    }
}
