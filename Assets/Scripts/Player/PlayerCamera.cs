using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera = default;

    [Header("Tutorial Camera")]
    [SerializeField] private Transform m_playerTransform = default;
    [SerializeField] private float m_smoothingSpeed = 0.125f;

    [Header("Normal Camera")]
    [SerializeField] private float m_movementSpeed = 8.0f;
    [SerializeField] private float m_minMovementSpeed = 3.0f;
    [SerializeField] private float m_maxMovementSpeed = 10.0f;
    [SerializeField] private float m_difficultyMultiplier = 0.1f;

    private bool m_tutorialCamera = true;

    private float m_yPosition;
    private float m_zPosition;

    public void DisableTutorialCamera()
    {
        m_tutorialCamera = false;
    }

    public void EnableTutorialCamera()
    {
        m_tutorialCamera = true;

        Vector3 position = m_playerTransform.position;
        position.y = m_yPosition;
        position.z = m_zPosition;

        m_mainCamera.transform.position = position;
    }

    private void Awake()
    {
        m_yPosition = m_mainCamera.transform.position.y;
        m_zPosition = m_mainCamera.transform.position.z;

        EnableTutorialCamera();
    }

    private void Update()
    {
        if (m_tutorialCamera)
        {
            Vector3 desiredPosition = m_playerTransform.position;
            Vector3 position = Vector3.Lerp(m_mainCamera.transform.position, desiredPosition, m_smoothingSpeed);
            position.y = m_yPosition;
            position.z = m_zPosition;

            m_mainCamera.transform.position = position;
        }
        else
        {
            Vector3 cameraPosition = m_mainCamera.transform.position;

            float movementSpeed = Mathf.Max(m_minMovementSpeed * Time.deltaTime, m_movementSpeed * Time.deltaTime * ScoreManager.Diffculty() * m_difficultyMultiplier);
            movementSpeed = Mathf.Min(movementSpeed, m_maxMovementSpeed * Time.deltaTime);

            cameraPosition.x += movementSpeed;
            m_mainCamera.transform.position = cameraPosition;
        }
    }
}