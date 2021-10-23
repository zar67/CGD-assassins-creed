using UnityEngine;

public class ClimbablePlatform : MonoBehaviour
{
    [SerializeField] private PlatformEffector2D m_platformEffector = default;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_platformEffector.surfaceArc = 0;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_platformEffector.surfaceArc = 180;
    }
}