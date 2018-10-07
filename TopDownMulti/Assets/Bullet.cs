using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {
    
    [SerializeField]
    TrailRenderer mybt;
    bool isActive = true;
    float timeAlive = 0;
    float yPos;

    [SyncVar]
    [SerializeField]
    Vector3 targetPos;
    [SerializeField]
    float damage;
	// Use this for initialization
	void Start ()
    {
        mybt = GetComponent<TrailRenderer>();
        yPos = transform.position.y;
        //targetPos = transform.forward * 150f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 50.0f * Time.deltaTime);
        timeAlive += Time.deltaTime;
        if (isActive)
        {
            if (timeAlive >= mybt.time)
            {
                isActive = false;
            }
            //transform.Translate(Vector3.forward * 50 * Time.deltaTime);
            //transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            
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
            if (timeAlive >= .01f)
            {
                mybt.enabled = true;
            }
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        other.GetComponent<EnemyScript>().TakeDamage(damage);
    //    }
    //}

    //public void SetDamage(float _damage)
    //{
    //    damage = _damage;
    //}

    public void SetTargetPos(Vector3 _pos)
    {
        targetPos = _pos;
        //targetPos.y = 1.0f;
        targetPos.x = _pos.x - transform.position.x;
        targetPos.y = 1.0f;
        targetPos.z = _pos.z - transform.position.z;
        targetPos = targetPos.normalized;
        targetPos = (targetPos * 150f) + transform.position;
    }

    [Server]
    void CmdDie()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
