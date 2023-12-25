using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleHealthManager : Singleton<CastleHealthManager>
{
    private int castleCurrentHealth;
    private int castleMaximumHealth;

    private void Start()
    {
        // Load castle health data.
        // Assaign current health amount.
        LoadHealthData();
    }
    public void SetCastleHealth(int amount)
    {
        // add given amount to the castle health amount.
        // save health.
        castleCurrentHealth += amount;
    }
    public void SetCastleMaximumHealth(int amount)
    {
        // add given amount to the castle health amount.
        // save health.
        castleMaximumHealth += amount;
    }
    public void SaveCastleHealthChange()
    {
        // save current data.
    }
    private void SaveHealthData()
    {
        // save health data to player pref.
    }
    private void LoadHealthData()
    {
        // loads health data from player pref.
        if (!PlayerPrefs.HasKey("GateHealthCurrent"))
        {
            PlayerPrefs.SetInt("GateHealthCurrent", 10);
            PlayerPrefs.SetInt("GateHealthMax", 10);
            Debug.Log("Here " + PlayerPrefs.GetInt("GateHealthMax"));
        }
        else
        {
            LoadGateHealth();
            Debug.Log("Here " + PlayerPrefs.GetInt("GateHealthMax"));
        }
    }
    private void LoadGateHealth()
    {
        castleCurrentHealth = PlayerPrefs.GetInt("GateHealthCurrent");
        castleMaximumHealth = PlayerPrefs.GetInt("GateHealthMax");
        Debug.Log(castleCurrentHealth + " " + castleMaximumHealth);
    }

    public int CastleCurrentHealth
    {
        get { return castleCurrentHealth; }
        set
        {
            castleCurrentHealth = value;
            SaveHealthData();
        }
    }
    public int CastleMaximumtHealth
    {
        get { return castleMaximumHealth; }
        set
        {
            castleMaximumHealth = value;
            SaveHealthData();
        }
    }
}
