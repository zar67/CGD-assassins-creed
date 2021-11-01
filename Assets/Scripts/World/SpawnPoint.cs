using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SpawnPointManager.AddSpawnPoint(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
