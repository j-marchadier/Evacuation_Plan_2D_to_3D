using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_check : MonoBehaviour
{
    Rigidbody rb;
    GameObject last_collision;
    bool collision = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        last_collision = collision.gameObject;
    }

    public GameObject getCollision(){
        return last_collision;
    }

    public bool has_collided(){
        return collision;
    }

    public void clear(){
        this.last_collision = null;
        this.collision = false;
    }
}
