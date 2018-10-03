using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {
    [SerializeField]
    TrailRenderer mybt;
    bool isActive = true;
    float timeAlive = 0;
    float yPos;

    [SerializeField]
    float damage;
	// Use this for initialization
	void Start ()
    {
        mybt = GetComponent<TrailRenderer>();
        yPos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeAlive += Time.deltaTime;
        if (isActive)
        {
            if (timeAlive >= mybt.time)
            {
                isActive = false;
            }
            transform.Translate(Vector3.forward * 50 * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
        else
        {
            if (timeAlive >= mybt.time * 2.0f)
            {
                CmdDie();
            }
        }

        if (mybt.enabled == false)
        {
            if (timeAlive >= .2f)
            {
                mybt.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().TakeDamage(damage);
        }
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    [Server]
    void CmdDie()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
