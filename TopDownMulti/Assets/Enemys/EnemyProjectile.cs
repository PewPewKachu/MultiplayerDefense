using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class EnemyProjectile : NetworkBehaviour
{
    [SerializeField] float speed;
    Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        Invoke("CmdDie", 15.0f);
    }

    void Update ()
    {
        rigid.velocity = transform.forward * speed;
	}

    private void OnCollisionEnter(Collision collision)
    {
        Invoke("CmdDie", 0.5f);
    }

    [Command]
    void CmdDie()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
