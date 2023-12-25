using System;
using System.Collections;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject reinforcePrefab;
    [SerializeField] private Text wavesText;
    [SerializeField] private Text enemyWaveText;
    [SerializeField] private Text waveCounterText;
    [SerializeField] private Text level;
    [SerializeField] private Transform reinforceCamp;
    [SerializeField] private GameObject scout;
    
    private int counter;
    public float spawnRate;

    // Define variables
    private int currentLevel = 2;
    int currentWave = 0;
    private int _maxWave = 3;
    int totalEnemies = 0;
    int enemiesSpawned = 0;
    private int numEnemies = 0;
    private bool levelDone = false;
    private bool _canStartCountdown = false;

// Define arrays for enemy types and their spawn chances
private string[] enemyTypes = { "weak", "mid", "strong", "loyal" };
private int[] enemyTypeId = { 0, 1, 2, 3 };
private float[] enemySpawnChances = { 0.4f, 0.3f, 0.2f, 0.1f };

    private void Start()
    {
        // GameObject.Find("Canvas").gameObject.SetActive(false);
        level.text = "LEVEL " + currentLevel;
        StartCoroutine(TransitionToTarget());
    }

    // Define a function to spawn a single enemy
    private void SpawnEnemy(int enemyTypeId)
    {
        var enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        var enemyHealth = enemy.GetComponentInChildren<XBotHealth>();
        // Spawn enemy logic here
        switch (enemyTypeId)
        {
            case 0:
                enemyHealth.AssignHealthBar(2, 2);
                break;
            case 1:
                enemyHealth.AssignHealthBar(4, 4);
                break;
            case 2:
                enemyHealth.AssignHealthBar(6, 6);
                break;
            case 3:
                enemyHealth.AssignHealthBar(10, 10);
                break;
        }
    }

// Define a function to spawn a wave of enemies
    private IEnumerator SpawnWave(int waveNumber, int difficulty)
    {
        SceneEnemiesManager.Instance.enemiesToKillInWave = numEnemies;
        SceneEnemiesManager.Instance.enemiesKilledInWave = 0;

        // Spawn enemies randomly, based on the defined spawn chances for each enemy type
        for (var i = 0; i < numEnemies; i++)
        {
            var randomNum = Random.Range(0f, 1f);
            var spawnChance = 0f;

            for (var j = 0; j < enemyTypes.Length; j++)
            {
                spawnChance += enemySpawnChances[j];
                if (randomNum <= spawnChance)
                {
                    SpawnEnemy(enemyTypeId[j]);
                    enemiesSpawned++;
                    break;
                }
            }

            // Add a random delay between enemy spawns within a wave
            var spawnDelay = Random.Range(4f, 6f);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Update variables
        totalEnemies += numEnemies;
        // currentWave = waveNumber;
    }

// Update function
    private void Update()
    {
        // Check if all soldiers from the previous wave are killed
        // if (enemiesSpawned == totalEnemies && totalEnemies > 0)
        if (currentWave == _maxWave && !levelDone && SceneEnemiesManager.Instance.enemiesKilledInWave == numEnemies)
        {
            levelDone = true;
            enemyWaveText.gameObject.SetActive(false);
            Time.timeScale = 0.1f;
            GameObject.Find("Canvas").SetActive(false);
            return;
        }

        {
            enemyWaveText.text = "Wave " + (currentWave) + "/" + _maxWave + " enemies to spawn in wave " +
                                 enemiesSpawned + "/" +
                                 numEnemies + " enemies killed in wave " +
                                 SceneEnemiesManager.Instance.enemiesKilledInWave + "/" + numEnemies;
        }

        if (numEnemies == enemiesSpawned && SceneEnemiesManager.Instance.enemiesKilledInWave == numEnemies &&
            !levelDone && _canStartCountdown)
        {
            currentWave++;
            Debug.LogWarning("Start next wave");
            // If all soldiers are killed, spawn the next wave with increased difficulty
            StartCoroutine(WaveCounter());
        }
    }

    IEnumerator WaveCounter()
    {
        Time.timeScale = 0.1f;
        // Calculate the number of enemies to spawn based on the wave number and difficulty
        var difficulty = Mathf.FloorToInt(currentLevel / 5) + 1;
        // numEnemies = Mathf.RoundToInt(Mathf.Pow(waveNumber, 1.5f) + (difficulty * 5));
        SceneEnemiesManager.Instance.enemiesKilledInWave = 0;
        numEnemies = Mathf.RoundToInt(Mathf.Pow(currentWave, 1.5f) + (difficulty * 5));
        // numEnemies = 1;
        // numEnemies = Mathf.RoundToInt(Mathf.Pow(currentLevel, 1.5f) + (difficulty * 5));
        // numEnemies = 4 + (3 * (currentLevel - 1)) + (currentLevel * currentWave);
        // Debug.LogError("Enemies to spawn im wave");
        enemiesSpawned = 0;
        waveCounterText.gameObject.SetActive(true);
        waveCounterText.text = "3";
        yield return new WaitForSeconds(.1f);
        waveCounterText.text = "2";
        yield return new WaitForSeconds(.1f);
        waveCounterText.text = "1";
        yield return new WaitForSeconds(.1f);
        if (currentWave != _maxWave)
            waveCounterText.text = "WAVE " + currentWave + " BEGINS";
        else
            waveCounterText.text = "LAST WAVE COMING";
        yield return new WaitForSeconds(.2f);
        waveCounterText.gameObject.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(SpawnWave(currentWave + 1, difficulty));
        yield return null;
    }

    public void InitializeReinforce()
    {
        var reinforce = Instantiate(reinforcePrefab, reinforceCamp.position, reinforceCamp.rotation);
        reinforce.GetComponentInChildren<XBotHealth>().AssignHealthBar(3, 3);
        Debug.Log("Reinforce " + reinforce.name + " is spawned!");
    }
    
    IEnumerator TransitionToTarget()
    {
        _canStartCountdown = false;
        var transitionTime = 2.0f;
        var targetPosition1 = new Vector3(12, 12, -15);
        var targetPosition2 = new Vector3(-5, 14, -15);
        var targetSize = 12;
        // var startPosition = new Vector3(-7.5f,14,-15);
        var startPosition = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z);
        var startSize = Camera.main.orthographicSize;
        
        var t = 0.0f;
        
        yield return new WaitForSeconds(2);

        while (t < 1.0f)
        {
            t += Time.deltaTime / transitionTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition2, t);
            yield return null;
        }

        t = 0.0f;
        Destroy(scout);

        while (t < 1.0f)
        {
            t += Time.deltaTime / transitionTime;
            Camera.main.transform.position = Vector3.Lerp(targetPosition2, targetPosition1, t);
            Camera.main.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }
        // GameObject.Find("Canvas").gameObject.SetActive(true);
        _canStartCountdown = true;
    }
}