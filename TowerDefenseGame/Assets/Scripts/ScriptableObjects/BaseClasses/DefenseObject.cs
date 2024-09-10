using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Placeables/Defenses")]
public class DefenseObject : BasePlacable
{
    public enum PlacableType
    {
        Tower,
        Defense
    }
    public PlacableType type;
    public float FireRate;
    public GameObject projectile;
    public int damage;
    public int rank;
    public float range;
}
