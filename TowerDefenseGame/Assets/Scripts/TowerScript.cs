using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public HealthBar healthBar;
    public DefenseObject placable;
    public float maxHealth;
    public float health;
    public float fireRate;
    float damage;
    public float range;
    public SphereCollider sphereCollider;
    public GameObject target;
    bool isTower = false;
    private Coroutine attackCoroutine;
    EnemyScript currentTarget;
    public bool isAttacking = false;
    public float targetCheckTime = 2;
    public float targetCheckTimer = 0;

    public void OnTowerPlaced(DefenseObject placable)
    {
        this.placable = placable;
        maxHealth = placable.health;
        health = placable.health;
        fireRate = placable.FireRate;
        range = placable.range;
        sphereCollider.radius = range;
        damage = placable.damage;
        healthBar.SetHealth(health, maxHealth);
        if(placable.type == DefenseObject.PlacableType.Tower)
        {
            isTower = true;
        }
    }
    void Update()
    {
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckTime) // checks for a target based on a timer
        {
            Debug.Log(target);
            if (target == null)
            {
                SetTarget(ResourceManager.instance.ClosestEnemy(transform.position));
            }
            targetCheckTimer = 0;
        }
        if (isTower)    // checks whether game is lost or not
        {
            if(health <= 0)
            {
                ResourceManager.instance.lost = true;
            }
        }
        else
        {
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
        if(isTower == false)    // makes defense follow closest tower
        {
            if(target != null)
            {
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), Vector3.up);
            }
        }
    }
    //attacks based on attack rate
    private IEnumerator AttackCoroutine()
    {
        while (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= range)
            {
                Attack();
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Attack()
    {
        if (currentTarget == null)
        {
            return;
        }
        currentTarget.OnDamageTaken(damage);
        Debug.Log("tower attack");
    }
    //sets target and starts the attack
    private void SetTarget(GameObject targetToAttack)
    {
        target = targetToAttack;
        if (target == null) return;
        currentTarget = target.GetComponent<EnemyScript>();
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
    }
    private void OnDestroy()
    {
        //stops the attack coroutine
        StopAttacking();
    }
}
