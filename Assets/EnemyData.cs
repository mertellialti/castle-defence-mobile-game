using System;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [SerializeField] private int castleDamage;
    [SerializeField] private string enemyName;
    [SerializeField] private int spawnDelay;
    public string KilledByName { get; set; }
    public int SpawnDelay { get; set; }

    private void Start()
    {
        SpawnDelay = spawnDelay;
        Debug.Log("Delay: " + SpawnDelay);
    }

    public string EnemyName
    {
        get { return enemyName; }
        set { enemyName = value; }
    }

    public int CastleDamage
    {
        get { return castleDamage; }
        set { castleDamage = value; }
    }
}