using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleMenu : MonoBehaviour
{
    public static ToggleMenu instance;
    [SerializeField] private TMP_Text[] toggleTexts;
    bool isMenuOpen = false;
    Color baseColor;
    public bool isDashEffectEnabled, isSpriteMovementEnabled, isParticlesEnabled, isLineRendererEnabled, isSoundEffectsEnabled;

    private void Awake()
    {
        instance = this;

        InitBooleans();
        InitTextColors();
    }
    private void Start()
    {
        baseColor = GetComponent<Image>().color;
    }

    public void OpenCloseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        isMenuOpen = !isMenuOpen;
        GetComponent<Animator>().SetTrigger(isMenuOpen ? "Open" : "Close");
        GetComponent<Image>().color = isMenuOpen ? new Color(0,0,0,1f) : baseColor;
    }
    public void ToggleParameter(int i)
    {
        EventSystem.current.SetSelectedGameObject(null);
        toggleTexts[i].color = toggleTexts[i].color == Color.white ? Color.grey : Color.white;

        switch(i)
        {
            case 0:
                isDashEffectEnabled = !isDashEffectEnabled;
                PlayerPrefs.SetInt("isDashEffectEnabled", isDashEffectEnabled ? 1 : 0);
                break;
            case 1:
                isSpriteMovementEnabled = !isSpriteMovementEnabled;
                PlayerPrefs.SetInt("isSpriteMovementEnabled", isSpriteMovementEnabled ? 1 : 0);
                break;
            case 2:
                isParticlesEnabled = !isParticlesEnabled;
                PlayerPrefs.SetInt("isParticlesEnabled", isParticlesEnabled ? 1 : 0);
                break;
            case 3:
                isLineRendererEnabled = !isLineRendererEnabled;
                PlayerPrefs.SetInt("isLineRendererEnabled", isLineRendererEnabled ? 1 : 0);
                break;
            case 4:
                isSoundEffectsEnabled = !isSoundEffectsEnabled;
                PlayerPrefs.SetInt("isSoundEffectsEnabled", isSoundEffectsEnabled ? 1 : 0);
                break;
        }
    }
    private void InitBooleans()
    {
        isDashEffectEnabled = PlayerPrefs.GetInt("isDashEffectEnabled", 1) == 1 ? true : false;
        isSpriteMovementEnabled = PlayerPrefs.GetInt("isSpriteMovementEnabled", 1) == 1 ? true : false;
        isParticlesEnabled = PlayerPrefs.GetInt("isParticlesEnabled", 1) == 1 ? true : false;
        isLineRendererEnabled = PlayerPrefs.GetInt("isLineRendererEnabled", 1) == 1 ? true : false;
        isSoundEffectsEnabled = PlayerPrefs.GetInt("isSoundEffectsEnabled", 1) == 1 ? true : false;
    }
    private void InitTextColors()
    {
        toggleTexts[0].color = isDashEffectEnabled ? Color.white : Color.grey;
        toggleTexts[1].color = isSpriteMovementEnabled ? Color.white : Color.grey;
        toggleTexts[2].color = isParticlesEnabled ? Color.white : Color.grey;
        toggleTexts[3].color = isLineRendererEnabled ? Color.white : Color.grey;
        toggleTexts[4].color = isSoundEffectsEnabled ? Color.white : Color.grey;
    }
}
