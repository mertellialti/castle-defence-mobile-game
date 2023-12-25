using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class XBotHealth : MonoBehaviour
{
    [SerializeField] public int MaxHealth = 5;
    [SerializeField] public int Health = 5;
    [SerializeField] private Text healthText;
    [SerializeField] private Animator animator;
    private XBotLogic xBotLogic;
    [SerializeField] private Transform head;
    [SerializeField] private GameObject alertPrefab;
    [SerializeField] private Slider healthBar;
    [SerializeField] Image healthBarFiller;
    private Color _defaultColor;
    [SerializeField] private Gradient gradient;
    private bool isPlayingAnimation = false;
    private float _timeToDrain = .25f;
    [SerializeField] private AudioSource healthAudioSource;
    private XBotMovement xBotMovement;

    // private void Awake()
    // {
    //     // MaxHealth = 5;
    //     Health = MaxHealth;
    //     healthBar.maxValue = MaxHealth;
    //     healthBar.value = MaxHealth;
    // }

    private void Start()
    {
        xBotMovement = GetComponent<XBotMovement>();
        xBotLogic = GetComponent<XBotLogic>();
        // _defaultColor = healthBarFiller.color;
        // healthText.text = Health + "/" + MaxHealth;
    }

    public void ReduceHealth(int amount)
    {
        
        if(xBotLogic.isDead)
            return;
        
        if (xBotLogic.isDefending)
        {
            Debug.Log("Defending");
            SoundManager.Instance.PlayShieldSound(healthAudioSource);
            StartCoroutine(SliderFlash(Color.blue, .5f));
            xBotLogic.isDefending = false;
        }
        else
        {
            Debug.Log("Damage taken  " + amount);
            if ((Health - amount) <= 0)
            {
                if (gameObject.layer == 6)
                {
                    SceneEnemiesManager.Instance.enemiesKilledInLevel++;
                    SceneEnemiesManager.Instance.enemiesKilledInWave++;
                }
                transform.root.name = "Death";
                xBotMovement.canMove = false;
                xBotLogic.isDead = true;
                SoundManager.Instance.PlayDeathSound(healthAudioSource);
                Health = 0;
                healthText.text = "Dead";
                healthBar.gameObject.SetActive(false);
                animator.Play("Death");
                xBotMovement.StopMovementSound();
                Invoke("Die",4f);
            }
            else
            {
                Health -= amount;
                healthText.text = Health + "/" + MaxHealth;
                // SoundManager.Instance.PlayHurtSound(healthAudioSource);
                healthBarFiller.color = gradient.Evaluate((float)Health / MaxHealth);
                StartCoroutine(SliderFlash(Color.red, .5f));
                healthBar.value = Health;
            }
        }
    }

    IEnumerator SliderFlash(Color flashColor, float duration)
    {
        isPlayingAnimation = true;
        healthBarFiller.color = flashColor;
        healthText.gameObject.SetActive(false);
        yield return new WaitForSeconds(duration);
        healthBarFiller.color = gradient.Evaluate((float)Health / MaxHealth);
        healthText.gameObject.SetActive(true);
    }

    private void Die()
    {
        var inf = animator.GetCurrentAnimatorClipInfo(0);
        var animName = inf[0].clip.name;
        var clipLength = inf[0].clip.length;
        Destroy(head.root.gameObject);
    }

    public void AssignHealthBar(int health, int maxHealth)
    {
        Health = health;
        MaxHealth = maxHealth;
        healthBar.maxValue = MaxHealth;
        healthBar.value = MaxHealth;
        _defaultColor = healthBarFiller.color;
        healthText.text = Health + "/" + MaxHealth;
    }
}