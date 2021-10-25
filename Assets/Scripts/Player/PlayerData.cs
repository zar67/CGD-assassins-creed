using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    const float m_START_HEALTH = 100.0f;
    float m_health = 0.0f;
    
    enum PlayerState 
    {
        psALIVE = 0,
        psDEAD = 1
	}PlayerState m_playerState = PlayerState.psALIVE;

    void Start()
    {
        m_health = m_START_HEALTH;
    }


    void Update()
    {
        
    }

    

    //Health Functions
    public void DamageTaken(float _damage)
    {
        m_health -= _damage;
        if(IsDead())
            SetPlayerDead();
    }
    public void ResetHealth(){m_health= m_START_HEALTH;}
    public void SetPlayerDead()
    {
        m_playerState = PlayerState.psDEAD;
        SceneControl.LoadGameOver();
    }
    private void SetPlayerAlive(){m_playerState = PlayerState.psALIVE;}
    private bool IsDead(){return m_health <= 0.0f;}
    

}
