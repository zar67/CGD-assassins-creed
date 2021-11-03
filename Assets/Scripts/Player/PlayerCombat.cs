using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject m_swordAnim;

    Rigidbody2D m_rig2D;
    PlayerData m_playerData;
    PlayerMovement m_playerMovement;

    const float m_DAMAGE_TIMER = 2.0f; // cant take damage once taken damage for x amount of time
    float m_currentDamageTime = 0.0f;
    bool m_canTakeDamage = true;
    float m_flashTime = 0.2f;
    float m_currentFlashTime = 0.0f;

    enum CombatState
    {
        ctIDLE = 0,
        ctSNEAK_ATTACK = 1, //when you are behind an enemey
        ctJUMP_ATTACK = 2, //when jumping on eneny
    }
    CombatState m_combatState = CombatState.ctIDLE;

    const float m_SNEAK_ATTACK_RANGE = 2.0f;
    const uint m_SCORE_INCREMENT = 25;
    const float m_SNEAK_ANIM_TIMER = 0.5f; // cant take damage once taken damage for x amount of time
    float m_currentSneakTimer = 0.0f;

    private void Start()
    {
        m_rig2D = gameObject.GetComponent<Rigidbody2D>();
        m_playerData = gameObject.GetComponent<PlayerData>();
        m_playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_combatState)
        {
            case CombatState.ctIDLE:
                {
                    GameObject enemyToAttack = TestSneakAttackRange();
                    if (enemyToAttack != null && TryKillEnemy(enemyToAttack))
                    {
                        m_combatState = CombatState.ctSNEAK_ATTACK;
                        ScoreManager.IncreaseScore();
                    }

                    if (m_canTakeDamage == false)
                    {
                        FlashPlayer();
                    }
                }
                break;
            case CombatState.ctSNEAK_ATTACK:
                {
                    if (m_currentSneakTimer < m_SNEAK_ANIM_TIMER)
                    {
                        m_currentSneakTimer += Time.deltaTime;
                    }
                    else
                    {
                        m_combatState = CombatState.ctIDLE;
                        m_currentSneakTimer = 0;
                    }
                }
                break;
        }
    }

    public void PlayerHit(float _damage)
    {
        if (m_canTakeDamage)
        {
            m_currentDamageTime = 0.0f;
            m_canTakeDamage = false;
            m_currentFlashTime = 0.0f;
            m_playerData.DamageTaken(_damage);
        }
    }

    void FlashPlayer()
    {
        m_currentFlashTime += Time.deltaTime;
        m_currentDamageTime += Time.deltaTime;

        if (m_currentFlashTime < m_flashTime)
            return;
        if (m_currentDamageTime > m_DAMAGE_TIMER)
        {
            m_canTakeDamage = true;
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
            return;
        }

        m_currentFlashTime = 0.0f;
        Color playerCol = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        if (playerCol.g == 1.0f)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
        }
        else
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    GameObject TestSneakAttackRange()
    {
        // cant do sneak attack if falling
        if (m_rig2D.velocity.y < 0)
            return null;

        Collider2D[] allColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, m_SNEAK_ATTACK_RANGE);
        foreach (var col in allColliders)
        {
            if (col.gameObject.tag == "Enemy" && col.gameObject.GetComponent<AIMovement>().IsKillable() && CheckAIDirection(col.gameObject.GetComponent<AIMovement>()))
            {
                FindObjectOfType<SoundManager>().Play("enemy_death");
                return col.gameObject;
            }
        }
        return null;
    }

    // return true if AI is looking away from player
    bool CheckAIDirection(AIMovement _aiMovement)
    {
        bool aiMovingLeft = _aiMovement.IsMovingLeft();
        if (!_aiMovement.HasSeenPlayer() && ((aiMovingLeft && m_rig2D.velocity.x < -0.1f) || (!aiMovingLeft && m_rig2D.velocity.x > 0.1f)))
            return true;

        return false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        string name = collider.gameObject.name;
        AIMovement aiMovement = collider.gameObject.transform.parent.gameObject.GetComponent<AIMovement>();
        //if hit enemy and velocity.y = -1 then you jumped on it
        if (name == "DeathCollider" && m_rig2D.velocity.y < 0.0f && !aiMovement.HasSeenPlayer())
        {
            if (TryKillEnemy(aiMovement.gameObject))
            {
                ScoreManager.IncreaseScore();
            }
        }
        else if (name == "DeathCollider" && m_combatState != CombatState.ctSNEAK_ATTACK && m_playerMovement.GetInsideHayBale() == false)
        {
            PlayerHit(20);
        }
    }

    // only kills enemy if enemy is killable
    bool TryKillEnemy(GameObject enemy)
    {
        AIMovement aiMovement = enemy.GetComponent<AIMovement>();

        if (aiMovement == null || !enemy.GetComponent<AIMovement>().IsKillable())
        {
            return false;
        }

        if (!aiMovement.DeathHandler.IsDying)
        {
            aiMovement.DeathHandler.KillEnemy();
            m_swordAnim.SetActive(false);
            m_swordAnim.SetActive(true);
            StartCoroutine(WaitForSwordFinish());
        }
        
        return true;
    }

    IEnumerator WaitForSwordFinish()
    {
        yield return new WaitForSeconds(0.4f);
        m_swordAnim.SetActive(false);
    }
}