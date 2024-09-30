using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSpawner : MonoBehaviour
{
    public ObjectPool _pool;
    public float time = 0.5f; 
    public int particleCountPerSecond = 10; // Number of particles to spawn per second
    public int particleCount = 100;
    public float spawnSize = 5f;
    public Color gizmoColor = Color.blue;
    // Draw the gizmos in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnSize * 2, spawnSize * 2, 1f));
    }
    private Coroutine spawnCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnParticles());
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    IEnumerator SpawnParticles()
    {
        while (true)
        {
            for (int i = 0; i < particleCountPerSecond; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    transform.position.x + Random.Range(-spawnSize, spawnSize),
                    transform.position.y + Random.Range(-spawnSize, spawnSize)
                );
                _pool.GetObject(spawnPosition);
            }
            yield return new WaitForSeconds(time);
        }
    }
}
