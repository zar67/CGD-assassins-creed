using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    [SerializeField] private PlayerCombat m_playerCombat = default;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_playerCombat.PlayerHit(1000);
    }
}
