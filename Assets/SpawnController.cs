using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;

    [Header("Task Management")] [SerializeField]
    private List<GameObject> enemyList;

    [SerializeField] private List<int> numberOfEnemiesToKilled;
    [SerializeField] private List<GameObject> killEnemyWithItemList;

    [Header("Tasks UI")] [SerializeField] private GameObject taskComponentPrefab;
    [SerializeField] private Transform taskContainerTransform;
    private TaskComponent taskComponentData;

    private float timer = 0f;
    private float delay = 0f;
    private float timerMax = 4f;

    private Dictionary<string, List<LevelTask>> Tasks = new Dictionary<string, List<LevelTask>>();

    private async void Start()
    {
        SceneTaskManager.Instance.isLevelVictory = false;
        SceneTaskManager.Instance.ResetDictionary();
        Time.timeScale = 0;
        await TaskCreator();
        Time.timeScale = 1;
    }
    private void Update()
    {
        if (!SceneTaskManager.Instance.isLevelVictory)
        {
            timer += Time.deltaTime;
            if (timer >= timerMax + delay)
            {
                timer = 0;
                var randomEnemy = RandomEnemy();
                randomEnemy.layer = 6;
                var enemySpawned = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);

                if (enemySpawned.TryGetComponent(out XBotLogic enemyData))
                {
                    // enemyData = enemySpawned.GetComponent<EnemyData>();
                    // enemySpawned.name = enemyData.EnemyName;
                    delay = 0 + Random.Range(3, 6);
                }
                else
                {
                    Debug.Log("couldn't find enemy data!");
                }
                timer = 0;
            }
        }
    }
    private GameObject RandomEnemy()
    {
        var randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }
    private async Task TaskCreator()
    {
        taskContainerTransform.gameObject.SetActive(false);
        for (int i = 0; i < enemyList.Count; i++)
        {
            LevelTask task;

            // var unitName = enemyList[i].GetComponent<EnemyData>().EnemyName;
            var unitName = "X_BOT";
            var amount = numberOfEnemiesToKilled[i];

            var tComponent = Instantiate(taskComponentPrefab, taskContainerTransform, true);
            var tcData = tComponent.GetComponent<TaskComponent>();

            var uiTaskComponentData = tComponent.GetComponent<TaskComponent>();

            if (killEnemyWithItemList[i] == null)
            {
                task = new LevelTask(unitName, amount, tcData);
                uiTaskComponentData.taskText.text = "Kill " + unitName + " 0/" + amount;
            }
            else
            {
                var killedByName = killEnemyWithItemList[i].name;
                task = new LevelTask(unitName, killedByName, amount, tcData);
                uiTaskComponentData.taskText.text = "Kill " + unitName + " with " + killedByName + " 0/" + amount;
            }

            if (!Tasks.ContainsKey(unitName))
            {
                List<LevelTask> lt = new List<LevelTask>();
                lt.Add(task);
                Tasks.Add(unitName, lt);
            }
            else
            {
                Tasks[unitName].Add(task);
            }
        }

        taskContainerTransform.gameObject.SetActive(true);

        SceneTaskManager.Instance.Tasks = Tasks;
        PrintTasksStatus();
        await Task.Yield();
    }
    public void PrintTasksStatus()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            var l = Tasks.ElementAt(i).Value;
            for (int j = 0; j < l.Count; j++)
            {
                Debug.Log("TASK--> " + Tasks.Keys.ElementAt(i) + "\n" + (j + 1) + ". " + l[j].UnitName + " " +
                          l[j].CurrentAmount + "/" + l[j].Amount);
            }
        }
    }
}