using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTimer : MonoBehaviour
{
    private SoundEffectManager soundEffectManager;
    [SerializeField] private ParticleSystem endParticle;
    private TimerManager timerManager;
    private bool hasPlayerFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        soundEffectManager = GameObject.Find("SoundEffectManager").GetComponent<SoundEffectManager>();
        timerManager = GameObject.Find("TimerManager").GetComponent<TimerManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(hasPlayerFinished) return;
            hasPlayerFinished = true;
            endParticle.Play();
            if(PlayerPrefs.GetInt("isSoundEffectsEnabled") == 1 ? true: false) soundEffectManager.PlaySoundEffect("Confetti",0.5f);
            timerManager.EndTimer(true);
        }
    }
}
