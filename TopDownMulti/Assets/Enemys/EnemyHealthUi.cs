using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnemyHealthUi : NetworkBehaviour
{
    [SerializeField] EnemyScript enemy;

    [SerializeField]
    float x_offset, y_offset;

    [SerializeField]
    Image fill;

    Vector3 offset;
    
    Slider slide;
    Camera cam;
    
	void Start ()
    {
        slide = GetComponent<Slider>();
        slide.maxValue = enemy.getMaxHealth();
        offset = new Vector3(x_offset, y_offset, 0);
        fill.color = Color.green;
    }
	
	void Update ()
    {
        slide.value = enemy.getHealth();
        if (slide.value <= 0)
            Destroy(gameObject);

        if (slide.value <= slide.maxValue * 0.25f)
            fill.color = Color.red;
        else if (slide.value <= slide.maxValue * 0.5f)
            fill.color = Color.yellow;
        else
            fill.color = Color.green;
        

        if (enemy != null)
            transform.position = cam.WorldToScreenPoint(enemy.gameObject.transform.position) + offset;
    }

    public void setenemy(EnemyScript _enemy, Camera _cam)
    {
        enemy = _enemy;
        cam = _cam;
    }
}
