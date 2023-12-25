using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateHealth : MonoBehaviour
{
    private int currentHealth;
    private int maximumHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] Text healthBarText;
    List<string> enemiesReachedToCastle;
    List<int> amountOfEnemiesReachedToCastle;

    private void Start()
    {
        //Get and set current health data from health manager.(This is Health Manager's work, will get from player prefs)
        //Prepare the health bar.
        currentHealth = CastleHealthManager.Instance.CastleCurrentHealth;
        maximumHealth = CastleHealthManager.Instance.CastleMaximumtHealth;
        Debug.Log("health: " + CastleHealthManager.Instance.CastleCurrentHealth + "/" +CastleHealthManager.Instance.CastleMaximumtHealth);
        UpdateHealthBarUI();
    }
    public void SetCastleHealth(int amount)
    {
        currentHealth += amount;
        UpdateHealthBarUI();
    }
    public void SetCastleMaxHealth(int amount)
    {
        maximumHealth += amount;
        UpdateHealthBarUI();
    }
    public void EnemiesReachedToCastle(string id)
    {
        // Store enemies reached to castle in level. 
    }
    private void UpdateHealthBarUI()
    {
        if (currentHealth <= 0)
        {
            // Stop the game.
            healthBar.gameObject.SetActive(false);
            healthBarText.gameObject.SetActive(false);
        }
        else
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maximumHealth;
            healthBar.value = currentHealth;
            healthBarText.text = currentHealth + "/ " + maximumHealth;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy reached to castle.");
        if (collision.gameObject.layer == 6)
        {
            if (collision.gameObject.TryGetComponent(out EnemyData data))
            {
                SetCastleHealth(-data.CastleDamage);
                // health.SetCastleHealth(-damageOnTower);
                Destroy(collision.gameObject);
            }
        }
    }
}
