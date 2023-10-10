using Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public int id = -1;
    public CharacterMov player;
    public Light light2D;

    public GameObject pressSpaceUI;
    public static int SizeId = Shader.PropertyToID("_Size");
    [SerializeField] private Material[] materialsToCull = null;
    [SerializeField] private float size = 1.0f;

    [Header("CameraList")]
    public CinemachineFreeLook playerCam;
    public CinemachineVirtualCamera levelCam;
    public CinemachineVirtualCamera legCam;
    public CinemachineVirtualCamera bossCam;

    private void Start()
    {
        if (playerCam.enabled)
        {
            id = 0;
        }
        else if (levelCam.enabled)
        {
            id = 1;
        }
        else if (legCam.enabled)
        { 
            id = 2;
        }
        else { id = 3; }

        if (player != null)
        {
            player.currentCameraId = id;
        }
    }
    private void Update()
    {
        if (light2D != null) { light2D.enabled = false; }
        foreach (var m in materialsToCull)
        {
            m.SetFloat(SizeId, 0.0f);
        }
        switch (id)
        {
            case 0: //Normal
                {
                    if (playerCam != null) { playerCam.enabled = true; }
                    if (levelCam != null) { levelCam.enabled = false; }
                    if (legCam != null) { legCam.enabled = false; }
                    if (bossCam != null) { bossCam.enabled = false; }
                    break;
                }
            case 1: //Level
                {
                    if (playerCam != null) { playerCam.enabled = false; }
                    if (levelCam != null) { levelCam.enabled = true; }
                    if (legCam != null) { legCam.enabled = false; }
                    if (bossCam != null) { bossCam.enabled = false; }
                    if (light2D != null) { light2D.enabled = true; }
                    if (pressSpaceUI != null) { pressSpaceUI.SetActive(false); }
                    break;
                }
            case 2: //Leg
                {
                    if (playerCam != null) { playerCam.enabled = false; }
                    if (levelCam != null) { levelCam.enabled = false; }
                    if (legCam != null) { legCam.enabled = true; }
                    if (bossCam != null) { bossCam.enabled = false; }
                    if (pressSpaceUI != null) { pressSpaceUI.SetActive(false); }
                    break;
                }
            case 3: //Boss
                {
                    if (playerCam != null) { playerCam.enabled = false; }
                    if (levelCam != null) { levelCam.enabled = false; }
                    if (legCam != null) { legCam.enabled = false; }
                    if (bossCam != null) { bossCam.enabled = true; }
                    if (GameplayDirector.cutsceneMode == CutsceneType.None)
                    {
                        foreach (var m in materialsToCull)
                        {
                            m.SetFloat(SizeId, size);
                        }
                    }
                    break;
                }
            default: //Normal
                {
                    if (playerCam != null) { playerCam.enabled = true; }
                    if (levelCam != null) { levelCam.enabled = false; }
                    if (legCam != null) { legCam.enabled = false; }
                    if (bossCam != null) { bossCam.enabled = false; }
                    break;
                }
        }

        if (player != null)
        {
            player.currentCameraId = id;
        }
    }

}
