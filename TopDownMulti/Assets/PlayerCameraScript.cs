using UnityEngine;
using System.Collections;

public class PlayerCameraScript : MonoBehaviour {

    [SerializeField]
    GameObject target;
    [SerializeField]
    float smoothTime = 10.0f;
    [SerializeField]
    float zOffset;
    [SerializeField]
    float yOffset;
    float yOffMin = 5f;
    float yOffMax = 18f;
    [SerializeField]
    float xOffset;

    [SerializeField]
    Vector3 mousePos;

    [SerializeField]
    float mag;

    Vector3 smoothVelocity;
    public Transform[] targets;

    Vector3 newPos = new Vector3();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //newPos.x = target.transform.position.x + xOffset;
        //newPos.z = target.transform.position.z + zOffset;
        //newPos.y = target.transform.position.y + yOffset;
        //transform.position = newPos;
        //transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        CenterCamOnTargets();
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            yOffset -= .5f;
            if (yOffset < yOffMin)
            {
                yOffset = yOffMin;
            }
        }
        else if (scroll < 0f)
        {
            yOffset += .5f;
            if (yOffset > yOffMax)
            {
                yOffset = yOffMax;
            }
        }
    }

    void CenterCamOnTargets()
    {
        //var bounds = new Bounds(target.transform.position, Vector3.zero);

        //bounds.Encapsulate(mousePos);

        Vector3 bounds; 
        bounds.x = (mousePos.x + transform.position.x) / 2;
        bounds.z = (mousePos.z + transform.position.z) / 2;
        bounds.y = target.transform.position.y;

        Vector3 dist;
        dist.x = mousePos.x - target.transform.position.x;
        dist.y = 0.0f;
        dist.z = mousePos.z - target.transform.position.z;

        Debug.DrawLine(mousePos, target.transform.position, Color.red);

        mag = dist.magnitude;
        if ((bounds - target.transform.position).magnitude > 10.0f)
        {
            
            return;
        }
        
        newPos.x = bounds.x + xOffset;
        newPos.z = bounds.z + zOffset;
        newPos.y = bounds.y + yOffset;
        transform.position = Vector3.Lerp(transform.position, newPos, smoothTime * Time.deltaTime);
    }

    public void SetMousePos(Vector3 _mousePos)
    {
        mousePos = _mousePos;
    }
}
