using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool patrols = true;
    public float timeBeforeFlipping = 2.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rigidbody = default;
    [SerializeField] private AIViewHandler viewHandler = default;
    [SerializeField] private SpriteRenderer spr = default;

    [Header("Movement")]
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothingAmount = 0.05f;
    public float movementSpeed = 200f;

    private Vector3 m_velocity = Vector3.zero;

    private bool movingLeft = false;
    private bool needsFlipping = true;
    private bool moving = false;

    private float timeBeforeFlippingRemaining = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeFlippingRemaining = timeBeforeFlipping;
    }

    // Update is called once per frame
    void Update()
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

        viewHandler.FlipView(movingLeft);
        spr.flipX = movingLeft;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            m_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        }

        //if (col.gameObject.tag == "Player")
        //{
        //    Destroy(gameObject);
        //}
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
}
