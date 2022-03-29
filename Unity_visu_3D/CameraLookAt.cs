using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField]
    float speedX = 5;

    float speedY = 5;

    readfile rf;

    [SerializeField]
    public GameObject _camera;

    private void Start()
    {
        rf = new readfile(Directory.GetFiles(Application.dataPath + "/", "*_mur.txt")[0], "walls");
        rf.read();
        transform.position = new Vector3(-rf.meanX, 0, rf.meanZ);
        _camera.transform.parent = this.transform;
        _camera.GetComponent<Camera>().farClipPlane = 10000;
    }
    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Mouse X");
        float inputY = Input.GetAxisRaw("Mouse Y");

        transform.Rotate(-inputY * Vector3.right * speedY, Space.Self);
        transform.Rotate(inputX * Vector3.up * speedX, Space.World);

        _camera.transform.LookAt(transform.position);
    }
}
