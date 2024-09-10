using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int survivedWaves;
    int Money;
    public string CurrentLevel;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetWaves(int waves)
    {
        survivedWaves = waves;
    }
    public int GetWaves()
    {
        return survivedWaves;
    }
    public void SetMoney(int money)
    {
        this.Money = money;
    }
    public int GetMoney()
    {
        return Money;
    }
}
