using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    private bool gettingWalls = false;
    private bool opDone = false;

    private List<GameObject> external_walls;
    public bool wall_list_loaded = false;



    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> external_walls = new List<GameObject>();
        this.wall_list_loaded = true;
    }

    public void start_operation()
    {
        this.setOp(false);
        this.setGettingWalls(true);
    }

    public bool OpIsDone()
    {
        return (this.opDone == true);
    }

    public void setOp(bool b)
    {
        this.opDone = b;
    }

    public void setGettingWalls(bool b)
    {
        this.gettingWalls = b;
    }

    public void stop_operation()
    {
        this.setOp(true);
        this.setGettingWalls(false);
    }

    public void setVal(float minx, float maxx, float minz, float maxz)
    {
        this.minX = minx;
        this.minZ = minz;
        this.maxX = maxx;
        this.maxZ = maxz;
        this.start_operation();
    }

    void Update()
    {
        if (this.gettingWalls && this.wall_list_loaded) this.get_external_walls();
        // si on est entrain de chercher les murs exterieurs
    }

    private void get_external_walls()
    {
        Debug.Log("dans get external walls");
        float distance = 120;
        float ecart = 100;
        float duration = 300;

        this.external_walls = new List<GameObject>(); // liste des murs exterieurs

        RaycastHit hit;

        Debug.Log("avant le 1");
        for (float i = maxX; i > minX; i--)
        { // depuis le bas (vue du dessus, z est vers le nord)
            Vector3 startpoint = new Vector3(i, 0, minZ - ecart); // point de départ
            Vector3 direction = Vector3.forward;    // direction

            Debug.DrawRay(startpoint, direction * distance, Color.red, duration); // debug

            if (Physics.Raycast(startpoint, direction, out hit, maxZ + ecart))
            {

                if (!this.external_walls.Contains(hit.collider.gameObject))
                {
                    this.external_walls.Add(hit.collider.gameObject); // ajoute objet touché
                    Debug.Log("Hit");
                }
            }
        }

        Debug.Log("apres le 1");

        Debug.Log("avant le 2");
        for (float i = minZ; i < maxZ; i++)
        { // depuis le bas (vue du dessus, z est vers le nord)
            Vector3 startpoint = new Vector3(minX - ecart, 0, i); // point de départ
            Vector3 direction = Vector3.right;    // direction

            Debug.DrawRay(startpoint, direction * distance, Color.green, duration); // debug

            if (Physics.Raycast(startpoint, direction, out hit, maxX + ecart))
            {

                if (!this.external_walls.Contains(hit.collider.gameObject))
                {
                    this.external_walls.Add(hit.collider.gameObject); // ajoute objet touché
                    Debug.Log("Hit");
                }
            }

        }
        Debug.Log("apres le 2");

        Debug.Log("avant le 3");
        for (float i = minX; i < maxX; i++)
        { // depuis le bas (vue du dessus, z est vers le nord)
            Vector3 startpoint = new Vector3(i, 0, maxZ + ecart); // point de départ
            Vector3 direction = -Vector3.forward;    // direction

            Debug.DrawRay(startpoint, direction * distance, Color.blue, duration); // debug

            if (Physics.Raycast(startpoint, direction, out hit, minZ - ecart))
            {

                if (!this.external_walls.Contains(hit.collider.gameObject))
                {
                    this.external_walls.Add(hit.collider.gameObject); // ajoute objet touché
                    Debug.Log("Hit");
                }
            }

        }
        Debug.Log("apres le 3");

        Debug.Log("avant le 4");
        for (float i = maxZ; i > minZ; i--)
        { // depuis le bas (vue du dessus, z est vers le nord)
            Vector3 startpoint = new Vector3(maxX + ecart, 0, i); // point de départ
            Vector3 direction = -Vector3.right;    // direction

            Debug.DrawRay(startpoint, direction * distance, Color.yellow, duration); // debug

            if (Physics.Raycast(startpoint, direction, out hit, minX - ecart))
            {

                if (!this.external_walls.Contains(hit.collider.gameObject))
                {
                    this.external_walls.Add(hit.collider.gameObject); // ajoute objet touché
                    Debug.Log("Hit");
                }
            }

        }
        Debug.Log("apres le 4");

        Debug.Log("end of operations");

        foreach (GameObject w in this.external_walls)
        {
            w.GetComponent<MeshRenderer>().enabled = false;
            Debug.Log(w);
        }

        this.stop_operation(); // no more getting walls
    }
}
