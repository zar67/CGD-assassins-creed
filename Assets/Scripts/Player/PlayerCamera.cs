using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera = default;
    [SerializeField] private float m_movementSpeed = 15.0f;
    [SerializeField] private float m_minMovementSpeed = 10.0f;
    [SerializeField] private float m_maxMovementSpeed = 25.0f;
    [SerializeField] private float m_difficultyMultiplier = 0.2f;

    private void Update()
    {
        Vector3 cameraPosition = m_mainCamera.transform.position;

        float movementSpeed = Mathf.Max(m_minMovementSpeed * Time.deltaTime, m_movementSpeed * Time.deltaTime * ScoreManager.Diffculty() * m_difficultyMultiplier);
        movementSpeed = Mathf.Min(movementSpeed, m_maxMovementSpeed * Time.deltaTime);

        cameraPosition.x += movementSpeed;
        m_mainCamera.transform.position = cameraPosition;
    }
}