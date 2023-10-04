using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private bool canStart = false;
    private void Start() {
        Invoke("CanStart", 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        if(canStart && Input.anyKeyDown){
            SceneManager.LoadScene("Game");
        }
    }
    public void CanStart(){
        canStart = true;
    }
}
