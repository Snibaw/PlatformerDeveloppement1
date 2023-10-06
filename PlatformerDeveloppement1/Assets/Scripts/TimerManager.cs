using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bestTimeText;
    private float startTime = 0;
    private int timerActive;
    private bool timerIsRunning = false;

    private void Start()
    {
        ShowOrHideTimer();
    }

    private void Update()
    {
        if(timerIsRunning)
        {
            timeText.text = "Time: " + (Time.time - startTime).ToString("F2");
        }
    }
    public void ResetTimer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ShowOrHideTimer()
    {
        timerActive = PlayerPrefs.GetInt("TimerActive", 1);
        timeText.gameObject.SetActive(timerActive == 1);
        bestTimeText.gameObject.SetActive(timerActive == 1);
        if(timerActive == 1)
        {
            bestTimeText.text = "Best Time: ";
            if(PlayerPrefs.HasKey("BestTime"))
            {
                bestTimeText.text += PlayerPrefs.GetFloat("BestTime").ToString("F2");
            }
            timeText.text = "Time: 0";
            timerIsRunning = false;
        }
        else
        {
            EndTimer();
        }
    }
    public void StartTimer()
    {
        startTime = Time.time;
        timerIsRunning = true;
    }
    public void EndTimer(bool hasFinished = false)
    {
        if(!timerIsRunning) return;

        timerIsRunning = false;
        float time = Time.time - startTime;
        timeText.text = "Time: " + time.ToString("F2");

        if(!hasFinished) return;

        if(!PlayerPrefs.HasKey("BestTime") ||time < PlayerPrefs.GetFloat("BestTime"))
        {
            PlayerPrefs.SetFloat("BestTime", time);
            bestTimeText.text = "Best Time: " + time.ToString("F2");
        }
    }
}
