using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour
{
    public AudioSource musicPlayer;
    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void GoToGame()
    {
        musicPlayer.Stop();
        SceneManager.LoadScene("destroyed_city", LoadSceneMode.Single);
    }
    public void GoToWebsite(string website)
    {
        Application.OpenURL(website);
    }

}
