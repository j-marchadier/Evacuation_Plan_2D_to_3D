using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Camera : MonoBehaviour
{
    [SerializeField]
    float speed = 5;

    float inputX = 0;
    float inputY = 0;

    Vector3 oldMousePos = Vector3.zero;
    Vector3 currentMousePos = Vector3.zero;

    Vector3 direction;

    readfile rf;

    private void Start()
    {
        rf = new readfile(Directory.GetFiles(Application.dataPath + "/", "*_mur.txt")[0], "walls");
        rf.read();
        transform.position = new Vector3(-rf.meanX, 0, rf.meanZ);
        transform.rotation = Quaternion.identity;
    }
    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        currentMousePos = Input.mousePosition;

        direction = new Vector3(0, 0, inputY);

        transform.Translate(direction * speed * Time.deltaTime);

        float mouseX = currentMousePos.x - oldMousePos.x;

        transform.rotation = Quaternion.AngleAxis(Time.deltaTime * speed * mouseX, transform.up) * transform.rotation;

        oldMousePos = currentMousePos;
    }
}
