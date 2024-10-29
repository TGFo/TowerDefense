using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TowerAOE : TowerParent
{
    public List<GameObject> targets = new List<GameObject>(10);
    public List<EnemyScript> enemyScripts = new List<EnemyScript>(10);
    public int maxTargets = 10;
    public override void Attack(EnemyScript target)
    {
        if (target == null)
        {
            return;
        }
        target.OnDamageTaken(damage);
    }

    public override IEnumerator AttackCoroutine()
    {
        while (target != null)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (Vector3.Distance(transform.position, targets[i].transform.position) <= range)
                {
                    Attack(enemyScripts[i]);
                }
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    public override void OnTowerPlaced(DefenseObject placable)
    {
        base.OnTowerPlaced(placable);
    }

    public override void SetTarget(GameObject targetToAttack)
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
    public void SetTargets(GameObject[] targetsToAttack)
    {
        if(targetsToAttack.Length == 0)
        {
            return;
        }
        target = targetsToAttack[0];
        targets.AddRange(targetsToAttack);
        if (target == null) return;
        for(int i = 0; i < targetsToAttack.Length; i++)
        {
            if (targets[i] == null) return;
            enemyScripts.Add(targetsToAttack[i].GetComponent<EnemyScript>());
        }
        currentTarget = target.GetComponent<EnemyScript>();
        if (!isAttacking)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
            isAttacking = true;
        }
    }
    public override void StopAttacking()
    {
        if (isAttacking)
        {
            StopCoroutine(attackCoroutine);
            isAttacking = false;
            target = null;
            currentTarget = null;
            targets.Clear();
            enemyScripts.Clear();
        }
    }

    void Update()
    {
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckTime) // checks for a target based on a timer
        {
            targets.Clear();
            enemyScripts.Clear();
            Debug.Log(target);
            if (target == null)
            {
                SetTargets(ResourceManager.instance.ClosestEnemies(transform.position));
            }
            targetCheckTimer = 0;
        }

        if (isTower == false)    // makes defense follow closest tower
        {
            if (target != null)
            {
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), Vector3.up);
            }
        }
    }
}
