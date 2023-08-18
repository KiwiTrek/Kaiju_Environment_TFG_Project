using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float duration = 0.0f;
    [SerializeField] private float finalPositionY = 4000.0f;
    private float currentTimer = 0.0f;
    private Vector2 oldPosition;

    void Start()
    {
        Time.timeScale = 1.0f;
        oldPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer >= 3.0f)
        {
            transform.localPosition = Vector3.Lerp(oldPosition, new Vector3(oldPosition.x, oldPosition.y + finalPositionY), (currentTimer - 3.0f) / duration);
        }
        else
        {
            transform.localPosition = oldPosition;
        }
    }
}
