using System;
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

    List<string> file_list_w;
    List<string> file_list_p;
    int wfile_num;
    int pfile_num;

    public bool making = true;



    private void Start()
    {
        initAll();
        // move teh current object to the center of the object in the file
        Utilities.childToParent(_camera,this.gameObject);
        // move the camera to the object
        _camera.GetComponent<Camera>().farClipPlane = Utilities.farClipPlane;
        // set camera render distance

        setCamView();
    }

    private void initAll(){
        file_list_w = Utilities.getFilesAt(Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + "/", "*_mur.txt");
        file_list_p = Utilities.getFilesAt(Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + "/", "prefab*.txt");
        pfile_num = 0;
        wfile_num = 0;
    }

    public string prefabFileToWallFile(string file){
        string outfile = file;
        string filename = outfile.Split('/')[outfile.Split('/').Length - 1]; // name + extension
        int tfTag = int.Parse(filename.Split('_', '.')[1]); // tag in a prefab file 'prefab_1.txt'

        foreach (string f in file_list_w)
        {
            string wfilename = f.Split('/')[f.Split('/').Length - 1]; // name + extension
            int wfTag = int.Parse(wfilename.Split('_')[0]); // tag in a walls file '1_mur.txt'
            outfile =  f;
            if (wfTag == tfTag)
                break;

        }

        return outfile;
    }

    public void setCamView()
    {
        string targetFile = file_list_w[wfile_num] ;
        if(!making){
            string pfile = file_list_p[pfile_num];
            targetFile = prefabFileToWallFile(pfile);
        }

        rf = new Readfile(targetFile, "walls");
        rf.read(); // read a "wall" file to get all of the values
        transform.position = new Vector3(-rf.meanX, 0, rf.meanZ);
    }

    public void cycleInDir(int i){

        if(making){ // if in make mode
            wfile_num += i;
            if (wfile_num >= file_list_w.Count)
            {
                wfile_num = 0;
            }
            if(wfile_num < 0){
                wfile_num = file_list_w.Count - 1;
            }
        }
        else{
            pfile_num += i;
            if (pfile_num >= file_list_p.Count)
            {
                pfile_num = 0;
            }
            if (pfile_num < 0)
            {
                pfile_num = file_list_p.Count - 1;
            }
        }

        setCamView();

    }

    void Update()
    {
        if (Input.GetKeyDown(Utilities.CYCLE_RIGHT))
        {
            cycleInDir(1);
        }
        else if (Input.GetKeyDown(Utilities.CYCLE_LEFT))
        {
            cycleInDir(-1);
        }
        else if (Input.GetKeyDown(Utilities.VISU_PREFAB_MODE))
        {
            making = false;
            pfile_num = 0;
            setCamView();
        }
        else if(Input.GetKeyDown(Utilities.MAKE_PREFAB_MODE))
        {
            making = true;
            setCamView();
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

    public void updatePrefabList(){
        file_list_p.Clear();
        file_list_p = Utilities.getFilesAt(Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + "/", "prefab*.txt");
    }
}