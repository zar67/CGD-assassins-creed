using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlidingCheck : MonoBehaviour
{
    [SerializeField] private LayerMask m_whatIsWall;

    [SerializeField] private bool m_isCollidingWithWall;

    public bool CollidingWithWall()
    {
        return m_isCollidingWithWall;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_whatIsWall == (m_whatIsWall | (1 << collision.gameObject.layer)))
        {
            m_isCollidingWithWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_whatIsWall == (m_whatIsWall | (1 << collision.gameObject.layer)))
        {
            m_isCollidingWithWall = false;
        }
    }
}
