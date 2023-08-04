using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleCollectibleScript : MonoBehaviour
{
	public bool rotate = true;
	public float rotationSpeed = 1.0f;

	public AudioSource audioSource = null;
	public GameObject pickupVFX = null;
	public GameObject mesh = null;

	bool picked = false;
	float timer = 0.0f;

	// Use this for initialization
	void Start ()
	{}
	
	// Update is called once per frame
	void Update ()
	{
		if (rotate) transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
		if (picked)
		{
			timer += Time.deltaTime;
			if (timer > 0.75f)
			{
				gameObject.SetActive (false);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") Collect (other.gameObject.GetComponent<CharacterLives>());
	}

	public void Collect(CharacterLives t)
	{
		t.lives++;
		t.livesUI.CreateHeart(1);

		//Effects
		audioSource.pitch = Random.Range(0.95f, 1.1f);
		audioSource.Play();

		GameObject vfx = Instantiate(pickupVFX, this.transform);
		Destroy(vfx, 0.5f);

		mesh.SetActive(false);
		picked = true;
	}

	public void Restart()
	{
		gameObject.SetActive(true);
		mesh.SetActive(true);
		picked = false;
		timer = 0.0f;
	}
}
