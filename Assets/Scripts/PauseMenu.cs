using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUi;
    public bool pause = false;
    [SerializeField] private AudioMixer mixer = null;
    [SerializeField] private Toggle mouseX;
    [SerializeField] private Toggle mouseY;
    [SerializeField] private Slider mouseSensitivity;
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider sfxVol;

    void Start()
    {
        pauseMenuUi.SetActive(false);

        mouseX.isOn = SettingsHolder.instance.invertedCameraX;
        mouseX.onValueChanged.Invoke(mouseX.isOn);

        mouseY.isOn = SettingsHolder.instance.invertedCameraY;
        mouseY.onValueChanged.Invoke(mouseY.isOn);

        mouseSensitivity.value = SettingsHolder.instance.mouseSensitivity;
        mouseSensitivity.onValueChanged.Invoke(mouseSensitivity.value);

        masterVol.value = SettingsHolder.instance.masterVol;
        masterVol.onValueChanged.Invoke(masterVol.value);

        musicVol.value = SettingsHolder.instance.musicVol;
        musicVol.onValueChanged.Invoke(musicVol.value);

        sfxVol.value = SettingsHolder.instance.sfxVol;
        sfxVol.onValueChanged.Invoke(sfxVol.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "destroyed_city") return;

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

    public void SwitchReverseCamX(bool value)
    {
        SettingsHolder.instance.invertedCameraX = value;
    }

    public void SwitchReverseCamY(bool value)
    {
        SettingsHolder.instance.invertedCameraY = value;
    }

    public void SwitchMouseSensitivity(float value)
    {
        SettingsHolder.instance.mouseSensitivity = value;
    }
    public void SwitchMaster(float value)
    {
        SettingsHolder.instance.masterVol = value;
        Debug.Log("Switching Master to " + value);
        mixer.SetFloat("masterVolume", value);
    }
    public void SwitchMusic(float value)
    {
        SettingsHolder.instance.musicVol = value;
        Debug.Log("Switching music to " + value);
        mixer.SetFloat("musicVolume", value);
    }
    public void SwitchSFX(float value)
    {
        SettingsHolder.instance.sfxVol = value;
        Debug.Log("Switching SFX to " + value);
        mixer.SetFloat("sfxVolume", value);
    }
    public void ReturnToMenu()
    {
        AudioListener.pause = false;
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }
    public void QuitApp()
    {
        Debug.Log("Quitting App!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
