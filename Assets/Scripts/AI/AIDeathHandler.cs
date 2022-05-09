using System.Collections;
using UnityEngine;

public class AIDeathHandler : MonoBehaviour
{
    [SerializeField] private AIMovement m_aiMovement = default;
    [SerializeField] private AIViewHandler m_topViewHandler = default;
    [SerializeField] private AIViewHandler m_bottomViewHandler = default;
    [SerializeField] private Animator m_animator = default;

    public bool IsDying = false;

    public void KillEnemy()
    {
        IsDying = true;
        m_animator.SetTrigger("Death");
        m_aiMovement.ResetOnDeath();
        m_topViewHandler.ResetOnDeath();
        m_bottomViewHandler.ResetOnDeath();

        StartCoroutine(WaitForAIDeathAnim());
    }

    private IEnumerator WaitForAIDeathAnim()
    {
        yield return new WaitForSeconds(2.0f);

        Destroy(m_aiMovement.gameObject);
    }
}