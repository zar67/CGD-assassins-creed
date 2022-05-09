using UnityEngine;

public class ScoreManager
{
    static int m_score = 0;
    static int m_highScore = 0;
    static int m_diffculty = 1;
    const int m_FIRST_DIFF_INTERVAL = 30;
    const int m_CHANGE_DIFFICULTY_INTERVAL = 50;
    static int m_diffcultyScore = 0;
    
    // if true then update text on screen
    static bool m_dirtyScore = false;

    //Score Functions
    public static void IncreaseScore(int scoreIncrease)
    {
        m_score += scoreIncrease;
        m_highScore = (m_score > m_highScore) ? m_score : m_highScore;

        //check diffculty level
        m_diffcultyScore += scoreIncrease;
        if(m_diffculty == 1)
            m_diffculty = (m_diffcultyScore % m_FIRST_DIFF_INTERVAL == 0) ? m_diffculty + 1 : m_diffculty;
        else
            m_diffculty = (m_diffcultyScore % m_CHANGE_DIFFICULTY_INTERVAL == 0) ? m_diffculty + 1: m_diffculty;

        SetScoreDirty(true);
    }

    public static void DecreseScore(int scoreDecrease)
    {
        m_score -= scoreDecrease;
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