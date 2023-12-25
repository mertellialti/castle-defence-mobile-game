using UnityEngine;

public class XBotMovement : MonoBehaviour
{
    private float _speed;
    public bool canMove;
    private int direction = -1;
    private float _walkSpeed, _runSpeed;
    [SerializeField] private AudioSource movementAudioSource;

    private void Start()
    {
        _walkSpeed = 4f;
        _runSpeed = 5f;
        
        if (gameObject.layer == 7)
            direction = 1;

        _speed = _walkSpeed;
        canMove = true;
    }

    // Start is called before the first frame update
    private void Update()
    {
        if (!canMove) return;
        transform.position += (Vector3.right / 1000) * (_speed * direction);
    }

    public void SetWalkSpeed()
    {
        _speed = _walkSpeed;
    }

    public void SetRunSpeed()
    {
        _speed = _runSpeed;
    }

    public void PlayWalkSound()
    {
        SoundManager.Instance.PlayWalkSound(movementAudioSource);
    }
    
    public void PlayRunSound()
    {
        SoundManager.Instance.PlayRunSound(movementAudioSource);
    }

    public void StopMovementSound()
    {
        movementAudioSource.clip = null;
    }
}