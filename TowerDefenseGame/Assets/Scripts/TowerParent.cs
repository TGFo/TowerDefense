using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyTowerEnums;

public abstract class TowerParent : MonoBehaviour
{
    public HealthBar healthBar;
    public DefenseObject placable;
    public PlacableType placableType;

    public float maxHealth;
    public float health;
    public float fireRate;
    protected float damage;
    public float range;
    public SphereCollider sphereCollider;
    public GameObject target;
    protected bool isTower = false;
    private protected Coroutine attackCoroutine;
    protected EnemyScript currentTarget;
    public bool isAttacking = false;
    public float targetCheckTime = 2;
    public float targetCheckTimer = 0;

    public virtual void OnTowerPlaced(DefenseObject placable)
    {
        this.placable = placable;
        maxHealth = placable.health;
        health = placable.health;
        fireRate = placable.FireRate;
        range = placable.range;
        sphereCollider.radius = range;
        damage = placable.damage;
        placableType = placable.type;
        healthBar.SetHealth(health, maxHealth);
        if (placable.type == PlacableType.Tower)
        {
            isTower = true;
        }
    }
    //attacks based on attack rate
    public virtual IEnumerator AttackCoroutine()
    {
        while (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= range)
            {
                Attack(currentTarget);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    public virtual void Attack(EnemyScript target)
    {
        if (target == null)
        {
            return;
        }
        target.OnDamageTaken(damage);
        Debug.Log("Attack" + gameObject.name);
    }
    //sets target and starts the attack
    public virtual void SetTarget(GameObject targetToAttack)
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

    public virtual void StopAttacking()
    {
        if (isAttacking)
        {
            //StopCoroutine(attackCoroutine);
            isAttacking = false;
            target = null;
            currentTarget = null;
        }
    }
    public virtual void OnDamageTaken(float takenDamage)
    {
        //Debug.Log("damage taken");
        health -= takenDamage;
        healthBar.SetHealth(health, maxHealth);
        if (health <= 0)
        {
            OnKilled();
        }
    }
    public void OnKilled()
    {
        ResourceManager.instance.spawnedDefenses.Remove(gameObject);
        if (isTower)    // checks whether game is lost or not
        {
            ResourceManager.instance.lost = true;
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        //stops the attack coroutine
        StopAttacking();
    }
}
