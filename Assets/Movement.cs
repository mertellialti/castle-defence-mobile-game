using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Vector3 speedVector;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    private void Start()
    {
        //movementSpeed = Vector3.left; // new Vector3(-1,0,0);
        speedVector = new Vector3(-speed, 0, 0);
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rigidbody.velocity = speedVector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ArrowBlue"))
        {
            var arrowHit = collision.gameObject;
            arrowHit.transform.SetParent(gameObject.transform);
            Debug.Log("Arrow hit!");
            // if hits below body part. 
            // Decrease movemenet velocity, play limping animation. 
        }
    }
    public Vector3 MovementSpeed
    {
        get { return speedVector; }
        set { speedVector = value; }
    }
    public float Speed
    {
        get { return speed; }
        set
        {
            speed = value;
            speedVector.x = -speed;
        }
    }
}
