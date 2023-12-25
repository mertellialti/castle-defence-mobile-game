using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("on trigger enter " + other);
        if (other.gameObject.layer != gameObject.layer)
        {
            // play sword sound sound
            if (transform.root.name.Contains("Death"))
            {
                Debug.Log("should play stab sound");
                SoundManager.Instance.PlayStabSound(audioSource);    
            }
            
            else
            {
                Debug.Log("should play sword sound");
                SoundManager.Instance.PlaySwordSound(audioSource);
            }
        }
    }
}