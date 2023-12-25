using System.Collections;
using UnityEngine;

public class XBotCombat : MonoBehaviour
{
    private GameObject Opponent { get; set; }
    private XBotHealth XBotHealth { get; set; }
    [SerializeField] private int damage;
    [SerializeField] Animator animator;
    public GameObject target;
    public XBotHealth targetHealth;
    
    private void Awake()
    {
        damage = 2;
    }

}