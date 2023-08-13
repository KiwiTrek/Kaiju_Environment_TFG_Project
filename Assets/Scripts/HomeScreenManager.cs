using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour
{

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void GoToGame()
    {
        SceneManager.LoadScene("destroyed_city");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToWebsite(string website)
    {
        Application.OpenURL(website);
    }

}
