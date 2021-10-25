using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject m_swordAnim;
    Rigidbody2D m_rig2D;
    PlayerData m_playerData; 

    const float m_DAMAGE_TIMER = 2.0f; // cant take damage once taken damage for x amount of time
    float m_currentDamageTime = 0.0f;
    bool m_canTakeDamage = true;
    enum CombatState 
    {
        ctIDLE = 0,
        ctSNEAK_ATTACK = 1, //when you are behind an enemey
        ctJUMP_ATTACK = 2, //when jumping on eneny
        ctATTACKING = 3
    }
    CombatState m_combatState = CombatState.ctIDLE;

    const float m_SNEAK_ATTACK_RANGE = 15.0f;
    const uint m_SCORE_INCREMENT = 25;
    const float m_SNEAK_ANIM_TIMER = 0.6f; // cant take damage once taken damage for x amount of time
    float m_currentSneakTimer = 0.0f;

    int a = 0;
	private void Start()
	{
		m_rig2D = gameObject.GetComponent<Rigidbody2D>();
        m_playerData = gameObject.GetComponent<PlayerData>();
	}

	// Update is called once per frame
	void Update()
    {
        switch(m_combatState)
        {
            case CombatState.ctIDLE:
            {
                GameObject enemyToAttack = TestSneakAttackRange();
                if(a == 0)//if(enemyToAttack != null)
                {
                    m_combatState = CombatState.ctSNEAK_ATTACK;
                    m_swordAnim.SetActive(true);
                    a++;
		        }
			}break;
            case CombatState.ctJUMP_ATTACK:
            {
                
			}break;
            case CombatState.ctSNEAK_ATTACK:
            {
                if (m_currentSneakTimer < m_SNEAK_ANIM_TIMER)
                {
                    m_currentSneakTimer += Time.deltaTime;
                }
                else
                {
                    m_swordAnim.SetActive(false);
                    m_combatState = CombatState.ctIDLE;
		        }
			}break;
            case CombatState.ctATTACKING:
            {
                
			}break;
        }
    }

    IEnumerator DamageTakenTimer()
    {
        while(m_currentDamageTime < m_DAMAGE_TIMER)
        {
            m_currentDamageTime += Time.deltaTime;
		}
        m_canTakeDamage = true;
        yield return new WaitForSeconds(0.1f);
	}

    IEnumerator SneakAttackAnimtimer()
    {
        print("START ANIM");
        if (m_currentSneakTimer < m_SNEAK_ANIM_TIMER)
        {
            m_currentSneakTimer += Time.deltaTime;
            m_combatState = CombatState.ctIDLE;
        }
        else
        {
            m_swordAnim.SetActive(false);
		}
        print("END ANIM");
        
        yield return new WaitForSeconds(0.1f);
	}

    public void PlayerHit(float _damage)
    {
        if(m_canTakeDamage)
        {
            m_currentDamageTime = 0.0f;
            m_canTakeDamage = false;
            m_playerData.DamageTaken(_damage);
            StartCoroutine(DamageTakenTimer());
		}
	}

	GameObject TestSneakAttackRange()
	{ 
       /*foreach(GameObject enemy in allEnemies)//@@@ need to get reference of all enemies
       {
            bool inRange = Vector2.Distance(enemy.transform.position, gameObject.transform.position) < m_SNEAK_ATTACK_RANGE;
            bool enemyFacingOtherWay = enemy.GetComponent<Rigidbody2D>().velocity != m_rig2D.velocity;
            if(inRange && enemyFacingOtherWay)
            {
                return enemy;
			}
	   }*/
       return null;
    }
    void AnimateSneakAttack()
    {

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
        //if hit enemy and velocity.y = -1 then you jumped on it
		if(collision.gameObject.tag == "Enemy" && m_rig2D.velocity.y < 0.0f)
        {
            
		}
	}
}
