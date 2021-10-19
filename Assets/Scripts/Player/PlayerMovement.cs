using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public const float GROUNDED_RADIUS = 0.05f;
	public const float CEILING_RADIUS = 0.05f;
	public const float WALL_GRABBING_RADIUS = 0.05f;

	[Header("References")]
	[SerializeField] private Rigidbody2D m_rigidbody = default;
	[SerializeField] private Animator m_animator = default;
	[SerializeField] private Transform m_groundCheck = default;
	[SerializeField] private Transform m_ceilingCheck = default;
	[SerializeField] private Transform m_wallCheck = default;
	[SerializeField] private Collider2D m_crouchCollider = default;

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

	private bool m_canControl = true;

	private bool m_isGrounded = false;
	private bool m_isFacingRight = true;
	private Vector3 m_velocity = Vector3.zero;

	private float m_movementInput = 0;
	private bool m_jumpInput = false;
	private bool m_jumpInputPressedThisFrame = false;
	private bool m_crouchingInput = false;

	private bool m_wasCrouching = false;

	private bool m_canGrabWall = false;
	private bool m_isGrabbingWall = false;

	private float m_wallJumpTimer = 0;

	private float m_defaultGravityScale;

    private void Awake()
    {
		m_defaultGravityScale = m_rigidbody.gravityScale;
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

	}

	public void HandleMovementInput(InputAction.CallbackContext context)
	{
		m_movementInput = context.ReadValue<float>();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_isGrounded;

		// Ground Check
		m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, GROUNDED_RADIUS, m_whatIsGround);
		
		// Wall Grabbing Check
		m_canGrabWall = Physics2D.OverlapCircle(m_wallCheck.position, WALL_GRABBING_RADIUS, m_whatIsWall);
	}

	private void Update()
	{
		if (m_canControl)
		{
			float moveDir = m_movementInput * m_runSpeed * Time.deltaTime;

			UpdateCrouching(ref moveDir);
			UpdateWallJumping(ref moveDir);
			UpdateJumping(ref moveDir);

			UpdateMovement(moveDir);
		}
		else
		{
			m_wallJumpTimer -= Time.deltaTime;
			if (m_wallJumpTimer <= 0)
			{
				m_wallJumpTimer = 0;
				m_canControl = true;
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
	}

	private void UpdateAnimator()
	{
		m_animator.SetFloat("Speed", Mathf.Abs(m_rigidbody.velocity.x));
		m_animator.SetBool("Jump", !m_isGrounded);
	}

	private void UpdateCrouching(ref float movement)
	{
		bool crouch = m_crouchingInput;

		// Crouch Check
		if (!m_crouchingInput)
		{
			if (Physics2D.OverlapCircle(m_ceilingCheck.position, CEILING_RADIUS, m_whatIsGround))
			{
				crouch = true;
			}
		}

		if (m_isGrounded || m_canAirControl)
		{
			if (crouch)
			{
				m_wasCrouching = true;
				movement *= m_crouchSpeedMultiplier;
				m_crouchCollider.enabled = false;
			}
			else
			{
				m_crouchCollider.enabled = true;
				m_wasCrouching = false;
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
			}
		}

		if (m_isGrabbingWall)
		{
			m_rigidbody.gravityScale = m_defaultGravityScale * m_wallGrabbingGravityMultiplier;
			m_rigidbody.velocity = Vector2.zero;

			if (m_jumpInput && m_jumpInputPressedThisFrame)
			{
				m_wallJumpTimer = m_wallJumpDuration;
				m_rigidbody.AddForce(new Vector2(-(m_movementInput * m_jumpForce), m_jumpForce * m_wallJumpHeightMultiplier));
				Flip();

				m_rigidbody.gravityScale = m_defaultGravityScale;
				m_isGrabbingWall = false;
				m_canControl = false;
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
			m_isGrounded = false;
			m_rigidbody.AddForce(new Vector2(0f, m_jumpForce));
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
			Vector3 targetVelocity = new Vector2(move * 10f, m_rigidbody.velocity.y);
			m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_velocity, m_movementSmoothingAmount);
		}
	}

	private void Flip()
	{
		m_isFacingRight = !m_isFacingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}