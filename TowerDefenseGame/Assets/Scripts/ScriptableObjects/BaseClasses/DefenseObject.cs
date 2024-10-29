using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyTowerEnums;

[CreateAssetMenu(fileName = "New Defense", menuName = "Placeables/Defenses")]
public class DefenseObject : BasePlacable
{
    public PlacableType type;
    public float FireRate;
    public GameObject projectile;
    public int damage;
    public int rank;
    public float range;
    public float slowTime;
}
