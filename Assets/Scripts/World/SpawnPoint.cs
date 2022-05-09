using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Awake()
    {
        SpawnPointManager.AddSpawnPoint(gameObject.transform);
    }
}
