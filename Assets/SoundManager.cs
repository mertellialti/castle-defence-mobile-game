using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private List<AudioClip> swordSounds;
    [SerializeField] private List<AudioClip> shieldSounds;
    [SerializeField] private List<AudioClip> deathSounds;
    [SerializeField] private List<AudioClip> hurtSounds;
    [SerializeField] private List<AudioClip> stabSounds;
    [SerializeField] private List<AudioClip> walkSounds;
    [SerializeField] private List<AudioClip> whoopSounds;
    [SerializeField] private List<AudioClip> runSounds;
    [SerializeField] private AudioSource audioSourceCharge;

    private bool isChargeSoundCanPlayed = true;

    public void PlaySwordSound(AudioSource audioSrc)
    {
        var randVal1 = Random.Range(0, 3);
        if (randVal1 == 1)
        {
            var randVal = Random.Range(0, swordSounds.Count - 1);
            audioSrc.clip = swordSounds[randVal];
            audioSrc.Play();
        }
        else
        {
            var randVal = Random.Range(0, stabSounds.Count - 1);
            audioSrc.clip = stabSounds[randVal];
            audioSrc.Play();
        }
    }

    public void PlayShieldSound(AudioSource audioSrc)
    {
        var randVal = Random.Range(0, shieldSounds.Count - 1);
        audioSrc.clip = shieldSounds[randVal];
        audioSrc.Play();
    }

    public void PlayHurtSound(AudioSource audioSrc)
    {
        var randVal = Random.Range(0, hurtSounds.Count - 1);
        audioSrc.clip = hurtSounds[randVal];
        audioSrc.Play();
    }

    public void PlayDeathSound(AudioSource audioSrc)
    {
        var randVal = Random.Range(0, deathSounds.Count - 1);
        audioSrc.clip = deathSounds[randVal];
        audioSrc.Play();
    }

    public void PlayStabSound(AudioSource audioSrc)
    {
        var randVal1 = Random.Range(0, 2);
        if (randVal1 == 1)
        {
            var randVal = Random.Range(0, whoopSounds.Count - 1);
            audioSrc.clip = whoopSounds[randVal];
            audioSrc.Play();
        }
        else
        {
            var randVal = Random.Range(0, stabSounds.Count - 1);
            audioSrc.clip = stabSounds[randVal];
            audioSrc.Play();
        }
    }

    public void PlayWalkSound(AudioSource audioSrc)
    {
        var randVal = Random.Range(0, walkSounds.Count - 1);
        audioSrc.clip = walkSounds[randVal];
        audioSrc.Play();
    }

    public void PlayRunSound(AudioSource audioSrc)
    {
        try
        {
            var randVal = Random.Range(0, runSounds.Count - 1);
            audioSrc.clip = runSounds[randVal];
            audioSrc.Play();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void PlayCharge()
    {
        try
        {
            if (isChargeSoundCanPlayed)
            {
                audioSourceCharge.Play();
                isChargeSoundCanPlayed = false;
                Invoke("SetChargeSoundCanBePlayed", 5f);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void SetChargeSoundCanBePlayed()
    {
        isChargeSoundCanPlayed = true;
        audioSourceCharge.Stop();
    }
}