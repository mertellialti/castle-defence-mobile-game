using System;
using UnityEngine;
public class BaseData : MonoBehaviour
{
    private string name;
    private void Start()
    {
        name = gameObject.tag;
        Debug.Log("name: " +name);
    }
}
