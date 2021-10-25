using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<GameObject> worldPieces = new List<GameObject>();

    [SerializeField] private float DISTANCE_FROM_LAST_PART = 1;
    [SerializeField] private float SPAWN_DISTANCE = 5;

    private Vector2 lastPartEndPosition = Vector2.zero;
    private GameObject lastPartLoaded = null;
    private Dictionary<Vector2, GameObject> loadedParts = new Dictionary<Vector2, GameObject>();

    private void Start()
    {
        //Setup starting platform
        lastPartEndPosition = worldPieces[0].GetComponent<TerrainPart>().endPosition.position;
    }

    private void Update()
    {
        float distance = lastPartEndPosition.x - playerTransform.position.x;
        
        if(distance < SPAWN_DISTANCE)
        {
            lastPartEndPosition = SpawnTerrainPart().endPosition.position;
            loadedParts.Add(lastPartEndPosition, lastPartLoaded); //For destroying later
        }
    }

    private TerrainPart SpawnTerrainPart()
    {
        int partIndex = Random.Range(0, worldPieces.Count);
        Vector2 m_lastPosition = new Vector2(lastPartEndPosition.x + DISTANCE_FROM_LAST_PART, lastPartEndPosition.y);

        lastPartLoaded = Instantiate(worldPieces[partIndex], m_lastPosition, Quaternion.identity, this.transform);
        return lastPartLoaded.GetComponent<TerrainPart>();
    }
}
