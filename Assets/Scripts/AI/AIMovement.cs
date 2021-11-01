using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIMovement : MonoBehaviour
{
    #region Difficulty enum and structs

    [Serializable] enum DifficultyParameter
    {
        MOVEMENT_SPEED,
        MOVEMENT_SPEED_PLAYER_FOUND,
        TIME_BFR_RUSHING_PLAYER,
        VISION_DISTANCE,
        TIME_BFR_FLIPPING,
    }

    [Serializable] struct ValueEvolution
    {
        public DifficultyParameter parameter;
        public int maxDifficultyValue;
        public float minValue;
        public float maxValue;

        public float GetCurrentValue(int currentDifficulty)
        {
            float d = (float)currentDifficulty / maxDifficultyValue;
            Debug.Log("C: " + currentDifficulty.ToString() + " MD: " + maxDifficultyValue.ToString() + " D: " + d.ToString());

            if (d > 1) d = 1;

            return minValue + ((maxValue - minValue) * d);
        }
    }

    [Serializable] struct DifficultyEvolution
    {
        public List<ValueEvolution> difficultyEvolutions;

        public float GetValueOfParameter(int d, DifficultyParameter p)
        {
            foreach (var v in difficultyEvolutions)
            {
                if (v.parameter == p)
                {
                    return v.GetCurrentValue(d);
                }
            }

            Debug.LogError("COULD NOT FIND PARAMETER: " + p);
            return 0;
        }
    }

    #endregion

    #region Variables

    [Header("Difficulty")]
    [SerializeField] private DifficultyEvolution difficultyEvolution;
    [SerializeField] private bool killable = true;
    [SerializeField] private Color killableColor = default; 
    [SerializeField] private Color unkillableColor = default;

    [Header("Patrol")]
    [SerializeField] private bool patrols = true;
    private float timeBeforeFlipping = 2.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rigidbody = default;
    [SerializeField] private Animator m_animator = default;
    [SerializeField] private AIDeathHandler m_deathHandler = default;
    [SerializeField] private AIViewHandler viewHandlerBottom = default;
    [SerializeField] private AIViewHandler viewHandlerTop = default;
    [SerializeField] private SpriteRenderer spr = default;
    [SerializeField] private GameObject fndMark = default;
    [SerializeField] private FloorCheck floorCheck = default;
    [SerializeField] private SpriteRenderer sprite = default;

    [Header("Movement")]
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothingAmount = 0.05f;
    private float movementSpeed = 200f;
    private float timeBfrRushingPlayer = 0.3f;
    private float playerFoundSpeed = 400f;
    [SerializeField] private float m_jumpForce = 150f;

    [Header("AI Vision")]
    [SerializeField] private float stopDistanceWall = 3.0f;
    [SerializeField] private float stopDistancePlatform = 1.0f;
    private float visionPlayer = 5.5f;

    private int difficulty = 0;

    private Vector3 m_velocity = Vector3.zero;

    private bool movingLeft = false;
    private bool needsFlipping = true;
    private bool moving = false;

    private float timeBeforeFlippingRemaining = 0f;

    private bool playerFound = false;
    private bool playerDir = false;
    private float timeBeforeRushRemaining = 0;

    private bool jumped = false;

    [Header("Debug")]
    public bool USEDEBUGVALUES = false;
    public bool _VISIONVISIBLE = true;
    public int _DIFF = 0;
    public bool _KILLABLE = false;

    #endregion

    public bool IsMovingLeft(){return movingLeft;}
    public bool HasSeenPlayer() { return playerFound; }
    public bool IsKillable() { return killable; }

    public AIDeathHandler DeathHandler => m_deathHandler;

    #region Difficulty

    public void SetKillable(bool _killable)
    {
        killable = _killable;
        sprite.color = (killable ? killableColor : unkillableColor);
    }

    public void SetVisionVisibility(bool visible)
    {
        viewHandlerBottom.SetVisionVisibility(visible);
        viewHandlerTop.SetVisionVisibility(visible);
    }

    public void SetDifficulty(int _diff)
    {
        difficulty = _diff;
        SetDifficultyParameters();
    }

    public void IncreaseDifficulty()
    {
        difficulty++;
        SetDifficultyParameters();
    }

    void SetDifficultyParameters()
    {
        movementSpeed = difficultyEvolution.GetValueOfParameter(difficulty, DifficultyParameter.MOVEMENT_SPEED);
        playerFoundSpeed = difficultyEvolution.GetValueOfParameter(difficulty, DifficultyParameter.MOVEMENT_SPEED_PLAYER_FOUND);
        timeBfrRushingPlayer = difficultyEvolution.GetValueOfParameter(difficulty, DifficultyParameter.TIME_BFR_RUSHING_PLAYER);
        timeBeforeFlipping = difficultyEvolution.GetValueOfParameter(difficulty, DifficultyParameter.TIME_BFR_FLIPPING);
        visionPlayer = difficultyEvolution.GetValueOfParameter(difficulty, DifficultyParameter.VISION_DISTANCE);

        timeBeforeFlippingRemaining = timeBeforeFlipping;
        timeBeforeRushRemaining = timeBfrRushingPlayer;

        viewHandlerBottom.SetUpHandler(stopDistanceWall, stopDistancePlatform, visionPlayer);
        viewHandlerTop.SetUpHandler(stopDistanceWall, stopDistancePlatform, visionPlayer);

        Debug.Log(visionPlayer);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (USEDEBUGVALUES)
        {
            difficulty = _DIFF;
            SetVisionVisibility(_VISIONVISIBLE);
            killable = _KILLABLE;
        }
        SetDifficultyParameters();
        SetKillable(killable);
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

        UpdateAnimator();
    }

    public void ResetOnDeath()
    {
        m_rigidbody.velocity = Vector2.zero;
        fndMark.SetActive(false);
        enabled = false;
    }

    private void UpdateAnimator()
    {
        m_animator.SetFloat("Speed", Mathf.Abs(m_rigidbody.velocity.x));
        m_animator.SetBool("Jump", jumped);
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

        UpdateMovement((movingLeft ? -1 : 1) * playerFoundSpeed * Time.deltaTime);

        //if (!viewHandlerBottom.HasFoundPlayer() && viewHandlerTop.HasFoundPlayer())
        //{
        //    if (floorCheck.TouchingFloor() && !jumped)
        //    {
        //        jumped = true;
        //        m_rigidbody.constraints = RigidbodyConstraints2D.None;
        //        m_rigidbody.AddForce(new Vector2((movingLeft ? -50f : 50f), m_jumpForce * 10f));
        //    }
        //}
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
        playerDir = _playerDir;
        if (!playerFound) FoundPlayerVisuals();

        playerFound = true;
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

    #region Floor Check

    public void TouchedFloor()
    {
        m_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        
        float y = transform.position.y % 1.0f;
        transform.position = new Vector3(transform.position.x, (transform.position.y - y) + (y < 0.5f ? 0.02f : 1.02f), transform.position.z);
    }

    public void LeftFloor()
    {
        m_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        jumped = false;
    }

    #endregion 
}
