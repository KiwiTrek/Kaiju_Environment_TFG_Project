
using Cinemachine;
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
	public float sprintMult = 2;
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
	public CinemachineFreeLook playerCam;
	public CharacterController controller;
	public Collider swordCollider;
	public CharacterLives health;
	public Animator animator;
	public Transform groundChecker;
	public LayerMask groundMask;
	public LayerMask groundMask2;

	[Space]
	public Collider trunk = null;
    public DataCompilator compilator;

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
	public int currentJumpCount;
	bool jumpPressed;
	int jumpHash;
	Vector3 lastP;
	float timeWithoutAttacking;

	int isHurtHardHash;
	int isMovingHash;
	bool sprintPressed;
	int isSprintingHash;
	int attackStartHash;

	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();

		isHurtHardHash = Animator.StringToHash("isHurtHard");
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

		bool isHurtHard = animator.GetBool(isHurtHardHash);
		if (!health.dead || !isHurtHard)
		{
			InputMagnitude();

            //Physically move player
            PlayerMoveAndRotation();
        }

		isGrounded = (Physics.CheckSphere(groundChecker.position, groundDistance, groundMask) || Physics.CheckSphere(groundChecker.position, groundDistance, groundMask2));
        animator.SetBool("grounded", isGrounded);
        if (isGrounded & verticalMov.y < 0)
        {
			verticalMov.y = -1.0f;
			currentJumpCount = maxJumpCount;
        }

		bool jump = animator.GetBool(jumpHash);
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
		}
		else
        {
			sprintPressed = false;
		}

		if (Input.GetButtonDown("Jump") && currentJumpCount != 0)
		{
			if (compilator != null)
			{
				compilator.RegisterCharacterJump();
			}
			Debug.Log("Jump!");
			currentJumpCount--;
			jumpPressed = true;
		}

		timeWithoutAttacking += Time.deltaTime;
		if (timeWithoutAttacking > 2.0f)
		{
			timeWithoutAttacking = 0;
			numberClicks = 0;
            animator.SetBool("attackStart", false);
            animator.SetInteger("hitCount", numberClicks);
			canAttack = true;
        }

		if (Input.GetButtonDown("Fire1"))
		{
            if (compilator != null)
            {
                compilator.RegisterAttack();
            }
            timeWithoutAttacking = 0;
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
	public void CanAttackToFalse()
    {
		canAttack = false;
    }
	public void ActivateCollider()
	{
		swordCollider.enabled = true;
	}
    public void DeactivateCollider()
    {
        swordCollider.enabled = false;
    }
    public void AttackStarted()
    {
		bool attackStart = animator.GetBool(attackStartHash);
		if (attackStart)
		{
			animator.SetBool("attackStart", false);
		}
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
                InputX *= sprintMult;
                InputZ *= sprintMult;
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

			if (playerCam.enabled) desiredMoveDirection = forward;
        }
		
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);

		//Position correction for 2D area (Extremely hardcoded)
		if (trunk != null)
		{
			if (Vector3.Distance(transform.position, trunk.ClosestPoint(transform.position)) <= 0.44f)
			{
				Vector3 direction = transform.position - trunk.ClosestPoint(transform.position);
				controller.Move(direction);
			}
		}
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }
}
