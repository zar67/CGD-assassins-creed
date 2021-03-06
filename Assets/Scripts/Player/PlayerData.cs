using System.Collections;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private Animator m_characterAnimator;
    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private TMPro.TextMeshProUGUI m_scoreTxt;

    const float m_START_HEALTH = 1.0f;
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

	private void Update()
	{
		if(ScoreManager.IsScoreDirty())
        {
            ScoreManager.SetScoreDirty(false);
            m_scoreTxt.text = ScoreManager.Score().ToString();
		}
	}
	//Health Functions
	public void DamageTaken(float _damage)
    {
        m_health -= _damage;

        FindObjectOfType<SoundManager>().Play("hit_hurt");
        if (IsDead())
            SetPlayerDead();
    }
    public void ResetHealth(){m_health= m_START_HEALTH;}
    public void SetPlayerDead()
    {
        m_playerState = PlayerState.psDEAD;

        FindObjectOfType<SoundManager>().Play("player_death");
        m_characterAnimator.SetTrigger("Death");
        m_playerMovement.HandleCharacterDeath();
        Camera.main.GetComponent<PlayerCamera>().EnableTutorialCamera();
        StartCoroutine(WaitForDeathAnim());
    }

    private void SetPlayerAlive(){m_playerState = PlayerState.psALIVE;}

    private bool IsDead(){return m_health <= 0.0f;}

    private IEnumerator WaitForDeathAnim()
    {
        yield return new WaitForSeconds(2.0f);
        SceneControl.LoadGameOver();
    }
}