using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Camera_prefab : MonoBehaviour
{
    public Transform target;
    public Camera cam;
    private float distanceToTarget = 1000;
    private Vector3 previousPosition;

    private bool stopped = true;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("camera").GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag("start").transform;
        if(target == null) Debug.Log("No camera target !");
        if(cam == null) Debug.Log("No camera !");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            distanceToTarget = distanceToTarget - 10;
            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            distanceToTarget = distanceToTarget + 10;
            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
        }
        else{
            if (Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y") != 0 && stopped)
            {
                previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
                stopped = false;
            }
            else if (Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y") != 0)
            {
                Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
                Vector3 direction = previousPosition - newPosition;

                float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
                float rotationAroundXAxis = direction.y * 180; // camera moves vertically

                cam.transform.position = target.position;

                cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
                cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

                cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

                previousPosition = newPosition;
            }
            else{
                stopped = true;
            }
        }
    }
}
