using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyTowerEnums;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/BaseEnemy")]
public class EnemyBase : ScriptableObject
{
    public EnemyType enemyType;
    public int preferredDistance;
    public float range;
    public float damage;
    public float attackSpeed;
    public float maxHealth;
    public float regenRate;
    public float speedMultiplier;
    public int rank;
    public int killReward;
    public GameObject model;
}
