using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneManager : MonoBehaviour
{
	[SerializeField] private string homeSceneName;
	[SerializeField] private GameObject skipButton;
	[SerializeField] private float idleDuration = 1.0f;
	[SerializeField] private float idleTimer = 0.0f;
	[SerializeField] private Vector2 mousePosition = Vector2.zero;
	[SerializeField] private RectTransform credits;
	[SerializeField] private float finalPositionY = 2900.0f;

	private void OnEnable()
	{
		Cursor.visible = false;
		skipButton.SetActive(false);
		mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}

	private void Update()
	{
		if((mousePosition.x - Input.GetAxis("Mouse X")) != 0 || (mousePosition.y - Input.GetAxis("Mouse Y")) != 0)
		{
			mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			idleTimer = idleDuration;
			//turn on
			Cursor.visible = true;
			skipButton.SetActive(true);
		}
		else
			idleTimer -= Time.deltaTime;

		if(idleTimer < 0.0f)
		{
			//turn off
			Cursor.visible = false;
			skipButton.SetActive(false);
		}

		if (credits.position.y >= (540.0f + finalPositionY))
		{
			GoToHomeScene();
		}
	}

	public void GoToHomeScene()
	{
		SceneManager.LoadScene(homeSceneName, LoadSceneMode.Single);
	}
}