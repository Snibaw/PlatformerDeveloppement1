using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundEffects;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string soundEffectName, float volume = 1f)
    {
        foreach(AudioClip soundEffect in soundEffects)
        {
            if(soundEffect.name == soundEffectName)
            {
                audioSource.PlayOneShot(soundEffect, volume);
            }
        }
    }
}
