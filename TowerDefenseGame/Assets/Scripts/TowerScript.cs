using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyTowerEnums;

public class TowerScript : TowerParent
{
    void Update()
    {
        base.HandleMouseOver();
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckTime) // checks for a target based on a timer
        {
            //Debug.Log(target);
            if (target == null)
            {
                SetTarget(ResourceManager.instance.ClosestEnemy(transform.position));
            }
            targetCheckTimer = 0;
        }
        
        if(isTower == false)    // makes defense follow closest tower
        {
            if(target != null)
            {
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), Vector3.up);
            }
        }
    }
    private void OnDestroy()
    {
        //stops the attack coroutine
        StopAttacking();
    }
}
