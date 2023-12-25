using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    private int maximumHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] Text healthBarText;
    [SerializeField] Canvas unitCanvas;
    [SerializeField] private GameObject hitBox;
    private HeadBox headBox;
    [SerializeField] private Transform head;

    // Start is called before the first frame update
    private void Start()
    {
        headBox = hitBox.GetComponent<HeadBox>();
        maximumHealth = currentHealth;
        SetHealthBarValues();
    }
    public void SetHealth(int amount, string killedBy, string killHitFrom)
    {
        if (currentHealth > 0)
        {
            currentHealth += amount;
            if (currentHealth <= 0)
            {
                gameObject.tag = "Death";
                //play enemies dying animation.
                //destroy instance with a delay.
                SetHealthBarValues();
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
            
                if (TryGetComponent(out Shield shield))
                    Destroy(GetComponent<Shield>());
                if (TryGetComponent(out Movement movement))
                    movement.MovementSpeed = Vector3.zero;

                if (killHitFrom == "head")
                {
                    headBox.AfterDeathText = "headshot bonus! +" + maximumHealth * 1.5 + " coins";
                }
                else
                {
                    headBox.AfterDeathText = "+" + maximumHealth + " coins";
                }
            
                headBox.transform.position = head.transform.position;
                Instantiate(hitBox, head.transform.position, Quaternion.identity);
                var enemyData = GetComponent<EnemyData>();
                var unitName = enemyData.EnemyName;
                enemyData.KilledByName = killedBy;
                Debug.Log("Death Enemy Data\n"+GetComponent<EnemyData>().EnemyName+"\n"+GetComponent<EnemyData>().KilledByName);
                // SceneTaskManager.Instance.UpdateTaskDictionaryComponents(unitName);
                SceneTaskManager.Instance.UpdateTask(enemyData);
                // SceneEnemiesManager.Instance.UpdateDictionary(gameObject.name, enemyData);
                StartCoroutine(Dying());
            }
            else
                SetHealthBarValues(); 
        }
    }
    private void SetHealthBarValues()
    {
        if (currentHealth > 0)
        {
            healthBar.maxValue = maximumHealth;
            healthBar.value = currentHealth;
            healthBarText.text = currentHealth + "/" + maximumHealth;
        }
        else
        {
            unitCanvas.gameObject.SetActive(false);
        }
    }
    IEnumerator Dying()
    {
        //do something.
        yield return new WaitForSeconds(5f);
        //do something.
        Destroy(gameObject);
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public int MaximumHealth
    {
        get { return maximumHealth; }
        set { maximumHealth = value; }
    }
    public void HideHealthUI()
    {
        unitCanvas.gameObject.SetActive(false);
    }
}