using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    float speed = 5;

    float inputX = 0;
    float inputY = 0;

    Vector3 direction;
    
    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        direction = new Vector3(0, 0, inputY);

        transform.Translate(direction * speed * Time.deltaTime);

        if(inputX != 0)
        {
            transform.rotation = Quaternion.AngleAxis(Time.deltaTime * speed * inputX, transform.up) * transform.rotation;
        }
    }
}
