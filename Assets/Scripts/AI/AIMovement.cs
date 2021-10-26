using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool patrols = true;
    public float timeBeforeFlipping = 2.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rigidbody = default;
    [SerializeField] private AIViewHandler viewHandlerBottom = default;
    [SerializeField] private AIViewHandler viewHandlerTop = default;
    [SerializeField] private SpriteRenderer spr = default;

    [Header("Movement")]
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothingAmount = 0.05f;
    public float movementSpeed = 200f;
    public float playerFoundSpeed = 400f;

    private Vector3 m_velocity = Vector3.zero;

    private bool movingLeft = false;
    private bool needsFlipping = true;
    private bool moving = false;

    private float timeBeforeFlippingRemaining = 0f;

    private bool playerFound = false;
    private bool playerDir = false;

    public bool IsMovingLeft(){return movingLeft;}

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeFlippingRemaining = timeBeforeFlipping;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFound)
        {
            Debug.Log("PLAYER FOUND");
            MovementWhenFoundPlayer();
        }
        else
        {
            NormalMovement();
        }
        
    }

    void MovementWhenFoundPlayer()
    {
        if (playerDir != movingLeft)
        {
            FlipAi();
        }

        if (moving)
        {
            UpdateMovement((movingLeft ? -1 : 1) * playerFoundSpeed * Time.deltaTime);
        }
    }

    void NormalMovement()
    {
        if (moving)
        {
            if (!patrols)
            {
                StopAndSetUpFlip();
            }
            else
            {
                UpdateMovement((movingLeft ? -1 : 1) * movementSpeed * Time.deltaTime);
            }
        }
        else
        {
            timeBeforeFlippingRemaining -= Time.deltaTime;
            if (timeBeforeFlippingRemaining <= 0)
            {
                moving = true;
                if (needsFlipping)
                {
                    FlipAi();
                }
            }
        }
    }

    void FlipAi()
    {
        movingLeft = !movingLeft;
        needsFlipping = false;

        viewHandlerBottom.FlipView(movingLeft);
        viewHandlerTop.FlipView(movingLeft);
        spr.flipX = movingLeft;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            m_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

            float y = transform.position.y % 1.0f;

            transform.position = new Vector3(transform.position.x, (transform.position.y - y) + (y < 0.5f ? 0.02f : 1.02f), transform.position.z);
        }
    }

    private void UpdateMovement(float move)
    {
        Vector3 targetVelocity = new Vector2(move * 10f, m_rigidbody.velocity.y);
        m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_velocity, movementSmoothingAmount);
    }

    public void ReachedEndOfFloor(bool left_side)
    {
        if (left_side != movingLeft || !patrols) return;

        StopAndSetUpFlip();
    }

    public void ReachedWall(bool left_side)
    {
        if (left_side != movingLeft || !patrols) return;
        StopAndSetUpFlip();
    }

    void StopAndSetUpFlip()
    {
        needsFlipping = true;
        moving = false;
        timeBeforeFlippingRemaining = timeBeforeFlipping;
        m_rigidbody.velocity = Vector3.zero;
    }

    public void FoundPlayer(bool _playerDir)
    {
        playerFound = true;
        playerDir = _playerDir;
    }

    public void LostPlayer()
    {
        playerFound = false;
    }
}
