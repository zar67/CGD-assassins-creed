using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public const float GROUNDED_RADIUS = 0.05f;
	public const float CEILING_RADIUS = 0.05f;

	[Header("References")]
	[SerializeField] private Rigidbody2D m_rigidbody = default;
	[SerializeField] private Transform m_groundCheck = default;
	[SerializeField] private Transform m_ceilingCheck = default;
	[SerializeField] private Collider2D m_crouchDisableCollider = default;

	[Header("Movement Configuration Values")]
	[SerializeField] private float m_runSpeed = 400f;
	[Range(0, 0.3f)] [SerializeField] private float m_movementSmoothingAmount = 0.05f;

	[Header("Jumping Configuration Values")]
	[SerializeField] private float m_jumpForce = 400f;
	[SerializeField] private bool m_canAirControl = false;
	[SerializeField] private float m_fallMultiplier = 2.5f;
	[SerializeField] private float m_lowJumpMultiplier = 2f;
	[SerializeField] private float m_lastGroundedDelay;
	[SerializeField] private LayerMask m_whatIsGround;

	[Header("Crouching Configuration Values")]
	[Range(0, 1)] [SerializeField] private float m_crouchSpeedMultiplier = 0.36f;

	private bool m_isGrounded = false;
	private bool m_isFacingRight = true;
	private Vector3 m_velocity = Vector3.zero;

	private float m_movementInput = 0;
	private bool m_jumpInput = false;
	private bool m_crouchingInput = false;

	private bool m_wasCrouching = false;
	private float m_lastTimeGrounded = 0;

	public void HandleJump(InputAction.CallbackContext context)
	{
		m_jumpInput = context.ReadValue<float>() > 0;
	}

	public void HandleCrouch(InputAction.CallbackContext context)
	{
		m_crouchingInput = context.ReadValue<float>() > 0.8f;
	}

	public void HandleClimb(InputAction.CallbackContext context)
	{

	}

	public void HandleMovement(InputAction.CallbackContext context)
	{
		m_movementInput = context.ReadValue<float>();
	}
	private void FixedUpdate()
	{
		bool wasGrounded = m_isGrounded;
		m_isGrounded = false;

		// Ground Check
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_groundCheck.position, GROUNDED_RADIUS, m_whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_isGrounded = true;
			}
		}
	}

	private void Update()
	{
		float moveDir = m_movementInput * m_runSpeed * Time.fixedDeltaTime;

		Move(moveDir, m_crouchingInput, m_jumpInput);
	}

	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
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
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
				}

				move *= m_crouchSpeedMultiplier;

				if (m_crouchDisableCollider != null)
				{
					m_crouchDisableCollider.enabled = false;
				}
			}
			else
			{
				if (m_crouchDisableCollider != null)
				{
					m_crouchDisableCollider.enabled = true;
				}

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
				}
			}

			Vector3 targetVelocity = new Vector2(move * 10f, m_rigidbody.velocity.y);
			m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_velocity, m_movementSmoothingAmount);

			// Check if we are facing the right direction
			if ((move > 0 && !m_isFacingRight) ||
				(move < 0 && m_isFacingRight))
			{
				Flip();
			}
		}

		if (m_isGrounded && jump)
		{
			m_isGrounded = false;
			m_rigidbody.AddForce(new Vector2(0f, m_jumpForce));
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
