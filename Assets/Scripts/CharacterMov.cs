
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class CharacterMov : MonoBehaviour {

    public float velocity;
	public float sprintMult;
	public int maxJumpCount;
	public float jumpHeight;
	public float gravity;
	int currentJumpCount;
	bool jump;
	
    [Space]

	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;
	public bool isGrounded;
	public Transform groundChecker;
	public LayerMask groundMask;
	public float groundDistance;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    private Vector3 verticalMov;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		InputMagnitude ();

		isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask);
        if (isGrounded & verticalMov.y < 0)
        {
			verticalMov.y = -1.0f;
			currentJumpCount = maxJumpCount;
        }
		
		if (jump)
        {
			jump = false;
			verticalMov.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

		verticalMov.y += gravity * Time.deltaTime;
        
		controller.Move(verticalMov * Time.deltaTime);
    }

    void PlayerMoveAndRotation() {
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		if (Input.GetButton("Sprint"))
		{
			InputX *= sprintMult;
			InputZ *= sprintMult;
		}

		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * velocity);
		}
	}

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

	void InputMagnitude() {
		//Calculate Input Vectors
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude

		speed = new Vector2(InputX, InputZ).sqrMagnitude;

		//Physically move player
		if (speed > allowPlayerRotation) {
			anim.SetFloat ("Blend", speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation ();
		} else if (speed < allowPlayerRotation) {
			anim.SetFloat ("Blend", speed, StopAnimTime, Time.deltaTime);
		}

		if (Input.GetButtonDown("Jump") && currentJumpCount != 0)
        {
			currentJumpCount--;
			jump = true;
        }
	}
}
