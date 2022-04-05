using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField]
    float speedX = 5;
    // x speed of camera

    float speedY = 5;
    // y speed of camera

    readfile rf;
    // a file reader

    [SerializeField]
    public GameObject _camera;
    // a camera

    private void Start()
    {
        rf = new readfile(Directory.GetFiles(Application.dataPath + "/", "*_mur.txt")[0], "walls");
        rf.read();
        // read a "wall" file to get all of the values
        transform.position = new Vector3(-rf.meanX, 0, rf.meanZ);
        // move teh current object to the center of the object in the file
        _camera.transform.parent = this.transform;
        // move the camera to the object
        _camera.GetComponent<Camera>().farClipPlane = 10000;
        // set camera render distance
    }
    void Update()
    {
        float inputX = Input.GetAxisRaw("Mouse X");
        // get mouse movements on X axis
        float inputY = Input.GetAxisRaw("Mouse Y");
        // get mouse movements on Y axis

        transform.Rotate(-inputY * Vector3.right * speedY, Space.Self);
        transform.Rotate(inputX * Vector3.up * speedX, Space.World);
        // move the camera around the current object

        _camera.transform.LookAt(transform.position);
        // look at the current object
    }
}
