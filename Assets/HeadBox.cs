using UnityEngine;
using UnityEngine.UI;

public class HeadBox : MonoBehaviour
{
    private float timer = 0f;
    private float duration = 5f;
    private int opacity = 255;
    [SerializeField] private Text text;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position += new Vector3(0, Time.deltaTime, 0);
        }
    }

    public string AfterDeathText
    {
        get { return text.text; }
        set { text.text = value; }
    }
}