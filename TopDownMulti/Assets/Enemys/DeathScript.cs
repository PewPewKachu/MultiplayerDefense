using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathScript : NetworkBehaviour
{
    [SerializeField] float time;
	void Start ()
    {
        Invoke("CmdDie", time);
	}


    [Command]
    void CmdDie()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
