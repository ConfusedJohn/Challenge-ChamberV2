using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    /*
    [SerializeField]
    private InputManager input;
    [SerializeField]
    private Canvas MainMenuPanel;
    [SerializeField]
    private Canvas PlayPanel;
    [SerializeField]
    private Canvas OptionsPanel;
    [SerializeField]
    private Canvas Tutorial1;
    [SerializeField]
    private Canvas Tutorial2;
    [SerializeField]
    private Canvas Tutorial3;
    [SerializeField]
    private Canvas Tutorial4;
    */
    [SerializeField]
    private AudioMixer myMixer;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider SFXSlider;
    [SerializeField]
    private Toggle Fullscreen;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume")) 
        {
            LoadMusicVolume();
        }
        else 
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            LoadFullScreen();
        }
        else
        {
            SetFullscreen();
        }

    }

    public void SelectLevel(String level)
    {
        SceneManager.LoadScene(level);
    }
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Update is called once per frame
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music",Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
    private void LoadSFXVolume()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetSFXVolume();
    }

    public void SetFullscreen()
    {
        bool fullscreen = false;
        if (PlayerPrefs.GetFloat("Fullscreen") == 1)
        {
            fullscreen = true;
        }

        if (fullscreen)
        {
            //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetFloat("Fullscreen", 1);
        }
        else 
        {
            //Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetFloat("Fullscreen", 0);
        }
       
    }
    public void SetFullscreen(bool is_Fulllscreen)
    {
        if (is_Fulllscreen)
        {
            Screen.fullScreen = is_Fulllscreen;
            PlayerPrefs.SetFloat("Fullscreen", 1);
        } else 
        {
            Screen.fullScreen = is_Fulllscreen;
            PlayerPrefs.SetFloat("Fullscreen", 0);
        }

    }

    private void LoadFullScreen()
    {
        if (PlayerPrefs.GetFloat("Fullscreen")==1)
        {
            Fullscreen.isOn = true;
        }
        else
        {
            Fullscreen.isOn = false;
        }
        SetFullscreen();
    }
}
