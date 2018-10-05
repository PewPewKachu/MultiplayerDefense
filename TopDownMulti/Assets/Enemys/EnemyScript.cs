using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.AI;

public class EnemyScript : NetworkBehaviour
{
    //Behaviour
    enum EnemyType { meele, ranged};
    [SerializeField]
    EnemyType type;
    enum logicState { idel, followPack, agro, attack};
    [SerializeField]
    [SyncVar]
    logicState curState;

    [SerializeField]
    GameObject[] players, allies;
    [SerializeField]
    GameObject projectile, healthBar, deathParticles;
    [SerializeField]
    NavMeshAgent navAgent;

    [SerializeField]
    GameObject target, ally;

    [SerializeField]
    [SyncVar]
    float health, speed, rateOfFire, maxHealth;

    //Raycast
    RaycastHit hit;
    [SerializeField]
    LayerMask mask;

    Vector3 newPos;

    Animator anim;

    float currDistance = 10000000000;
    [SyncVar]
    bool dead = false;
    bool rate = false, once = false, first = false;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponentInChildren<Animator>();
        players = GameObject.FindGameObjectsWithTag("Player");
        navAgent = GetComponent<NavMeshAgent>();
        target = null;
        ally = GameObject.FindGameObjectWithTag("Enemy");
        maxHealth = health;
        navAgent.speed = speed;
        newPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetClosestPlayer();
        if (navAgent != null && !dead)
        {
            switch (curState)
            {
                case logicState.idel:
                    switch (type)
                    {
                        case EnemyType.meele:
                            if (target != null)
                                curState = logicState.agro;
                            else
                            {
                                findNearestAlly();
                                if (ally != null && (ally.transform.position - transform.position).magnitude > 5)
                                {
                                    newPos = ally.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                                    curState = logicState.followPack;
                                }
                                else
                                {
                                    if ((newPos - transform.position).magnitude < 10)
                                        newPos = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                                    else
                                        navAgent.SetDestination(newPos);
                                }

                            }
                            break;
                        case EnemyType.ranged:
                            once = false;
                            if ((newPos - transform.position).magnitude < 10)
                                newPos = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                            else
                                navAgent.SetDestination(newPos);
                            if (target != null)
                                curState = logicState.agro;
                            break;
                        default:
                            break;
                    }
                    break;
                case logicState.followPack:
                    switch (type)
                    {
                        case EnemyType.meele:
                            if (ally != null)
                            {
                                navAgent.SetDestination(newPos);
                                if ((newPos - transform.position).magnitude < 5)
                                    curState = logicState.idel;
                            }
                            break;
                        case EnemyType.ranged:
                            findNearestAlly();
                            if (!once)
                            {
                                once = true;
                                navAgent.SetDestination(allies[Random.Range(0, allies.Length)].transform.position);
                            }
                            if ((target.transform.position - transform.position).magnitude > 10)
                                curState = logicState.idel;
                            break;
                        default:
                            break;
                    }
                    break;
                case logicState.agro:
                    switch (type)
                    {
                        case EnemyType.meele:
                            if (target != null)
                            {
                                navAgent.SetDestination(target.transform.position);
                                if (Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, Mathf.Infinity, mask))
                                {
                                    Debug.DrawLine(transform.position, hit.collider.transform.position, Color.white);
                                    Debug.Log(hit.collider.tag);
                                    if (hit.collider.tag != "Player")
                                        Invoke("attensionSpan", 2.5f);
                                }
                                if ((target.transform.position - transform.position).magnitude < 2)
                                    curState = logicState.attack;
                            }
                            break;
                        case EnemyType.ranged:
                            if (target != null)
                            {
                                navAgent.SetDestination(target.transform.position);
                                if (Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, Mathf.Infinity, mask))
                                {
                                    Debug.DrawLine(transform.position, hit.collider.transform.position, Color.white);
                                    Debug.Log(hit.collider.tag);
                                    if (hit.collider.tag != "Player")
                                        Invoke("attensionSpan", 5f);
                                }
                                if ((target.transform.position - transform.position).magnitude < 15)
                                    curState = logicState.attack;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case logicState.attack:
                    switch (type)
                    {
                        case EnemyType.meele:
                            if (target != null)
                            {
                                anim.SetTrigger("attack");
                                if (target != null)
                                {
                                    navAgent.SetDestination(target.transform.position);
                                    if ((target.transform.position - transform.position).magnitude > 1)
                                        curState = logicState.agro;
                                }
                                else
                                    curState = logicState.idel;
                            }
                            break;
                        case EnemyType.ranged:
                            navAgent.SetDestination(transform.position);
                            if (target != null)
                            {
                                transform.LookAt(target.transform.position);
                                if (!rate)
                                {
                                    rate = true;
                                    Invoke("shoot", rateOfFire);
                                }
                                if ((target.transform.position - transform.position).magnitude > 20)
                                    curState = logicState.agro;
                                if ((target.transform.position - transform.position).magnitude < 10)
                                    curState = logicState.followPack;
                            }
                            else
                                curState = logicState.idel;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        else
            navAgent.SetDestination(transform.position);
        if (type == EnemyType.meele)
        {
            //Animation
            if (navAgent.destination != navAgent.transform.position)
                anim.SetBool("walk", true);
            else
                anim.SetBool("walk", false);
        }
    }

    private void shoot()
    {
        rate = false;
        GetClosestPlayer();
        if (target != null)
        {
            GameObject gameObj = Instantiate(projectile, transform.position, transform.rotation);
            NetworkServer.Spawn(gameObj);
        }
    }

    private void attensionSpan()
    {
        if (target != null && Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, Mathf.Infinity, mask))
        {
            curState = logicState.idel;
            target = null;
        }
    }

    private void GetClosestPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (target != null)
                currDistance = (target.transform.position - transform.position).magnitude;
            else
                currDistance = 100000000;
            if (Physics.Raycast(transform.position, (players[i].transform.position - transform.position).normalized, out hit, Mathf.Infinity ,mask))
            {
                if (hit.collider.tag == "Player")
                {
                    if ((players[i].transform.position - transform.position).magnitude < currDistance)
                    {
                        target = players[i];
                    }
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if(!first)
        {
            first = true;
            players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                GameObject temp = Instantiate(healthBar, transform.position, Quaternion.identity);
                temp.GetComponent<EnemyHealthUi>().setenemy(this, players[i].GetComponentInChildren<Camera>());
                temp.transform.parent = players[i].GetComponentInChildren<Canvas>().gameObject.transform;
            }
        }
        health -= _damage;
        if(type == EnemyType.meele)
            anim.SetTrigger("hurt");
        Debug.Log(health);
        if (health <= 0)
        {
            if (!dead)
            {
                dead = true;
                death();
            }
        }
    }

    private void findNearestAlly()
    {
        allies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < allies.Length; i++)
        {
            if(ally != null)
                currDistance = (ally.transform.position - transform.position).magnitude;
            else
                currDistance = 100000000;
            if ((allies[i].transform.position - transform.position).magnitude < currDistance)
            {
                ally = allies[i];
            }
        }

    }

    void death()
    {
        switch(type)
        {
            case EnemyType.meele:
                anim.SetTrigger("die");
                Invoke("CmdDie", 3.0f);
                break;
            case EnemyType.ranged:
                CmdDie();
                break;
        }
    }

    public float getHealth()
    {
        return health;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }

    [Command]
    void CmdDie()
    {
        GameObject temp = Instantiate(deathParticles, transform.position, Quaternion.identity);
        NetworkServer.Spawn(temp);

        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
