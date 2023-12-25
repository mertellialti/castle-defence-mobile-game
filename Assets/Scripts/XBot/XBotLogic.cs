using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class XBotLogic : MonoBehaviour
{
    // If eye has logic, keep fighting.
    [Header("X-BOT Scripts")] private XBotEye _xBotEye;
    private XBotMovement _xBotMovement;

    private bool _isAttacking;
    public bool isDefending;
    private Animator _animator;
    public int attackCounter = 0;
    public int defenceCounter = 0;
    private AudioSource _audioSource;
    public bool isDead;
    private float animationLength = 0;
    private float attackRate = 2f;

    private bool isWalking, isRunning;

    private void Start()
    {
        isWalking = false;
        isRunning = true;
        _isAttacking = false;
        _audioSource = GetComponent<AudioSource>();
        _xBotMovement = GetComponent<XBotMovement>();
        _xBotEye = transform.GetChild(3).GetComponent<XBotEye>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (transform.position.x <= 0f && gameObject.layer == 6)
        {
            StartCoroutine(DamageOnCastle());
        }
    }

    private void LateUpdate()
    {
        if (isDead)
            return;
        
        if (_xBotEye.enemyLogic && !_isAttacking && !_xBotEye.enemyLogic.isDead)
        {
            isWalking = false;
            isRunning = false;
            _xBotMovement.StopMovementSound();
            _xBotMovement.canMove = false;
            _isAttacking = true;
            if (!_xBotEye.enemyLogic.isDead)
                Attack();
        }
        else if ((!_xBotEye.enemyLogic || _xBotEye.enemyLogic.isDead) && (!_isAttacking) &&(!isDefending) && (!_xBotEye.enemyInPath && !_xBotEye.enemyInCombatRange) && !isWalking && isRunning )
        {
            isWalking = true;
            isRunning = false;
            _animator.Play("Walk");
            _xBotMovement.canMove = true;
            _xBotMovement.SetWalkSpeed();
            _xBotMovement.PlayWalkSound();
            Debug.Log("logic is moving");
        }
        else if ((!_xBotEye.enemyLogic || _xBotEye.enemyLogic.isDead) && (!_isAttacking) &&(!isDefending) && (_xBotEye.enemyInPath && !_xBotEye.enemyInCombatRange) && !isRunning && isWalking )
        {
            SoundManager.Instance.PlayCharge();
            isWalking = false;
            isRunning = true;
            _animator.Play("Run");
            _xBotMovement.canMove = true;
            _xBotMovement.SetRunSpeed();
            _xBotMovement.PlayRunSound();
        }
    }

    private void Attack()
    {
        if (isDead)
        {
            return;
        }
        
        var random = Random.Range(0, 2);
        var randomDefence = Random.Range(0, 10);

        if (randomDefence == 0 && _xBotEye.enemyLogic._isAttacking && !_xBotEye.enemyLogic.isDead)
        {
            isDefending = true;
            _animator.Play("Defend");
            // Get the length of the animation clip
            var stateInfo = _animator.GetCurrentAnimatorClipInfo(0);
            var animationClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            animationLength = animationClip.averageDuration;
            // Wait for the animation to finish playing
            defenceCounter++;
            // reduce health.
            // yield return new WaitForSeconds(animationLength);
            if (!_xBotEye.enemyLogic.isDead && !isDead)
                _xBotEye.enemyHealth.ReduceHealth(0);
        }
        else if(!_xBotEye.enemyLogic.isDead)
        {
            _animator.Play("Combat" + random);
            // Get the length of the animation clip
            var animationClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            animationLength = animationClip.averageDuration;
            // Wait for the animation to finish playing
            attackCounter++;
            // SoundManager.Instance.PlaySwordSound(_audioSource);
            // reduce health.
            // yield return new WaitForSeconds(animationLength);
            var damageVal = (int)Mathf.Clamp(animationLength, 1, 2);
            Debug.Log("Damage val is " + damageVal);
            if (!_xBotEye.enemyLogic.isDead && !isDead)
                _xBotEye.enemyHealth.ReduceHealth(damageVal);
        }

        // if (!_xBotEye.enemyLogic && !isDead)
        //     Invoke("Attack", animationLength);
        if (animationLength < 2f)
        {
            animationLength = 2f;
        }
        Invoke("WaitForAnimationEnd", animationLength + 1f);
    }
    private void WaitForAnimationEnd()
    {
        _isAttacking = false;
        isDefending = false;
        isWalking = false;
        isRunning = true;
    }

    private IEnumerator DamageOnCastle()
    {
        //damage on castle
        isDead = true;
        SceneEnemiesManager.Instance.enemiesKilledInWave++;
        Destroy(transform.root.gameObject);
        yield return null;
    }
}