using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public string gameOverScene = "GameOver";
    public string winScene = "Win";

    public MeshGenerator meshGenerator;

    public TMP_Text maxDefensesText;
    public TMP_Text defensesText;
    public TMP_Text currentMoneyText;
    public TMP_Text currentWaveText;

    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public string[] tutorialMessages;

    public int maxDefenses = 3;
    public int defenses = 3;
    public int money;
    public int currentWave = 0;
    public List<GameObject> spawnedEnemies;
    public List<GameObject> spawnedDefenses;

    public bool win = false;
    public bool lost = false;
    bool started = false;

    public BasePlacable[] defensePlacables;
    public EnemyBase[] spawnableEnemies;
    public BasePlacable tower;
    public GameObject towerObj;
    public TowerParent mainTower;

    public bool towerPlaced = false;

    public WaveBase[] waves;

    public static ResourceManager instance { get; private set; }
    //basic script for keeping track of resources
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        tutorialText.text = tutorialMessages[0];
    }
    public void PlaceDefense(DefenseObject defense)
    {
        money += -defense.cost;
        defenses++;
    }
    // Update is called once per frame
    void Update()
    {
        maxDefensesText.text = maxDefenses.ToString();
        defensesText.text = defenses.ToString();
        currentMoneyText.text = money.ToString();
        if (towerPlaced)
        {
            if(started ==  false)
            {
                if (Input.GetKeyUp(KeyCode.P))
                {
                    started = true;
                    tutorialPanel.gameObject.SetActive(false);
                    StartCoroutine(StartWave());
                }
            }
        }
        if(currentWave >= 1)
        {
            if (spawnedEnemies.Count == 0)
            {
                StartCoroutine(StartWave());
            }
        }
        if(lost)
        {
            SceneManager.LoadScene(gameOverScene);
        }
        if(win)
        {
            SceneManager.LoadScene(winScene);
        }
    }
    public void OnEnemyKilled(int rank)
    {
        money += 10 * rank;
    }
    public BasePlacable[] GetPlacables()
    {
        return defensePlacables;
    }
    public void OnTowerPlaced()
    {
        towerPlaced = true;
        tutorialText.text = tutorialMessages[1];
    }
    public IEnumerator StartWave()
    {
        GameManager.instance.SetWaves(currentWave);
        Debug.Log("starting next wave");
        EnemyBase enemyToSpawn;
        currentWave++;
        currentWaveText.text = "Wave:" + currentWave.ToString();
        for (int i = 0; i < waves[currentWave - 1].Wave.Count; i++)
        {
            enemyToSpawn = waves[currentWave - 1].Wave[i];
            SpawnEnemy(enemyToSpawn);
            yield return new WaitForSeconds(1f);
        }
        if(currentWave >=  waves.Length)
        {
            currentWave = 0;
        }
    }
    public GameObject ClosestEnemy(Vector3 pos)
    {
        GameObject closest = null;
        float shortestDistance;
        if(spawnedEnemies.Count > 0)
        {
            shortestDistance = Vector3.Distance(pos, spawnedEnemies[0].transform.position);
            closest = spawnedEnemies[0];
            //Debug.Log(shortestDistance);
            foreach (GameObject enemy in spawnedEnemies)
            {
                if(Vector3.Distance(enemy.transform.position, pos) <= shortestDistance)
                {
                    closest = enemy;
                }
            }
        }
        return closest;
    }
    public GameObject[] ClosestEnemies(Vector3 pos)
    {
        List<GameObject> closest = new List<GameObject>(10);
        float shortestDistance;
        if (spawnedEnemies.Count > 0)
        {
            shortestDistance = Vector3.Distance(pos, spawnedEnemies[0].transform.position);
            closest.Add(spawnedEnemies[0]);
            Debug.Log(shortestDistance);
            for(int i = 0; i < spawnedEnemies.Count; i++)
            {
                if (Vector3.Distance(spawnedEnemies[i].transform.position, pos) <= shortestDistance)
                {
                    closest.Add(spawnedEnemies[i]);
                }
            }
        }
        return closest.ToArray();
    }
    public GameObject GetClosestDefense(Vector3 pos)
    {
        GameObject closest = null;
        float shortestDistance;
        if (spawnedDefenses.Count > 0)
        {
            shortestDistance = Vector3.Distance(pos, spawnedDefenses[0].transform.position);
            closest = spawnedDefenses[0];
            //Debug.Log(shortestDistance);
            foreach (GameObject defense in spawnedDefenses)
            {
                if (Vector3.Distance(defense.transform.position, pos) <= shortestDistance)
                {
                    closest = defense;
                }
            }
        }
        return closest;
    }
    public void AddMoney(int amount)
    {
        money += amount;
        GameManager.instance.SetMoney(money);
    }
    public void SpawnEnemy(EnemyBase enemyToSpawn, bool randomPath = true, Vector3[] path = null)
    {
        EnemyScript spawnedEnemy;
        GameObject enemyObject;
        enemyObject = Instantiate(enemyToSpawn.model);
        spawnedEnemy = enemyObject.GetComponent<EnemyScript>();
        spawnedEnemy.enemyStats = enemyToSpawn;
        spawnedEnemy.ApplyStats();
        if(randomPath)
        {
            spawnedEnemy.path = meshGenerator.GetRandomPath();
        }
        else
        {
            spawnedEnemy.path = path;
        }
        spawnedEnemy.BeginMoving(spawnedEnemy.currentPathIndex);
        spawnedEnemies.Add(enemyObject);
    }
    public float GetCurrentTowerPercentage()
    {
        if(towerPlaced == false)
        {
            return 1;
        }
        float currentPercentage = 0;
        currentPercentage = mainTower.health / mainTower.maxHealth;
        Debug.Log(currentPercentage);
        return currentPercentage;
    }
}
