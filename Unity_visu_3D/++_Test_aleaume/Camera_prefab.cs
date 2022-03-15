using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Camera_prefab : MonoBehaviour
{
    public GameObject target;
    public float sensitivity;

    public readfile rf;
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void FixedUpdate ()
    {
        target = GameObject.FindGameObjectWithTag("start"); // récupere object de base

        float rotateHorizontal = Input.GetAxisRaw("Mouse X");
        float rotateVertical = Input.GetAxisRaw("Mouse Y");
        transform.RotateAround (target.transform.position, -Vector3.up, rotateHorizontal * sensitivity);
        transform.RotateAround (Vector3.zero, transform.right, rotateVertical * sensitivity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
