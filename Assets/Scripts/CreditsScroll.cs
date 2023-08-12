using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float duration = 0.0f;
    [SerializeField] private float finalPositionY = 4000.0f;
    private float currentTimer = 0.0f;
    private Vector3 oldPosition;

    void Start()
    {
        Time.timeScale = 1.0f;
        oldPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer >= 3.0f)
        {
            transform.position = Vector3.Lerp(oldPosition, new Vector3(oldPosition.x, oldPosition.y + finalPositionY, oldPosition.z), (currentTimer - 3.0f) / duration);
        }
    }
}
