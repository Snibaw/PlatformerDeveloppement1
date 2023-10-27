using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTextBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("HasPressedHelpOnce"))
        {
            GetComponent<Animator>().SetTrigger("Show");
        }
    }
}
