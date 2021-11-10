using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    static int m_score = 0;
    static int m_highScore = 0;
    static int m_diffculty = 1;
    const int m_FIRST_DIFF_INTERVAL = 30;
    const int m_CHANGE_DIFFICULTY_INTERVAL = 50;
    static int m_diffcultyScore = 0;
    
    const int m_SCORE_INSCREASE = 10;
    const int m_SCORE_DECREASE = 1;

    // if true then update text on screen
    static bool m_dirtyScore = false;

    //Score Functions
    public static void IncreaseScore()
    {
        m_score += m_SCORE_INSCREASE;
        m_highScore = (m_score > m_highScore) ? m_score : m_highScore;

        //check diffculty level
        m_diffcultyScore += m_SCORE_INSCREASE;
        if(m_diffculty == 1)
            m_diffculty = (m_diffcultyScore % m_FIRST_DIFF_INTERVAL == 0) ? m_diffculty + 1 : m_diffculty;
        else
            m_diffculty = (m_diffcultyScore % m_CHANGE_DIFFICULTY_INTERVAL == 0) ? m_diffculty + 1: m_diffculty;

        Debug.Log("Diffculty : " + m_diffculty.ToString());
        SetScoreDirty(true);
    }

    public static void DecreseScore()
    {
        m_score -= m_SCORE_DECREASE;
        SetScoreDirty(true);
    }

    public static void ResetScore()
    {
        m_score = 0;
        m_diffculty = 1;
        SetScoreDirty(true);
    }

    public static int Score() {return m_score;}
    public static int HighScore(){return m_highScore;}
    public static int Diffculty(){ return m_diffculty;}
    public static void SetScoreDirty(bool _value){m_dirtyScore = _value;}
    public static bool IsScoreDirty(){return m_dirtyScore;}
}