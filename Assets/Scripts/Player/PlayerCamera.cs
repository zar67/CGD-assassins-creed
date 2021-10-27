using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private const float DIFFICULTY = 1.0f;

    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private float m_movementSpeed = 15.0f;
    [SerializeField] private float m_difficultyMultiplier = 0.2f;

    private void Update()
    {
        Vector3 cameraPosition = m_mainCamera.transform.position;

        cameraPosition.x += m_movementSpeed * Time.deltaTime * DIFFICULTY * m_difficultyMultiplier;
        m_mainCamera.transform.position = cameraPosition;
    }
}
