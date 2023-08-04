using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUi;
    public bool pause = false;
    [SerializeField] private AudioMixer mixer = null;
    void Start()
    {
        pauseMenuUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
        }

        if (pause)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    public void Pause()
    {
        pauseMenuUi.SetActive(true);
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SwitchMaster(float value)
    {
        Debug.Log("Switching Master to " + value);
        mixer.SetFloat("masterVolume", value);
    }
    public void SwitchMusic(float value)
    {
        Debug.Log("Switching music to " + value);
        mixer.SetFloat("musicVolume", value);
    }
    public void SwitchSFX(float value)
    {
        Debug.Log("Switching SFX to " + value);
        mixer.SetFloat("sfxVolume", value);
    }
}
