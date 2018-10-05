using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DamageUi : NetworkBehaviour
{
    [SerializeField]
    [SyncVar]
    float damage, speed, die, Xrange;

    Text text;
    Color trans;

    EnemyScript enemy;
    Vector3 offset;

    Camera cam;

	void Start ()
    {
        text = GetComponent<Text>();
        trans = text.color;
        trans.a = 0.0f;
        Invoke("Die", die);
        Xrange = Random.Range(-Xrange, Xrange);
	}
	
	void Update ()
    {
        damage = enemy.getDamge();
        text.text = damage.ToString();
        text.color = Color.Lerp(text.color, trans, Time.deltaTime * 2.0f);

        offset += Vector3.up * Time.deltaTime * speed;
        offset.x += Xrange * Time.deltaTime;
        if (enemy != null)
            transform.position = cam.WorldToScreenPoint(enemy.gameObject.transform.position) + offset;
        else
            Die();
    }

    public void setDamage(float _damage, Camera _cam, EnemyScript _enemy)
    {
        cam = _cam;
        enemy = _enemy;
        damage = enemy.getDamge();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
