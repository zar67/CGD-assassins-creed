using UnityEngine;

public class CameraStyleSwitcher : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Camera.main.GetComponent<PlayerCamera>().DisableTutorialCamera();
        }
    }
}
