using System.Linq;
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

    public List<GameObject> external_walls;
    public bool wall_list_loaded = false;

    public float actual_pos;
    public float actual_limit;
    public char actual_axis;
    public int actual_direction;

    public GameObject actual_wall;


    // Start is called before the first frame update
    void Start()
    {
        this.external_walls = new List<GameObject>();
        this.wall_list_loaded = true;
        this.actual_pos = maxX;
        this.actual_limit = maxZ;
        this.actual_axis = 'x';
        this.actual_direction = -1;
        this.actual_wall = null;
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

    void FixedUpdate()
    {
        if (this.gettingWalls && this.wall_list_loaded) // si on est entrain de chercher les murs exterieurs
        {
            this.get_external_walls();
        }

    }

    private void get_external_walls() // l'axe, la position, la limit de distance, et la direction
    {
        //Debug.Log("dans get external walls");
        float distance = 120;
        float ecart = 10;
        float duration = 300;

        Vector3 startpoint = new Vector3(); // point de départ
        Vector3 vect_direction = new Vector3(); // direction

        switch (actual_axis)
        {
            case 'x':
                if (actual_direction < 0)
                {
                    startpoint = new Vector3(actual_pos, 0, this.minZ - ecart); // starting point on the line
                    vect_direction = Vector3.forward;    // direction
                }
                else
                {
                    startpoint = new Vector3(actual_pos, 0, this.maxZ + ecart); // starting point on the line
                    vect_direction = -Vector3.forward;    // direction
                }
                break;
            case 'z':
                if (actual_direction > 0)
                {
                    startpoint = new Vector3(minX - ecart, 0, actual_pos); // starting point on the line
                    vect_direction = Vector3.right;    // direction
                }
                else
                {
                    startpoint = new Vector3(maxX + ecart, 0, actual_pos); // starting point on the line
                    vect_direction = -Vector3.right;    // direction
                }
                break;
            default:
                break;
        }


        RaycastHit hit;
        Ray ray = new Ray(startpoint, vect_direction);
        int layer_mask = LayerMask.GetMask("Default");

        Debug.DrawRay(startpoint, vect_direction * distance, Color.red, duration);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
        {
            //Debug.Log("Hit!");
            //Debug.Log(hit.collider.gameObject.tag);
            if (!this.external_walls.Contains(hit.collider.gameObject) && hit.collider.gameObject.tag == Utilities.TAG_WALL)
            {
                this.external_walls.Add(hit.collider.gameObject); // ajoute objet touché
                Utilities.loadMaterial(hit.collider.gameObject,Utilities.RED_MAT);

            }

            switch (actual_axis)
            // si hit, on bouge jusqu'a la fin du mur, 1 sinon
            {
                case 'x':
                    switch (actual_direction)
                    {
                        case 1:
                            this.actual_pos = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.max.x + 1;

                            //modif pour plafond
                            /*GameObject cube = makeRoof();

                            cube.transform.position = startpoint;
                            Vector3 target = startpoint;
                            target.x = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.min.x - 1;

                            Utilities.setTarget(cube,target,20,20);*/

                            break;
                        case -1:
                            this.actual_pos = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.min.x - 1;

                            break;
                        default:
                            break;
                    }
                    break;
                case 'z':
                    switch (actual_direction)
                    {
                        case 1:
                            this.actual_pos = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.max.z + 1;

                            //modif pour plafond
                            /*GameObject cube = makeRoof();

                            cube.transform.position = startpoint;
                            Vector3 target = startpoint;
                            target.z = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.min.z - 1;

                            Utilities.setTarget(cube, target, 20, 20);*/

                            break;
                        case -1:
                            this.actual_pos = hit.collider.gameObject.GetComponent<MeshRenderer>().bounds.min.z - 1;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }


        }
        else
        {
            //Debug.Log("Pas de hit !");
            this.actual_pos += this.actual_direction;

        }


        switch (actual_direction) // les changements de coté
        {
            case 1:
                switch (this.actual_axis)
                {
                    case 'x': // en haut vers la droite
                        if (this.actual_pos > this.maxX)
                        {
                            //Debug.Log("switch actual_axis !");
                            this.actual_pos = this.maxZ;
                            this.actual_axis = 'z';
                            this.actual_direction = -1;
                            this.actual_limit = this.minX;

                        }
                        break;
                    case 'z': // a gauche vers le haut
                        if (this.actual_pos > this.maxZ)
                        {
                            //Debug.Log("switch actual_axis !");
                            this.actual_pos = this.minX;
                            this.actual_axis = 'x';
                            this.actual_direction = 1;
                            this.actual_limit = this.minZ;

                        }
                        break;
                    default:
                        break;
                }
                break;
            case -1:
                switch (this.actual_axis)
                {
                    case 'x': // en bas vers la gauche
                        if (this.actual_pos < this.minX)
                        {
                            //Debug.Log("switch actual_axis !");
                            this.actual_pos = this.minZ;
                            this.actual_axis = 'z';
                            this.actual_direction = 1;
                            this.actual_limit = this.maxX;
                        }
                        break;
                    case 'z': // a droite vers le bas
                        if (this.actual_pos < this.minZ)
                        {
                            //Debug.Log("switch actual_axis !");
                            //Debug.Log("Operation finished !!!");

                            this.stop_operation(); // no more getting walls

                            /*
                            foreach (GameObject w in this.external_walls)
                            {
                                Debug.Log(w);
                            }*/
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private GameObject makeRoof(){
        GameObject cube = Utilities.createCube(Utilities.TAG_ROOF);
        cube.transform.parent = GameObject.FindWithTag(Utilities.TAG_CONTAINER).transform;
        Utilities.loadMaterial(cube, Utilities.ROOF_MAT);
        return cube;
    }
}
