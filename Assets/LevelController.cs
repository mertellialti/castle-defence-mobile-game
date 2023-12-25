using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
// set up difficulty level parameters
    int currentLevel = 1;
    int maxEnemiesPerWave = 10;
    float timeBetweenWaves = 30.0f;
    float timeSinceLastWave = 0.0f;
    float weakerEnemySpawnChance = 0.5f;
    float midEnemySpawnChance = 0.3f;
    float strongEnemySpawnChance = 0.2f;

// spawn enemies
    void SpawnEnemies()
    {
        var enemiesToSpawn = maxEnemiesPerWave * currentLevel;
        var numWeakEnemies = (int)(enemiesToSpawn * weakerEnemySpawnChance);
        var numMidEnemies = (int)(enemiesToSpawn * midEnemySpawnChance);
        var numStrongEnemies = (int)(enemiesToSpawn * strongEnemySpawnChance);
        var numLoyalEnemies = enemiesToSpawn - numWeakEnemies - numMidEnemies - numStrongEnemies;

        for (var i = 0; i < numWeakEnemies; i++)
        {
            // spawn weak enemy at random location
            // SpawnEnemy("weak", Random.Range(0, mapWidth), Random.Range(0, mapHeight));
        }

        for (var i = 0; i < numMidEnemies; i++)
        {
            // spawn mid enemy at random location
            // SpawnEnemy("mid", Random.Range(0, mapWidth), Random.Range(0, mapHeight));
        }

        for (var i = 0; i < numStrongEnemies; i++)
        {
            // spawn strong enemy at random location
            // SpawnEnemy("strong", Random.Range(0, mapWidth), Random.Range(0, mapHeight));
        }

        for (var
             i = 0;
             i < numLoyalEnemies;
             i++)
        {
            // spawn loyal enemy at random location
            // SpawnEnemy("loyal", Random.Range(0, mapWidth), Random.Range(0, mapHeight));
        }
    }

// update game loop
    void Update()
    {
        timeSinceLastWave += Time.deltaTime;
        if (timeSinceLastWave >= timeBetweenWaves)
        {
            SpawnEnemies();
            timeSinceLastWave = 0.0f;
            currentLevel++;

            // adjust difficulty based on current level
            if (currentLevel <= 5)
            {
                maxEnemiesPerWave = 5;
                weakerEnemySpawnChance = 0.6f;
                midEnemySpawnChance = 0.3f;
                strongEnemySpawnChance = 0.1f;
            }
            else if (currentLevel <= 10)
            {
                maxEnemiesPerWave = 10;
                weakerEnemySpawnChance = 0.5f;
                midEnemySpawnChance = 0.3f;
                strongEnemySpawnChance = 0.2f;
            }
            else if (currentLevel <= 15)
            {
                maxEnemiesPerWave = 15;
                weakerEnemySpawnChance = 0.4f;
                midEnemySpawnChance = 0.3f;
                strongEnemySpawnChance = 0.3f;
            }
            else
            {
                maxEnemiesPerWave = 20;
                weakerEnemySpawnChance = 0.3f;
                midEnemySpawnChance = 0.3f;
                strongEnemySpawnChance = 0.4f;
            }
        }
    }
}