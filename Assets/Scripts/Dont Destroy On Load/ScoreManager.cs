using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    static int m_score = 0;
    static int m_highScore = 0;


    //Score Functions
    public static void IncreaseScore(int _value)
    {
        m_score += _value;
        m_highScore = (m_score > m_highScore) ? m_score : m_highScore;
    }
    public static void DecreseScore(int _value){m_score -= _value;}
    public static void ResetScore(){m_score = 0;}
    public static int Score() {return m_score;}
    public static int HighScore(){return m_highScore;}
}
