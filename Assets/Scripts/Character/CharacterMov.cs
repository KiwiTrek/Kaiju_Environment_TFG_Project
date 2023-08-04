
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundType
{
	Jump,
	Land,
	GruntSwing,
	Sword,
	GruntSwingBig,
	Hurt,
	Thud,
	Death,
	GetUp
}
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
	public AudioSource mov;
	public AudioSource grunt;
	public AudioSource swing;
	public GameObject[] slashes;
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
	[Header("Sound Clip")]
    public AudioClip[] swordSFX;
    public AudioClip[] jumpsSFX;
    public AudioClip[] hurtSFX;
    public AudioClip[] gruntSwingSFX;
	public AudioClip[] getUpSFX;
	public AudioClip gruntSwingBigSFX;
    public AudioClip deathSFX;
    public AudioClip thudSFX;

    [Space]
    public DataCompilator compilator;

	[Space]
	[Header("Debug values")]
	public int currentCameraId;
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
            PlaySound(SoundType.Jump);
            verticalMov.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
			canAttack = true;
            numberClicks = 0;
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

        if (Input.GetButtonDown("Fire1"))
        {
            if (compilator != null)
            {
                compilator.RegisterAttack();
            }
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
	public void InvertCamX()
	{
		playerCam.m_XAxis.m_InvertInput = !playerCam.m_XAxis.m_InvertInput;
	}
    public void InvertCamY()
    {
        playerCam.m_XAxis.m_InvertInput = !playerCam.m_XAxis.m_InvertInput;
    }
	public void SwitchSensitivity(float sensitivity)
	{
		playerCam.m_XAxis.m_MaxSpeed = 2.0f * sensitivity;
		playerCam.m_YAxis.m_MaxSpeed = sensitivity / 33;
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
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }

	public void SlashActivate(int i)
	{
		slashes[i].SetActive(true);
	}

	public void ResetSlashes()
	{
		foreach(var slash in slashes)
		{
			slash.SetActive(false);
		}
	}

	public void PlaySound(SoundType type)
	{
		mov.pitch = Random.Range(0.90f, 1.1f);
		grunt.pitch = Random.Range(0.95f, 1.05f);
		swing.pitch = Random.Range(0.9f, 1.1f);
		switch (type)
		{
            case SoundType.Jump:
                {
                    mov.clip = jumpsSFX[Random.Range(0, jumpsSFX.Length)];
					mov.Play();
                    break;
                }
            case SoundType.Land:
                {
                    grunt.clip = hurtSFX[Random.Range(1, hurtSFX.Length)];
                    grunt.Play();
                    break;
                }
            case SoundType.GruntSwing:
                {
                    grunt.clip = gruntSwingSFX[Random.Range(0, gruntSwingSFX.Length)];
					grunt.Play();
                    break;
                }
            case SoundType.Sword:
                {
                    swing.clip = swordSFX[Random.Range(0, swordSFX.Length)];
					swing.Play();
                    break;
                }
            case SoundType.GruntSwingBig:
                {
					swing.clip = gruntSwingBigSFX;
					swing.Play();
                    break;
                }
            case SoundType.Hurt:
                {
                    grunt.clip = hurtSFX[Random.Range(0, hurtSFX.Length)];
					grunt.Play();
                    break;
                }
            case SoundType.Thud:
                {
                    mov.clip = thudSFX;
                    mov.pitch = Random.Range(0.85f, 1.00f);
					mov.Play();
                    break;
                }
            case SoundType.Death:
                {
                    grunt.clip = deathSFX;
					grunt.Play();
                    break;
                }
			case SoundType.GetUp:
				{
					grunt.clip = getUpSFX[Random.Range(0, getUpSFX.Length)];
					grunt.pitch = Random.Range(0.8f, 1.0f);
					grunt.Play();
					break;
				}
            default:
				break;
		}
	}
}
