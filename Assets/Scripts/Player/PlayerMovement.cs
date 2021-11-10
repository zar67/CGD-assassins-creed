using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public const float GROUNDED_RADIUS = 0.05f;
	public const float CEILING_RADIUS = 0.5f;
	public const float WALL_GRABBING_RADIUS = 0.05f;

	[Header("References")]
	[SerializeField] private SoundManager m_soundManager = default;
	[SerializeField] private Rigidbody2D m_rigidbody = default;
	[SerializeField] private Animator m_animator = default;

	[Header("Transform Checks")]
	[SerializeField] private Transform m_groundCheck = default;
	[SerializeField] private Transform m_ceilingCheck = default;
	[SerializeField] private Transform m_wallCheck = default;

	[Header("Colliders")]
	[SerializeField] private BoxCollider2D m_mainCollider = default;
	[SerializeField] private Collider2D m_hangingCollider = default;

	[Header("Movement Configuration Values")]
	[SerializeField] private float m_runSpeed = 400f;
	[Range(0, 0.3f)] [SerializeField] private float m_movementSmoothingAmount = 0.05f;

	[Header("Jumping Configuration Values")]
	[SerializeField] private float m_jumpInputThreshold = 0f;
	[SerializeField] private float m_jumpForce = 400f;
	[SerializeField] private bool m_canAirControl = false;
	[SerializeField] private float m_fallMultiplier = 2.5f;
	[SerializeField] private float m_lowJumpMultiplier = 2f;
	[SerializeField] private LayerMask m_whatIsGround;

	[Header("Wall Jumping Configuration Values")]
	[SerializeField] private float m_wallGrabbingGravityMultiplier = 0.7f;
	[SerializeField] private float m_wallJumpDuration = 0.2f;
	[SerializeField] private float m_wallJumpHeightMultiplier = 1.5f;
	[SerializeField] private LayerMask m_whatIsWall;

	[Header("Crouching Configuration Values")]
	[SerializeField] private float m_crouchInputThreshold = 0.8f;
	[Range(0, 1)] [SerializeField] private float m_crouchSpeedMultiplier = 0.36f;

	[Header("Climbing Configuration Values")]
	[SerializeField] private float m_hangingSpeedMultiplier = 0.5f;
	[SerializeField] private LayerMask m_whatIsClimbablePlatforms;

	[Header("Diving Configuration Values")]
	[SerializeField] private float m_diveSpeedMultiplier = 0.1f;
	[SerializeField] private LayerMask m_whatIsDivableArea;

	private bool m_canControl = true;
	private float m_defaultGravityScale;

	private bool m_isGrounded = false;
	private bool m_isFacingRight = true;
	private Vector3 m_velocity = Vector3.zero;

	private float m_movementInput = 0;
	private bool m_jumpInput = false;
	private bool m_jumpInputPressedThisFrame = false;
	private bool m_crouchingInput = false;
	private float m_climbingInput = 0;
	private bool m_climbingPressedThisFrame = false;

	private bool m_forceCrouch = false;

	private bool m_canGrabWall = false;
	private bool m_isGrabbingWall = false;
	private bool m_isWallJumping = false;
	private float m_wallJumpTimer = 0;

	private bool m_isHanging = false;
	private Collider2D m_platformHangingOn = null;
	private bool m_canHangNearby = false;

	private bool m_isDiving = false;
	private bool m_insideHayBale = false;

	public bool GetInsideHayBale() {return m_insideHayBale;}

	public void SetInsideHayBale(bool _value) {m_insideHayBale = _value;}

    private void Awake()
    {
		m_defaultGravityScale = m_rigidbody.gravityScale;
		m_hangingCollider.enabled = false;
	}

	public void HandleJumpInput(InputAction.CallbackContext context)
	{
		m_jumpInput = context.ReadValue<float>() > m_jumpInputThreshold;
		m_jumpInputPressedThisFrame = context.performed;
	}

	public void HandleCrouchInput(InputAction.CallbackContext context)
	{
		m_crouchingInput = context.ReadValue<float>() > m_crouchInputThreshold;
	}

	public void HandleClimbInput(InputAction.CallbackContext context)
	{
		m_climbingInput = context.ReadValue<float>();
		m_climbingPressedThisFrame = context.performed;
	}

	public void HandleMovementInput(InputAction.CallbackContext context)
	{
		m_movementInput = context.ReadValue<float>();
	}

	private void FixedUpdate()
	{
		// Ground Check
		m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, GROUNDED_RADIUS, m_whatIsGround);

		m_isGrounded = m_isGrounded ||
			(Physics2D.OverlapCircle(m_groundCheck.position, GROUNDED_RADIUS, m_whatIsClimbablePlatforms) && m_rigidbody.velocity.y <= 0);

		// Crouch Check
		m_forceCrouch = Physics2D.OverlapCircle(m_ceilingCheck.position, CEILING_RADIUS, m_whatIsGround);

		// Can Hang Check
		m_canHangNearby = Physics2D.OverlapCircle(m_ceilingCheck.position, CEILING_RADIUS, m_whatIsClimbablePlatforms);

		// Wall Grabbing Check
		m_canGrabWall = Physics2D.OverlapCircle(m_wallCheck.position, WALL_GRABBING_RADIUS, m_whatIsWall);

		if (m_canControl)
		{
			float moveDir = m_movementInput * m_runSpeed * Time.deltaTime;

			UpdateCrouching(ref moveDir);
			UpdateWallJumping(ref moveDir);
			UpdateJumping(ref moveDir);
			UpdateClimbing(ref moveDir);
			UpdateDiving(ref moveDir);

			UpdateMovement(moveDir);
		}
		else
		{
			if (m_isWallJumping)
			{
				m_wallJumpTimer -= Time.deltaTime;
				if (m_wallJumpTimer <= 0)
				{
					m_wallJumpTimer = 0;
					m_canControl = true;
					m_isWallJumping = false;
				}
			}

			if (m_isDiving)
			{
				if (m_isGrounded)
				{
					m_isDiving = false;
					m_canControl = true;
					m_rigidbody.gravityScale = m_defaultGravityScale;
				}
			}
		}

		UpdateAnimator();

		// Check if we are facing the right direction
		if ((m_movementInput > 0 && !m_isFacingRight) ||
			(m_movementInput < 0 && m_isFacingRight))
		{
			Flip();
		}

		m_jumpInputPressedThisFrame = false;
		m_climbingPressedThisFrame = false;
	}

	private void UpdateAnimator()
	{
		m_animator.SetFloat("Speed", Mathf.Abs(m_rigidbody.velocity.x));
		m_animator.SetBool("Jump", !m_isGrounded);
		m_animator.SetBool("Crouch", m_crouchingInput || m_forceCrouch || m_insideHayBale);
		m_animator.SetBool("Hang", m_isHanging);
		m_animator.SetBool("GrabbingWall", m_isGrabbingWall);
		m_animator.SetBool("Dive", m_isDiving);
	}

	private void UpdateCrouching(ref float movement)
	{
		bool crouch = m_crouchingInput || m_forceCrouch;

		if (m_isGrounded || m_canAirControl)
		{
			if (crouch)
			{
				movement *= m_crouchSpeedMultiplier;
				m_mainCollider.size = new Vector2(1, 0.9f);
				m_mainCollider.offset = new Vector2(0, -0.55f);
			}
			else
			{
				m_mainCollider.size = new Vector2(1, 1.9f);
				m_mainCollider.offset = new Vector2(0, -0.06f);
			}
		}
	}

	private void UpdateWallJumping(ref float movement)
	{
		bool wasGrabbingWall = m_isGrabbingWall;

		m_isGrabbingWall = false;
		if (m_canGrabWall && !m_isGrounded)
		{
			if ((m_isFacingRight && movement > 0) || (!m_isFacingRight && movement < 0))
			{
				m_isGrabbingWall = true;
				m_soundManager.Play("wall_slide");
			}
		}

		if (m_isGrabbingWall)
		{
			m_rigidbody.gravityScale = m_defaultGravityScale * m_wallGrabbingGravityMultiplier;
			m_rigidbody.velocity = Vector2.zero;

			if (m_jumpInput && m_jumpInputPressedThisFrame)
			{
				m_soundManager.Play("wall_jump");
				m_wallJumpTimer = m_wallJumpDuration;
				m_rigidbody.AddForce(new Vector2(-(m_movementInput * m_jumpForce * 6f), m_jumpForce * m_wallJumpHeightMultiplier));
				Flip();

				m_rigidbody.gravityScale = m_defaultGravityScale;
				m_isGrabbingWall = false;
				m_canControl = false;
				m_isWallJumping = true;
			}
		}
		else
		{
			m_rigidbody.gravityScale = m_defaultGravityScale;
		}
	}

	private void UpdateJumping(ref float movement)
	{
		if (m_jumpInput && m_isGrounded)
		{
			m_soundManager.Play("jump");
			m_isGrounded = false;
			m_rigidbody.AddForce(new Vector2(0f, m_jumpForce * 10f));
		}

		if (m_rigidbody.velocity.y < 0)
		{
			m_rigidbody.velocity += Vector2.up * Physics2D.gravity * (m_fallMultiplier - 1) * Time.deltaTime;
		}
		else if (m_rigidbody.velocity.y > 0 && !m_jumpInput)
		{
			m_rigidbody.velocity += Vector2.up * Physics2D.gravity * (m_lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}

	public void UpdateMovement(float move)
	{
		if (m_isGrounded || m_canAirControl)
		{
			Vector3 targetVelocity = new Vector2(move, m_rigidbody.velocity.y);
			m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_velocity, m_movementSmoothingAmount);
		}
	}

	private void UpdateClimbing(ref float movement)
	{
		if (m_isHanging)
		{
			movement *= m_hangingSpeedMultiplier;
		}

		if (m_climbingInput < 0 && m_climbingPressedThisFrame)
		{
			if (m_isHanging)
			{
				SetCollidersForHanging(false);
				m_platformHangingOn = null;
				m_isHanging = false;
			}
			else if (!m_isHanging && m_isGrounded)
			{
				StartHanging(m_groundCheck.position, GROUNDED_RADIUS);
			}
			else if (m_canHangNearby && !m_isGrounded)
			{
				StartHanging(m_ceilingCheck.position, CEILING_RADIUS);
			}
		}
		else if (m_climbingInput > 0 && m_climbingPressedThisFrame)
		{
			if (m_isHanging)
			{
				SetCollidersForHanging(false);
				m_platformHangingOn = null;
				m_isHanging = false;
				m_rigidbody.AddForce(new Vector2(0f, m_jumpForce * 12));
			}
		}
	}

	private void StartHanging(Vector3 colliderCheckPosition, float colliderCheckRadius)
	{
		// Check For Climbable Platform
		Collider2D[] colliders = Physics2D.OverlapCircleAll(colliderCheckPosition, colliderCheckRadius, m_whatIsClimbablePlatforms);

		if (colliders.Length > 0)
		{
			m_platformHangingOn = colliders[0];
			SetCollidersForHanging(true);
			m_isHanging = true;

			m_soundManager.Play("hang");
		}
	}

	private void SetCollidersForHanging(bool hanging)
	{
		Physics2D.IgnoreCollision(m_mainCollider, m_platformHangingOn, hanging);
		m_hangingCollider.enabled = hanging;
	}

	private void Flip()
	{
		m_isFacingRight = !m_isFacingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void UpdateDiving(ref float movement)
	{
		if (!m_isDiving)
		{
			// Check For Divable area
			Collider2D[] colliders = Physics2D.OverlapCircleAll(m_groundCheck.position, CEILING_RADIUS, m_whatIsDivableArea);

			if (colliders.Length > 0)
			{
				m_isDiving = true;

				m_rigidbody.gravityScale = m_defaultGravityScale * m_diveSpeedMultiplier;
				m_rigidbody.velocity = new Vector2(0.0f, 0.0f);
				m_canControl = false;
			}
		}
	}
}