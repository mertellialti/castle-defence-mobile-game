using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    private int coin;
    private int gem;

    private void Start()
    {
        // Load player coin data.
        // Assaign current coin amount      
        // LoadCoinData();        
        //Demo amount gem
        Gem = 8;
    }
    private void SaveCoinData()
    {
        // save current data.
        PlayerPrefs.SetInt("Coins", coin);
    }
    public int Coin
    {
        get { return coin; }
        set
        {
            coin = value;
            // SaveCoinData();
        }
    }
    public int Gem
    {
        get { return gem; }
        set
        {
            gem = value;
            // SaveCoinData();
        }
    }
    private void LoadCoinData()
    {
        coin = PlayerPrefs.GetInt("Coins");
        Debug.Log(PlayerPrefs.GetInt("Coins"));
        //loads coin data from player pref. 
    }
}
