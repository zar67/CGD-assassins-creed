using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheckFloor : MonoBehaviour
{
    [SerializeField] private AIMovement AIMove = default;
    [SerializeField] private bool leftSide = default;


    private int numberTouching = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        numberTouching++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        numberTouching--;

        if (numberTouching == 0)
        {
            AIMove.ReachedEndOfFloor(leftSide);
        }
    }
}
