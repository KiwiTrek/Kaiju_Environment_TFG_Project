
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class CharacterMov : MonoBehaviour {

	[Header("Values")]
	[Range(0.0f, 30.0f)]
    public float velocity = 10.0f;
	[Range(1, 5)]
	public int sprintMult = 2;
	[Range(1, 10)]
	public int maxJumpCount = 2;
	[Range(0.0f, 30.0f)]
	public float jumpHeight = 15.0f;
	[Range(-30.0f, 30.0f)]
	public float gravity = 15.0f;
	[Range(0.0f, 0.5f)]
	public float groundDistance = 0.5f;

	[Space]
	[Header("Components")]
	public Camera cam;
	public CharacterController controller;
	public CharacterLives health;
	public Animator animator;
	public Transform groundChecker;
	public LayerMask groundMask;

	[Space]
	[Header("Debug values")]
	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public float desiredRotationSpeed = 0.1f;
	public bool isGrounded;
	public bool canAttack = true;
	public int numberClicks = 0;

    private Vector3 verticalMov;
	int currentJumpCount;
	bool jumpPressed;
	int jumpHash;
	Vector3 lastP;

	int isMovingHash;
	bool sprintPressed;
	int isSprintingHash;
	int attackStartHash;

	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();

		isMovingHash = Animator.StringToHash("isMoving");
		isSprintingHash = Animator.StringToHash("isSprinting");
		jumpHash = Animator.StringToHash("jump");
		attackStartHash = Animator.StringToHash("attackStart");

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update ()
	{
		lastP = transform.position;
		if (numberClicks == 0)
        {
			canAttack = true;
        }

		if (!health.dead)
		{
			InputMagnitude();

            //Physically move player
            PlayerMoveAndRotation();
        }

		bool jump = animator.GetBool(jumpHash);

		isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask);
		animator.SetBool("grounded", isGrounded);
        if (isGrounded & verticalMov.y < 0)
        {
			verticalMov.y = -1.0f;
			currentJumpCount = maxJumpCount;
        }

		animator.SetBool("jump", jumpPressed);
		if (jumpPressed)
        {
			jumpPressed = false;
			verticalMov.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

		verticalMov.y += gravity * Time.deltaTime;
		controller.Move(verticalMov * Time.deltaTime);
		float verticalVelocity = (lastP.y - transform.position.y) / Time.deltaTime;
		animator.SetFloat("jumpVelocity", verticalVelocity);
	}

	void InputMagnitude()
	{
		//Calculate Input Vectors
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		if (Input.GetButton("Sprint"))
		{
			sprintPressed = true;
			InputX *= sprintMult;
			InputZ *= sprintMult;
		}
		else
        {
			sprintPressed = false;
		}

		if (Input.GetButtonDown("Jump") && currentJumpCount != 0)
		{
			Debug.Log("Jump!");
			currentJumpCount--;
			jumpPressed = true;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			if (canAttack && numberClicks < 3)
			{
				numberClicks++;
				if (numberClicks == 1)
                {
					animator.SetBool("attackStart", true);
					animator.SetInteger("hitCount", numberClicks);
                }
				canAttack = false;
			}
        }
	}

	//For animation
	public void VerifyCombo()
    {
		if (canAttack)
		{
			numberClicks = 0;
			canAttack = false;
			animator.SetInteger("hitCount", numberClicks);
		}
		else
		{
			if (numberClicks > 1) animator.SetInteger("hitCount", numberClicks);
		}
	}
	public void CanAttackToTrue()
    {
		canAttack = true;
    }
	public void AttackStarted()
    {
		bool attackStart = animator.GetBool(attackStartHash);
		if (attackStart)
		{
			animator.SetBool("attackStart", false);
		}
	}
	public void CanAttackToFalse()
    {
		canAttack = false;
    }

	public void RotateToCamera(Transform t)
	{
		var forward = cam.transform.forward;
		desiredMoveDirection = forward;
		t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
	}
	void PlayerMoveAndRotation()
	{
		bool isMoving = animator.GetBool(isMovingHash);
		bool isSprinting = animator.GetBool(isSprintingHash);

		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		if (InputX != 0.0f || InputZ != 0.0f)
        {
			if (!isMoving) animator.SetBool("isMoving", true);

			if (sprintPressed)
			{
				if (!isSprinting) animator.SetBool("isSprinting", true);
			}
			else
            {
				if (isSprinting) animator.SetBool("isSprinting", false);
            }
			desiredMoveDirection = forward * InputZ + right * InputX;
			controller.Move(desiredMoveDirection * Time.deltaTime * velocity);
		}
		else
        {
			if (isMoving) animator.SetBool("isMoving", false);
			if (isSprinting) animator.SetBool("isSprinting", false);

			desiredMoveDirection = forward;
        }
		
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
	}
}
