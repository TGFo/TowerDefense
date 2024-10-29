using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Enemy", menuName = "Enemies/Spawners")]
public class EnemySpawner : EnemyBase
{
    public EnemyBase[] spawnableEnemies;
}
