using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] GameObject m_playOBJ;
    [SerializeField] GameObject m_quitOBJ;
    [SerializeField] GameObject m_highScoreOBJ;
    Button m_playBttn;
    Button m_quitBttn;
    Text m_highScoreTxt;

    // Start is called before the first frame update
    void Start()
    {
        if(m_playOBJ == null || m_quitOBJ == null || m_highScoreOBJ == null)
        {
            print("ERROR : ScreenManager() -> null objects");
		}
        else
        {
            m_playBttn = m_playOBJ.GetComponent<Button>();
            m_quitBttn = m_quitOBJ.GetComponent<Button>();
            m_highScoreTxt = m_highScoreOBJ.GetComponent<Text>();

            if(m_playBttn)
                m_playBttn.onClick.AddListener(delegate{OnPlay();});
            else
                print("ERROR : ScreenManager() -> null playbutton");

            if(m_quitBttn)
                m_quitBttn.onClick.AddListener(delegate{OnQuit();});
            else
                print("ERROR : ScreenManager() -> null m_quitBttn");
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
