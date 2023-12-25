using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public int PlayerBowRange { get; set; }
    public GameObject PlayerBow { get; set; }
    public GameObject PlayerArrow { get; set; }
    public int PlayerArrowDamage { get; set; }
    
    //load player manager data from player prefs. 
    //for now, its default values.

    private void Start()
    {
        PlayerBowRange = 20;
        PlayerArrowDamage = 2;
    }
}
