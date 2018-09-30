using UnityEngine;
using System.Collections;

public class PlayerCameraScript : MonoBehaviour {

    [SerializeField]
    GameObject target;
    [SerializeField]
    float smoothTime = 100.0f;
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

       

        Vector3 distPtoC;
        distPtoC.x = transform.position.x - target.transform.position.x;
        distPtoC.y = 0.0f;
        distPtoC.z = transform.position.z - target.transform.position.z;

        Vector3 distPtoM; //player to mouse dist
        distPtoM.x = mousePos.x - target.transform.position.x;
        distPtoM.y = 0.0f;
        distPtoM.z = mousePos.z - target.transform.position.z;

        Vector3 bounds;
        bounds = distPtoM / 2.0f;

        Debug.DrawLine(mousePos, target.transform.position, Color.red);

        mag = distPtoC.magnitude;
        if ((distPtoC).magnitude > 10.0f)
        {
            
            
        }
        else
        {
            newPos.x = bounds.x + xOffset;
            newPos.z = bounds.z + zOffset;
            newPos.y = bounds.y + yOffset;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref smoothVelocity, smoothTime);
        }
    }

    public void SetMousePos(Vector3 _mousePos)
    {
        mousePos = _mousePos;
    }
}
