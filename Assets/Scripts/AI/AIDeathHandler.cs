using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeathHandler : MonoBehaviour
{
    [SerializeField] private AIMovement AIMove = default;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Hit by player! preparing to die");
            Destroy(AIMove.gameObject);
        }
    }
}
