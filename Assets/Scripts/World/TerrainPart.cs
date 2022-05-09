using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPart : MonoBehaviour
{
    public Transform endPosition;
    public TerrainHeight terrainHeight;

    public bool Compare(TerrainHeight other)
    {
        //if low return mid or low
        //if mid return low or high
        //if high return mid or high

        switch (terrainHeight)
        {
            case TerrainHeight.LOW:
                if (other == TerrainHeight.LOW) return true;
                if (other == TerrainHeight.MID) return true;
                else return false;
            case TerrainHeight.MID:
                if (other == TerrainHeight.LOW) return true;
                if (other == TerrainHeight.MID) return true;
                if (other == TerrainHeight.HIGH) return true;
                else return false;
            case TerrainHeight.HIGH:
                if (other == TerrainHeight.MID) return true;
                if (other == TerrainHeight.HIGH) return true;
                else return false;
        }

        return false;
    }
}

public enum TerrainHeight
{
    HIGH,
    MID,
    LOW,
}