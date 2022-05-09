using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    static List<Transform> m_spawnPoints = new List<Transform>();
    public static void AddSpawnPoint(Transform _transform){m_spawnPoints.Add(_transform);}
	
	[SerializeField] GameObject m_enemyPrefab;

	private void Update()
	{
		if(m_spawnPoints.Count > 0)
		{
			int spawnAmount = (ScoreManager.Diffculty() < m_spawnPoints.Count) ? ScoreManager.Diffculty() : m_spawnPoints.Count;
			for(int i=0; i<spawnAmount; i++)
			{
				GameObject ai = Instantiate(m_enemyPrefab, m_spawnPoints[i].position, Quaternion.identity);
				ai.transform.SetParent(m_spawnPoints[i].parent);
			}
			while(m_spawnPoints.Count > 0)
			{
				Destroy(m_spawnPoints[0].gameObject);
				m_spawnPoints.RemoveAt(0);
			}
		}
	}
}
