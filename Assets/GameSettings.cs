using UnityEngine;

[System.Serializable]
public class GameSettings: MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
}