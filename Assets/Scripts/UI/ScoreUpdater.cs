using TMPro;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText = default;

    private void Update()
    {
        if (ScoreManager.IsScoreDirty())
        {
            m_scoreText.text = ScoreManager.Score().ToString();
        }
    }
}