using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private List<Transform> levelUIComponents;

    private void Start()
    {
        SceneTaskManager.Instance.UIController = this;
    }
    public void HideAll()
    {
        //If they are included in the same canvas...
        levelUIComponents[0].root.gameObject.SetActive(false);
        //If not...
        // foreach (var cmp in levelUIComponents)
        // {
        //     cmp.gameObject.SetActive(false);
        // }
    }

    public void ShowAll()
    {
        // foreach (var cmp in levelUIComponents)
        // {
        //     cmp.gameObject.SetActive(true);
        // }
    }
}