using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCheck : MonoBehaviour
{
    [SerializeField] private AIMovement AIMove = default;

    private int numberTouching = 0;

    public bool TouchingFloor()
    {
        return numberTouching > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        numberTouching++;
        if (numberTouching == 1)
        {
            AIMove.TouchedFloor();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        numberTouching--;

        if (numberTouching == 0)
        {
            AIMove.LeftFloor();
        }
    }
}
