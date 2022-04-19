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

    Readfile rf;
    // a file reader

    [SerializeField]
    public GameObject _camera;
    // a camera

    public bool cameraInRotationMode = false;
    public int rotation_direction = 1;
    List<string> file_list;
    int file_num;

    private void Start()
    {
        file_list = Utilities.getFilesAt(Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + "/", "*_mur.txt");
        file_num = 0;
        updateView();
        // move teh current object to the center of the object in the file
        Utilities.childToParent(_camera,this.gameObject);
        // move the camera to the object
        _camera.GetComponent<Camera>().farClipPlane = Utilities.farClipPlane;
        // set camera render distance
    }

    public void updateView()
    {
        string file = file_list[file_num];
        rf = new Readfile(file, "walls");
        rf.read();
        // read a "wall" file to get all of the values
        transform.position = new Vector3(-rf.meanX, 0, rf.meanZ);
    }
    void Update()
    {
        if (Input.GetKeyDown(Utilities.CYCLE_RIGHT)) 
        {
            file_num += 1;
            if(file_num > file_list.Count - 1)
            {
                file_num = 0;
            }
            updateView();
        }
        else if (Input.GetKeyDown(Utilities.CYCLE_LEFT)) 
        {
            file_num -= 1;
            if (file_num < 0)
            {
                file_num = file_list.Count - 1;
            }
            updateView();
        }
        float inputX = 0;
        if(!this.cameraInRotationMode)
            inputX = Input.GetAxisRaw("Mouse X");
        else
            inputX = Utilities.CAMERA_ROTATION_SPEED * this.rotation_direction;
        // get mouse movements on X axis
        float inputY = Input.GetAxisRaw("Mouse Y");
        // get mouse movements on Y axis

        transform.Rotate(-inputY * Vector3.right * speedY, Space.Self);
        transform.Rotate(inputX * Vector3.up * speedX, Space.World);
        // move the camera around the current object

        _camera.transform.LookAt(transform.position);
        // look at the current object

        Vector3 directionZoom = transform.position - _camera.transform.position;

        _camera.transform.position += directionZoom.normalized * Input.mouseScrollDelta.y * 10;
    }

    public void switchMode(){
        this.cameraInRotationMode = !this.cameraInRotationMode;
    }

    public void invertRotation(){
        this.rotation_direction = -this.rotation_direction;
    }
}
