using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.AI;

public class EnemyScript : NetworkBehaviour {
    [SerializeField]
    GameObject[] players;
    [SerializeField]
    NavMeshAgent navAgent;

    [SerializeField]
    GameObject target;

    [SerializeField]
    float health = 100;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        navAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        health = 100f;
	}
	
	// Update is called once per frame
	void Update () {
        GetClosestPlayer();
        if (navAgent != null && target != null)
        {
            navAgent.SetDestination(target.transform.position);
        }
	}

    private void GetClosestPlayer()
    {
        
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            float currDistance = (target.transform.position - transform.position).magnitude;
            if ((players[i].transform.position - transform.position).magnitude < currDistance)
            {
                target = players[i];
            }
        }

    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;
        if (health <= 0)
        {
            CmdDie();
        }
    }

    [Command]
    void CmdDie()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
