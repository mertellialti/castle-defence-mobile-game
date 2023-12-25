using UnityEngine;
public class SceneEnemiesManager : Singleton<SceneEnemiesManager>
{
    public int enemiesKilledInWave = 0;
    public int enemiesKilledInLevel = 0;
    public int enemiesToKillInWave;
    public int enemiesToKilledInLevel;
    public void RetreatAllEnemies()
    {
        var aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < aliveEnemies.Length; i++)
        {
            aliveEnemies[i].transform.RotateAround(Vector3.up, 180f);
            aliveEnemies[i].GetComponent<Movement>().Speed *= -1;
            aliveEnemies[i].GetComponent<EnemyHealth>().HideHealthUI();
        }
    }
}