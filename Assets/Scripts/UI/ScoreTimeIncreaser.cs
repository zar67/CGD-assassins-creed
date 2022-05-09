using UnityEngine;

public class ScoreTimeIncreaser : MonoBehaviour
{
    [SerializeField] private int m_scoreIncrease;
    [SerializeField] private float m_timeDelay = 1.0f;

    private float m_timer = 0.0f;

    void Update()
    {
        m_timer += Time.deltaTime;

        while (m_timer > m_timeDelay)
        {
            m_timer -= m_timeDelay;
            ScoreManager.IncreaseScore(m_scoreIncrease);
        }
    }
}
