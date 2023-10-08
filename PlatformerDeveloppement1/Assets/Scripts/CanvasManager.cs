using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject[] pauseMenuButtons;
    [SerializeField] private TMP_Text setTimerText;
    private int timerActive = 1;
    private bool isGamePaused = false;
    public GameObject pauseFirstButton;
    private TimerManager timerManager;
    [SerializeField] private GameObject spawnAnimation;
    [SerializeField] private TMP_Text spawnAnimationText;
    private bool isSpawnAnimationActive = false;
    // Start is called before the first frame update
    void Start()
    {
        timerManager = GameObject.Find("TimerManager").GetComponent<TimerManager>();
        pauseMenu.SetActive(false);
        isSpawnAnimationActive = PlayerPrefs.GetInt("SpawnAnimationActive",1) == 1;
        spawnAnimation.SetActive(isSpawnAnimationActive);
        SetTimerText();
    }
    private void SetTimerText()
    {
        timerActive = PlayerPrefs.GetInt("TimerActive",1);
        setTimerText.text = timerActive == 1 ? "Delete Timer" : "Add Timer";
        timerManager.ShowOrHideTimer();
    }

    public void PlayerPressPauseButton()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            OpenPauseMenu();
        }
    }
    private void OpenPauseMenu()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        pauseMenu.SetActive(true);
        pauseMenu.GetComponent<Animator>().SetTrigger("Open");

        for(int i = 0; i < pauseMenuButtons.Length; i++)
        {
            pauseMenuButtons[i].GetComponent<Animator>().SetTrigger(i%2 == 0 ? "Left" : "Right");
        }

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        StartCoroutine(ClosePauseMenu(0.5f));
    }
    private IEnumerator ClosePauseMenu(float time)
    {
        pauseMenu.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSecondsRealtime(time);
        pauseMenu.SetActive(false);
    }
    public void TimerButton()
    {
        PlayerPrefs.SetInt("TimerActive", 1-timerActive);
        SetTimerText();
    }
    public void MainMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void QuitGameButton()
    {
        Application.Quit();
    }
    public void CloseMenuIfOpen()
    {
        if(isGamePaused)
        {
            ResumeGame();
        }
    }
    public void ShowHideSpawnAnimation()
    {
        isSpawnAnimationActive = !isSpawnAnimationActive;
        PlayerPrefs.SetInt("SpawnAnimationActive", isSpawnAnimationActive ? 1 : 0);
        spawnAnimationText.text = isSpawnAnimationActive ? "Hide Spawn Animation" : "Show Spawn Animation";
    }
}
