using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyController : MonoBehaviour
{
    [Header("Currency Texts")]
    [SerializeField] private Text goldText, gemText;

    private void Start()
    {
        goldText.text = "Gold: " + CoinManager.Instance.Coin;
        gemText.text = "Gem: " + CoinManager.Instance.Gem;
    }
}
