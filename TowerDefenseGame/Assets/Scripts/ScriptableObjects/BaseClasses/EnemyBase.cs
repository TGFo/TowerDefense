using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies")]
public class EnemyBase : ScriptableObject
{
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
