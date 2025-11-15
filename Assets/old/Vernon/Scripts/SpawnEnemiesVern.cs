using UnityEngine;
using System.Collections;

public class SpawnEnemiesVern : MonoBehaviour
{
	public Transform enemyPrefab;
	private Transform spawnPoint;
	public float timeBetweenWaves = 5f;
	private float countdown = 2f;
	private int waveIndex = 1;
	public Vector3 endPoint;

	public DirectedAgent directedAgent;
	public void SpawnEnemies()
	{
		if (countdown <= 0f)
		{
			StartCoroutine(SpawnWave());
			countdown = timeBetweenWaves;
		}

		countdown -= Time.deltaTime;
	}

	IEnumerator SpawnWave()
	{
		waveIndex++;
		for (int i = 0; i < waveIndex; i++)
		{
			SpawnEnemy();
			yield return new WaitForSeconds(0.5f);
		}

	}

	public void SpawnEnemy()
	{
		Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
		directedAgent = enemyPrefab.GetComponent<DirectedAgent>();

		enemyPrefab.GetComponent<DirectedAgent>().MoveToLocation(endPoint);
	}

	public void setSpawnPoint(Transform point)
    {
		spawnPoint = point;
    }

	public void setEndPoint(Vector3 end)
    {
		endPoint = end;
    }
}
