using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawnerScript : NetworkBehaviour {
    [SerializeField]
    GameObject[] spawnPoints;

    [SerializeField]
    GameObject[] toSpawn;

    [SerializeField]
    float spawnCooldown = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnCooldown >= 0)
        {
            spawnCooldown -= Time.deltaTime;
        }
        else
        {
            CmdSpawnEnemy();
        }


    }

    [Command]
    void CmdSpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject enemyToSpawn = (GameObject)Instantiate(toSpawn[Random.Range(0, toSpawn.Length)], spawnPoints[spawnIndex].transform.position, spawnPoints[spawnIndex].transform.rotation);
        spawnCooldown = 2;

        NetworkServer.Spawn(enemyToSpawn);
    }

}
