using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHolder : MonoBehaviour
{
    public static SettingsHolder instance;
    public bool invertedCameraX = false;
    public bool invertedCameraY = false;
    public float mouseSensitivity = 100.0f;
    public float masterVol = 0.0f;
    public float musicVol = -10.0f;
    public float sfxVol = -10.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
