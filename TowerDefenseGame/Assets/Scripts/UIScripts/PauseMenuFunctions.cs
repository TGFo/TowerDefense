using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuFunctions : MonoBehaviour
{
    public GameObject pauseMenuObj;
    bool paused;
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
           PauseUnpause(!paused);
        }
    }
    public void RestartLevel(string sceneName)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
    }
    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
    public void PauseUnpause(bool paused)
    {
        if(paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        pauseMenuObj.SetActive(paused);
        this.paused = paused;
    }
}
