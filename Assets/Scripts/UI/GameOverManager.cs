using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_scoreTxt;
    [SerializeField] TextMeshProUGUI m_highScoreTxt;
    [SerializeField] Button m_playAgainBttn;
    [SerializeField] Button m_quitBttn;

    // Start is called before the first frame update
    void Awake()
    {
        if(m_scoreTxt)
            m_scoreTxt.text = ScoreManager.Score().ToString();
        if(m_highScoreTxt)
            m_highScoreTxt.text = ScoreManager.HighScore().ToString();
        if(m_playAgainBttn)
            m_playAgainBttn.onClick.AddListener(delegate{PlayAgain();});
        if(m_quitBttn)
            m_quitBttn.onClick.AddListener(delegate{SceneControl.LoadMainMenu();});
    }

    void PlayAgain()
    {
        ScoreManager.ResetScore();
        SceneControl.LoadPlay();
	}
}
