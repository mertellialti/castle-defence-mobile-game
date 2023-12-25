using UnityEngine;
using UnityEngine.UI;
public class TaskComponent : MonoBehaviour
{
    [SerializeField] public Text taskText;
    public string TaskKey { get; set; }
    public int CurrentVal { get; set; }
    public int MaxVal { get; set; }
}
