using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIViewHandler : MonoBehaviour
{
    [SerializeField] private AIMovement AIMove = default;

    private bool lookingLeft = false;

    public void FlipView(bool _lookingLeft)
    {
        if (_lookingLeft == lookingLeft) return;

        lookingLeft = _lookingLeft;
        Debug.Log(transform.localPosition);
        transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            AIMove.ReachedWall(lookingLeft);
        }
        else if (other.tag == "Player")
        {
            Debug.Log("FOUND PLAYER!!!");
        }
    }
}
