using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{

    public EnemyBase enemyStats;
    public HealthBar healthBar;

    public float health;
    public float maxHealth;
    float damage;
    float speedMult;
    public Vector3[] path;
    public float speed = 2f;
    public float attackSpeed = 2f;
    public float attackRange = 2f;
    public float stopDuration = 0.1f;
    public SphereCollider AttackCollider;
    private int currentPathIndex = 0;
    private bool isMoving = true;
    public GameObject target;
    public bool isAttacking = false;
    private Coroutine attackCoroutine;
    TowerScript currentTarget;
    public float targetCheckTime = 2;
    public float targetCheckTimer = 0;
    public int killReward = 5;
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
            Debug.Log(target);
            if (target == null && currentPathIndex <= path.Length - 1)
            {
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
        healthBar.SetHealth(health, maxHealth);
    }
    public void BeginMoving()
    {
        if (path.Length > 0)
        {
            transform.position = path[0];
            StartCoroutine(MoveAlongPath());
        }
    }
    private IEnumerator MoveAlongPath()
    {
        //moves along given path, stopping when point is reached for set time
        while (currentPathIndex < path.Length - 1)
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
    }

    private IEnumerator AttackCoroutine()
    {
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
        if(currentTarget == null) 
        {
            return;
        }
        currentTarget.OnDamageTaken(damage);
        Debug.Log("Enemy attack");
    }
    private void SetTarget(GameObject targetToAttack)
    {
        target = targetToAttack;
        currentTarget = target.GetComponent<TowerScript>();
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
        //adds money for purchasing defenses on death
        ResourceManager.instance.AddMoney(killReward);
        ResourceManager.instance.spawnedEnemies.Remove(gameObject);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        StopAttacking();
    }
}