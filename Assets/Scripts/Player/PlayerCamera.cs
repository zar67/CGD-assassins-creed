using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private Transform m_playerTransform;

    private void Update()
    {
        Vector3 cameraPosition = m_mainCamera.transform.position;

        cameraPosition.x = m_playerTransform.position.x;
        m_mainCamera.transform.position = cameraPosition;
    }
}
