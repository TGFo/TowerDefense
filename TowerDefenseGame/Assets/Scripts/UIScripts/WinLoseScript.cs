using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScript : MonoBehaviour
{
    public string lastLevel;
    public TMP_Text MoneyText;
    public TMP_Text WavesSurvivedText;
    public string moneyString = "Money earned: ";
    public string wavesString = "Waves survived: ";
    private void Start()
    {
        if(GameManager.instance == null) return;
        MoneyText.text = moneyString + GameManager.instance.GetMoney();
        WavesSurvivedText.text = wavesString + GameManager.instance.GetWaves();
        lastLevel = GameManager.instance.CurrentLevel;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(lastLevel);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
}
