using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private float m_movementSpeed = 15.0f;
    [SerializeField] private float m_difficultyMultiplier = 0.2f;

    private void Update()
    {
        Vector3 cameraPosition = m_mainCamera.transform.position;

        cameraPosition.x += m_movementSpeed * Time.deltaTime * ScoreManager.Diffculty() * m_difficultyMultiplier;
        m_mainCamera.transform.position = cameraPosition;
    }
}
