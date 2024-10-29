using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnemyTowerEnums;

public class EnemyScript : MonoBehaviour
{

    public EnemyBase enemyStats;
    public HealthBar healthBar;
    public EnemyType enemyType;

    public float health;
    public float maxHealth;
    float damage;
    float speedMult;
    public Vector3[] path;
    public float speed = 2f;
    public float attackSpeed = 2f;
    public float attackRange = 10f;
    public int preferredDistance = 1;
    public float stopDuration = 0.1f;
    public SphereCollider AttackCollider;
    public int currentPathIndex = 0;
    private bool isMoving = true;
    public GameObject target;
    public bool isAttacking = false;
    private Coroutine attackCoroutine;
    public TowerParent currentTarget;
    public float targetCheckTime = 100;
    public float targetCheckTimer = 0;
    public int killReward = 5;
    public bool slowed = false;
    private void Start()
    {
        ApplyStats();
    }
    //same attack and target check logic as towers
    private void Update()
    {
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckTime)
        {
            //Debug.Log(target);
            if (target == null && currentPathIndex <= path.Length - 1)
            {
                if(ResourceManager.instance.GetClosestDefense(transform.position) == null)
                {
                    return;
                }
                SetTarget(ResourceManager.instance.GetClosestDefense(transform.position));
            }
            if(currentPathIndex >= path.Length - 1)
            {
                SetTarget(ResourceManager.instance.towerObj);
            }
            targetCheckTimer = 0;
        }
    }
    public void ApplyStats()
    {
        maxHealth = enemyStats.maxHealth;
        health = enemyStats.maxHealth;
        damage = enemyStats.damage;
        speedMult = enemyStats.speedMultiplier;
        attackSpeed = enemyStats.attackSpeed;
        killReward = enemyStats.killReward;
        attackRange = enemyStats.range;
        enemyType = enemyStats.enemyType;
        preferredDistance = enemyStats.preferredDistance;
        healthBar.SetHealth(health, maxHealth);
    }
    public void BeginMoving(int pathPos = 0)
    {
        if (path.Length > 0)
        {
            transform.position = path[pathPos];
            currentPathIndex = pathPos;
            StartCoroutine(MoveAlongPath());
        }
    }
    private IEnumerator MoveAlongPath()
    {
        //moves along given path, stopping when point is reached for set time
        while (currentPathIndex < path.Length - preferredDistance)
        {
            if (isMoving)
            {
                Vector3 targetPosition = path[currentPathIndex];
                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed * speedMult) * Time.deltaTime);
                    yield return null;
                }
                yield return new WaitForSeconds(stopDuration);
                currentPathIndex++;
            }
        }
        if(currentPathIndex == path.Length)
        {
            if(enemyType == EnemyType.Suicider)
            {
                Attack();
                OnKilled();
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        //Debug.Log("attacks: " + target);
        while (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                Attack();
            }
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    private void Attack()
    {
        //Debug.Log("Enemy attack" + currentTarget);
        if(currentTarget == null) 
        {
            return;
        }
        currentTarget.OnDamageTaken(damage);
        //Debug.Log("Enemy attack");
    }
    private void SetTarget(GameObject targetToAttack)
    {
        if(enemyType == EnemyType.Suicider)
        {
            target = ResourceManager.instance.towerObj;
            currentTarget = target.GetComponent<TowerParent>();
            return;
        }
        if (targetToAttack == null)
        {
            return;
        }
        target = targetToAttack;
        currentTarget = target.GetComponent<TowerParent>();
        if (!isAttacking)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
            isAttacking = true;
        }
    }

    private void StopAttacking()
    {
        if (isAttacking)
        {
            StopCoroutine(attackCoroutine);
            isAttacking = false;
            target = null;
            currentTarget = null;
        }
    }
    public void OnDamageTaken(float takenDamage)
    {
        health -= takenDamage;
        healthBar.SetHealth(health, maxHealth);
        if(health <= 0 )
        {
            OnKilled();
        }
    }
    public void OnKilled()
    {
        EnemySpawner spawner;
        if(enemyType == EnemyType.Spawner)
        {
            spawner = (EnemySpawner)enemyStats;
            foreach(EnemyBase enemy in spawner .spawnableEnemies)
            {
                ResourceManager.instance.SpawnEnemy(enemy, false, path);
            }
        }
        //adds money for purchasing defenses on death
        ResourceManager.instance.AddMoney(killReward);
        ResourceManager.instance.spawnedEnemies.Remove(gameObject);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        StopAttacking();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enemyType != EnemyType.Suicider)
        {
            return;
        }
        if (CompareTag("Defense"))
        {
            Debug.Log("Collided");
            Attack();
            OnKilled();
        }
    }
    public void OnSlow(float slowValue, float slowTime)
    {
        if(slowed == true)
        {
            return;
        }
        Debug.Log("slowed");
        slowed = true;
        StartCoroutine(SlowEnemy(slowValue, slowTime));
    }
    public IEnumerator SlowEnemy(float slowValue, float slowTime)
    {
        float currentSpeed = speedMult;
        speedMult = speedMult - slowValue;
        yield return new WaitForSeconds(slowTime);
        speedMult = currentSpeed;
        slowed = false;
    }
}