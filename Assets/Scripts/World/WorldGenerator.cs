using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TerrainPart startingPart;
    [SerializeField] private List<GameObject> worldPieces = new List<GameObject>();

    [SerializeField] private float DISTANCE_FROM_LAST_PART = 1;
    [SerializeField] private float SPAWN_DISTANCE = 5;
    [SerializeField] private float PART_DESTROY_DISTANCE = 20;

    private Vector2 lastPartEndPosition = Vector2.zero;
    private TerrainPart lastPartLoaded = null;
    private Dictionary<Vector2, TerrainPart> loadedParts = new Dictionary<Vector2, TerrainPart>();

    private void Start()
    {
        loadedParts.Clear();

        //Setup starting platform
        lastPartLoaded = startingPart;
        lastPartEndPosition = lastPartLoaded.endPosition.position;
    }

    private void Update()
    {
        float distance = lastPartEndPosition.x - playerTransform.position.x;
        
        if(distance < SPAWN_DISTANCE)
        {
            lastPartEndPosition = SpawnTerrainPart().endPosition.position; //NEEDS TO CHECK WHAT LAST PART WAS
            loadedParts.Add(lastPartEndPosition, lastPartLoaded);
        }

        List<Vector2> partsToDestroy = new List<Vector2>();

        foreach(KeyValuePair<Vector2, TerrainPart> p in loadedParts)
        {
            //Check distance between camera and end position
            Vector2 pk = p.Key;
            float distance_from_camera = cameraTransform.position.x - pk.x;
            
            if(distance_from_camera > PART_DESTROY_DISTANCE) partsToDestroy.Add(pk);
        }

        foreach(Vector2 v in partsToDestroy)
        {
            //Destroy parts in list
            Destroy(loadedParts[v].gameObject);
            loadedParts.Remove(v);
        }
    }
    
    private TerrainPart SpawnTerrainPart()
    {
        Vector2 m_lastPosition = new Vector2(lastPartEndPosition.x + DISTANCE_FROM_LAST_PART, lastPartEndPosition.y);

        GameObject lastPartObject = Instantiate(GetUsuableTerrainPart(), m_lastPosition, Quaternion.identity, this.transform);
        lastPartLoaded = lastPartObject.GetComponent<TerrainPart>();
        return lastPartLoaded;
    }

    private GameObject GetUsuableTerrainPart()
    {
        if(lastPartLoaded != null)
        {
            List<GameObject> usableParts = new List<GameObject>();

            foreach(GameObject p in worldPieces)
            {
                TerrainPart tp = p.GetComponent<TerrainPart>();

                if (lastPartLoaded.Compare(tp.terrainHeight))
                {
                    usableParts.Add(p);
                }
            }

            int partIndex = Random.Range(0, usableParts.Count);
            return usableParts[partIndex];
        }

        return null;
    }
}
