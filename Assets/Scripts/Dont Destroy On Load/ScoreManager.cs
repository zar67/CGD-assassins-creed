using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int m_score = 0;
    int m_highScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    //Score Functions
    public void IncreaseScore(int _value)
    {
        m_score += _value;
        m_highScore = (m_score > m_highScore) ? m_score : m_highScore;
    }
    public void DecreseScore(int _value){m_score -= _value;}
    public void ResetScore(){m_score = 0;}
}
