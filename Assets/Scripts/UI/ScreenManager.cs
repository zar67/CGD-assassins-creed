using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private TextMeshProUGUI m_highScoreText;

    // Start is called before the first frame update
    void Awake()
    {
        if(m_playButton == null || m_quitButton == null || m_highScoreText == null)
        {
            print("ERROR : ScreenManager() -> null objects");
		}
        else
        {
            m_highScoreText.text = ScoreManager.HighScore().ToString();

            if (m_playButton)
            {
                m_playButton.onClick.AddListener(delegate
                {
                    OnPlay();
                });
            }
            else
            {
                print("ERROR : ScreenManager() -> null playbutton");
            }

            if (m_quitButton)
            {
                m_quitButton.onClick.AddListener(delegate
                {
                    OnQuit();
                });
            }
            else
            {
                print("ERROR : ScreenManager() -> null m_quitBttn");
            }
		}
    }

    void OnPlay()
    {
        SceneControl.LoadPlay();
	}

    void OnQuit()
    {
        Application.Quit();
	}
}
