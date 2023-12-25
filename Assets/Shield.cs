using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] private Transform shield;
    [SerializeField] private Slider shieldBarSlider;
    [SerializeField] private int shieldHealth;

    private void Start()
    {
        shieldBarSlider.minValue = 0;
        shieldBarSlider.maxValue = shieldHealth;
        shieldBarSlider.value = shieldHealth;
    }

    public void ShieldDamage(int damage)
    {
        shieldHealth += damage;
        UpdateShieldBar();
        if (shieldHealth <= 0)
        {
            Destroy(shield.gameObject);
            Destroy(shieldBarSlider.gameObject);
            gameObject.transform.root.GetComponent<Movement>().Speed += .5f;
            Destroy(GetComponent<Shield>());
        }
    }

    private void UpdateShieldBar()
    {
        shieldBarSlider.value = shieldHealth >= 0 ? 0 : shieldHealth;
    }
}