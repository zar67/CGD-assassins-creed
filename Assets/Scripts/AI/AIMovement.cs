using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private bool patrols = true;
    [SerializeField] private float timeBeforeFlipping = 2.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rigidbody = default;
    [SerializeField] private AIViewHandler viewHandlerBottom = default;
    [SerializeField] private AIViewHandler viewHandlerTop = default;
    [SerializeField] private SpriteRenderer spr = default;
    [SerializeField] private GameObject fndMark = default;

    [Header("Movement")]
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothingAmount = 0.05f;
    [SerializeField] private float movementSpeed = 200f;
    [SerializeField] private float timeBfrRushingPlayer = 0.3f;
    [SerializeField] private float playerFoundSpeed = 400f;

    private Vector3 m_velocity = Vector3.zero;

    private bool movingLeft = false;
    private bool needsFlipping = true;
    private bool moving = false;

    private float timeBeforeFlippingRemaining = 0f;

    private bool playerFound = false;
    private bool playerDir = false;
    private float timeBeforeRushRemaining = 0;

    public bool IsMovingLeft(){return movingLeft;}
    public bool HasSeenPlayer() { return playerFound; }

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeFlippingRemaining = timeBeforeFlipping;
        timeBeforeRushRemaining = timeBfrRushingPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFound)
        {
            MovementWhenFoundPlayer();
        }
        else
        {
            NormalMovement();
        }
        
    }

    #region Movement Region

    void MovementWhenFoundPlayer()
    {
        if (timeBeforeRushRemaining > 0)
        {
            timeBeforeRushRemaining -= Time.deltaTime;
            return;
        }

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

    private void UpdateMovement(float move)
    {
        Vector3 targetVelocity = new Vector2(move * 10f, m_rigidbody.velocity.y);
        m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_velocity, movementSmoothingAmount);
    }

    #endregion

    #region Flipping AI

    void FlipAi()
    {
        movingLeft = !movingLeft;
        needsFlipping = false;

        viewHandlerBottom.FlipView(movingLeft);
        viewHandlerTop.FlipView(movingLeft);
        spr.flipX = movingLeft;
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

    #endregion

    #region Player Interation

    public void FoundPlayer(bool _playerDir)
    {
        playerFound = true;
        playerDir = _playerDir;
        FoundPlayerVisuals();
    }

    void FoundPlayerVisuals()
    {
        m_rigidbody.velocity = Vector3.zero;

        fndMark.SetActive(true);
    }

    public void LostPlayer()
    {
        playerFound = false;
        timeBeforeRushRemaining = timeBfrRushingPlayer;

        fndMark.SetActive(false);
    }

    #endregion 

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            m_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

            float y = transform.position.y % 1.0f;

            transform.position = new Vector3(transform.position.x, (transform.position.y - y) + (y < 0.5f ? 0.02f : 1.02f), transform.position.z);
        }
    }
}
