using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public int id = -1;
    public CharacterMov player;

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
        switch (id)
        {
            case 0:
                if (playerCam != null) { playerCam.enabled = true; }
                if (levelCam != null) { levelCam.enabled = false; }
                if (legCam != null) { legCam.enabled = false; }
                if (bossCam != null) { bossCam.enabled = false; }
                break;
            case 1:
                if (playerCam != null) { playerCam.enabled = false; }
                if (levelCam != null) { levelCam.enabled = true; }
                if (legCam != null) { legCam.enabled= false; }
                if (bossCam != null) { bossCam.enabled = false; }
                break;
            case 2:
                if (playerCam != null) { playerCam.enabled = false; }
                if (levelCam != null) { levelCam.enabled = false; }
                if (legCam != null) { legCam.enabled = true; }
                if (bossCam != null) { bossCam.enabled = false; }
                break;
            case 3:
                if (playerCam != null) { playerCam.enabled = false; }
                if (levelCam != null) { levelCam.enabled = false; }
                if (legCam != null) { legCam.enabled = false; }
                if (bossCam != null) { bossCam.enabled = true; }
                break;
            default:
                if (playerCam != null) { playerCam.enabled = true; }
                if (levelCam != null) { levelCam.enabled = false; }
                if (legCam != null) { legCam.enabled = false; }
                if (bossCam != null) { bossCam.enabled = false; }
                break;
        }

        if (player != null)
        {
            player.currentCameraId = id;
        }
    }

}
