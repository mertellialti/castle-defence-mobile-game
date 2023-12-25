using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//list of arrow types
public enum arrowTypes
{
    Normal,
    Freeze,
    Explode
}

public class Arrow : MonoBehaviour
{
    //visible in the inspector
    public arrowTypes arrowType;
    public GameObject freezeArea;
    public GameObject explosion;
    public AudioClip hit;
    public AudioClip iceHit;
    private int damage = 1;

    public List<string> legColliderNames;
    public List<string> chestBoneNames;
    public List<string> headBoneNames;

    //not visible in the inspector
    Rigidbody rigidbodyComponent;
    bool flying = true;
    float startDistance;

    AudioSource audioSource;

    void Start()
    {
        //get the rigidbody and the audiosource
        rigidbodyComponent = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        damage = PlayerManager.Instance.PlayerArrowDamage;
    }

    void Update()
    {
        //if the arrow is flying, rotate it in a natural way
        if (flying)
            transform.LookAt(transform.position + rigidbodyComponent.velocity);

        //if this arrow hit the legs of an enemy and it's parented to the enemy, make sure it moves with the enemy legs
        if (transform.parent && transform.parent.name == "legs collider")
            transform.position = new Vector3(transform.position.x,
                transform.root.gameObject.GetComponent<Enemy>().leg.position.y + startDistance, transform.position.z);
    }

    //when this arrow hits a collider...
    void OnTriggerEnter(Collider col)
    {
        //check if it didn't hit another arrow,if it's flying and if it didn't hit a freezearea
        if (!col.gameObject.GetComponent<Arrow>() && flying && !col.gameObject.GetComponent<FreezeArea>())
        {
            //stop the arrow, turn off gravity for the arrow, parent it to the hit gameobject and disable its collider
            rigidbodyComponent.velocity = Vector3.zero;
            rigidbodyComponent.isKinematic = true;
            transform.parent = col.gameObject.transform;
            GetComponent<Collider>().enabled = false;
            flying = false;

            #region my addition

            var rootGameObj = col.gameObject.transform.root.GetChild(0).gameObject;
            Debug.Log("Found " + rootGameObj);
            Debug.Log(rootGameObj);
            Debug.Log(col.tag);

            //enemy layer 6, friendly 7
            if (rootGameObj.layer == 6)
            {
                if (col.transform.CompareTag("Head"))
                {
                    if (rootGameObj.TryGetComponent(out XBotHealth health))
                    {
                        health.ReduceHealth(damage);
                    }
                    else
                    {
                        Debug.Log("not found");
                    }
                }
                else if (col.transform.CompareTag("Body"))
                {
                    if (rootGameObj.TryGetComponent(out XBotHealth health))
                    {
                        health.ReduceHealth(damage);
                    }
                    else
                        Debug.Log("not found");
                }
                else if (col.transform.CompareTag("Leg"))
                {
                    if (rootGameObj.TryGetComponent(out XBotHealth health))
                    {
                        // health.SetHealth(-damage, gameObject.name, "leg");
                        health.ReduceHealth(damage);
                    }

                    if (rootGameObj.TryGetComponent(out Movement movement))
                    {
                        movement.Speed = movement.Speed / 2;
                    }
                }
                else if (col.transform.CompareTag("Shield"))
                {
                    if (rootGameObj.TryGetComponent(out Shield shield))
                    {
                        shield.ShieldDamage(-damage);
                    }
                }
            }

            #endregion

            //get the root object of the object that was hit
            GameObject rootObject = col.gameObject.transform.root.gameObject;

            //instantiate a freezearea if this is a freezearrow
            if (arrowType == arrowTypes.Freeze)
                Instantiate(freezeArea, transform.position, Quaternion.identity);

            //get the enemy script from the hit object (if it has one)
            Enemy enemyscript = rootObject.GetComponent<Enemy>();

            //check for the enemyscript and if it's not a frozen enemy
            if (enemyscript && !enemyscript.freeze)
            {
                RaycastHit hit;
                Vector3 fwd = transform.TransformDirection(Vector3.forward);

                //with a raycast, check the front of the arrow in order to instantiate the effect on time
                if (Physics.Raycast(transform.position, fwd, out hit))
                {
                    if (hit.distance < 1.5f && hit.transform.root.gameObject.GetComponent<Enemy>())
                    {
                        //instantiate the blood effect, parent it to the arrow and rotate it correctly
                        GameObject bloodEffect =
                            Instantiate(hit.transform.root.gameObject.GetComponent<Enemy>().bloodEffect, hit.point,
                                transform.rotation) as GameObject;
                        bloodEffect.transform.parent = gameObject.transform;
                        bloodEffect.transform.Rotate(Vector3.up * 180);
                    }
                }
            }

            //if there is an effect, stop it because the arrow is not flying anymore
            if (transform.Find("effect"))
                transform.Find("effect").gameObject.GetComponent<ParticleSystem>().Stop();

            //if the arrow hit legs...
            if (legColliderNames.Contains(col.gameObject.name))
            {
                //set a startdistance between the arrow and the leg to make the arrow move with the leg in update
                startDistance = Mathf.Abs(transform.root.gameObject.GetComponent<Enemy>().leg.position.y -
                                          transform.position.y);
                //start the leghit coroutine
                StartCoroutine(enemyscript.legHit());
                //play some audio
                playAudio(1f, 0.7f, enemyscript);
            }
            else if (chestBoneNames.Contains(col.gameObject.name))
            {
                //start the hit coroutine
                enemyscript.hit();
                //play some audio
                playAudio(1f, 0.8f, enemyscript);
            }
            else if (headBoneNames.Contains(col.gameObject.name))
            {
                //start the headshot coroutine
                StartCoroutine(enemyscript.headShot(this.gameObject));
                //play some audio
                playAudio(1f, 1f, enemyscript);
            }
            else
            {
                //play some audio
                playAudio(3f, 0.4f, enemyscript);
            }

            //if this is a tutorial and the arrow hit a target, tell the tutorial that the target was hit
            if (rootObject.GetComponent<Tutorial>())
                rootObject.GetComponent<Tutorial>().hit();

            //if this is an exploding arrow, add the explosion and destroy the arrow
            if (arrowType == arrowTypes.Explode)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    //play audio with a certain pitch and volume
    void playAudio(float pitch, float volume, Enemy enemy)
    {
        //if this is not an enemy or the enemy is not frozen, play normal sound
        if (!enemy || !enemy.freeze)
        {
            audioSource.clip = hit;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
        }
        //else, play ice hit sound
        else
        {
            audioSource.clip = iceHit;
            audioSource.pitch = 1.5f;
        }

        //actually play the sound
        audioSource.Play();
    }
}