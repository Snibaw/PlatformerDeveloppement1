using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowButtonsHelp : MonoBehaviour
{
    [SerializeField] private GameObject explanationParent;
    [SerializeField] private TMP_Text selectText;
    private bool explanationActive = false;
    // Start is called before the first frame update
    void Start()
    {
        selectText.text = "Show Buttons Help";
        explanationParent.SetActive(false);
   }

    public void ShowHideExplanation()
    {
        if(!PlayerPrefs.HasKey("HasPressedHelpOnce"))
            PlayerPrefs.SetInt("HasPressedHelpOnce", 1);
        explanationActive = !explanationActive;
        explanationParent.SetActive(explanationActive);
        selectText.text = explanationActive ? "Hide Buttons Help" : "Show Buttons Help";
    }
}
