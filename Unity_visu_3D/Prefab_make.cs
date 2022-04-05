using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class Prefab_make
{
    GameObject container;
    // contains all objects that will form the current mesh

    public bool hide_roof;
    // hide the roof true/false
    bool old_hide_value;
    // old "hide the roof" value for comparison purposes

    float wallSize;
    // size of the walls

    float previousWallSize;
    // previous size of the walls
    float currentWallSize;
    // current size of the walls
    float wallWidth;
    // width of the walls
    float previousWallWidth;
    // previous width of the walls
    float currentWallWidth;
    // current width of the walls

    float adjustFloorSize;
    // size of the floor/roof

    float previousFloorSize;
    // previous size of the floor / roof
    float currentFloorSize;
    // current size of the floor / roof

    readfile rf;
    // a file reader

    public Loader ld;
    // an infomration and components loader
    string[] filelist_walls;
    // list of all "walls" files
    string wall_file;
    // name of the actual "walls" file

    char fileTag;
    // tag of the actual file considered (0, 1, 2, ...)

    int nFile;
    // placement of the current file in the list

    string item_file;
    // name of the actual "items" file

    string door_file;
    // name of the actual "doors" file


    bool just_switched;
    // did we just switch to making mode ? true / false

    bool mesh_made = false;
    // is the mesh made ? true / false

    GameObject rayMaker;
    // raymaker object to find exterior walls and place roofs / floors

    public Prefab_make(Loader l)
    // make a prefab
    {
        this.ld = l; // get the Loader
        nFile = 0;
        // start with the first file in the list

        filelist_walls = Directory.GetFiles(Application.dataPath + "/", "*_mur.txt");
        // get all "walls" files
        if (filelist_walls.Length <= 0)
        // if there is no "walls" file at all
        {
            Debug.Log("No file to read ! Add files to Assets/");
            UnityEditor.EditorApplication.isPlaying = false; // stop the application
        }

        wallSize = 20;
        previousWallSize = wallSize;
        wallWidth = 6.35f;
        previousWallWidth = wallWidth;
        adjustFloorSize = 52;
        previousFloorSize = adjustFloorSize;
        // set basic wall size, width, and floor size

        while (!ld.isFinished()) Debug.Log("Waiting for textures...");
        // wait for the Loader to finish loading all textures

        old_hide_value = hide_roof;
        // get the old hide roof value

        just_switched = true;
        // we just switched to making mode
        mesh_made = false;
        // so the mesh is not made yet
    }

    public void update_make()
    // update the current mesh
    {
        if (just_switched)
        // if we just switched to making mode (or to a new "walls" file)
        {
            this.rayMaker = new GameObject();
            this.rayMaker.AddComponent<RayCaster>();
            // recreate the raymaker and give it the script
            createMesh();
            // create the current mesh
            just_switched = false;
            // get out of the "just switched" state
        }
        if (!mesh_made && this.rayMaker.GetComponent<RayCaster>().OpIsDone())
        // if the mesh is not made yet, and the operation of the raymaker is done
        {
            mesh_made = true; // the mesh is now made
            GameObject.Destroy(this.rayMaker); // destroy the raymaker
        }
        else if (mesh_made)
        // if the mesh is made
        {
            bool right_cycle = false;
            bool left_cycle = false;
            bool makePrefab = false;
            // various booleans to switch betweeen objects and save a prefab

            if (Input.GetKeyDown(KeyCode.Return)) makePrefab = true; // make prefab if press Enter
            if (Input.GetKeyDown(KeyCode.RightArrow)) right_cycle = true; // right arrow switch rigth
            if (Input.GetKeyDown(KeyCode.LeftArrow)) left_cycle = true; // left arrow switch left

            if (Input.GetKeyDown(KeyCode.H)) hide_roof = !hide_roof; // H hides the roof

            if (Input.GetKey(KeyCode.A)) // A decreases wall size
            {
                if (wallSize >= 1) wallSize -= 0.1f;
            }
            else if (Input.GetKey(KeyCode.Z)) // Z increases wall size
            {
                if (wallSize <= 100) wallSize += 0.1f;
            }

            if (Input.GetKey(KeyCode.Q)) // Q decreases wall width
            {
                if (wallWidth >= 1) wallWidth -= 0.1f;
            }
            else if (Input.GetKey(KeyCode.S)) // S increases wall width
            {
                if (wallWidth <= 10) wallWidth += 0.1f;
            }

            if (Input.GetKey(KeyCode.W)) // W decreases floor / roof size
            {
                if (adjustFloorSize >= 0) adjustFloorSize -= 0.1f;
            }
            else if (Input.GetKey(KeyCode.X)) // X increases floor / roof size
            {
                if (adjustFloorSize <= 60) adjustFloorSize += 0.1f;
            }

            if (right_cycle)
            // if cycling right
            {
                right_cycle = false; // not cycling right anymore
                nFile += 1; // increase file position in list
                if (nFile >= filelist_walls.Length)
                {
                    nFile = 0; // go back to the start if going too far
                }
                mesh_made = false; // mesh is not made anymore
                just_switched = true; // we just switched
            }
            if (left_cycle)
            // if cycling left
            {
                left_cycle = false;// not cycling right anymore
                nFile -= 1;// decrease file position in list
                if (nFile < 0)
                {
                    nFile = filelist_walls.Length - 1;// go back to the end if going too far
                }
                mesh_made = false; // mesh is not made anymore
                just_switched = true;// we just switched
            }
            if (makePrefab)
            // if we make a prefab
            {
                registerPrefab(); // save a prefab
            }

            updateMesh(); // update the mesh
        }
    }

    private void updateMesh()
    // updating the mesh
    {
        currentWallSize = wallSize;
        currentWallWidth = wallWidth;
        currentFloorSize = adjustFloorSize;
        // update wall size and width, and floor/roof size

        float changeSize = currentWallSize - previousWallSize;
        float changeWidth = currentWallWidth - previousWallWidth;
        float changeFloorSize = currentFloorSize - previousFloorSize;
        // define the quantity that still needs to change


        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        // get all walls
        foreach (GameObject wall in walls)
        // for all walls
        {
            wall.transform.localScale = new Vector3(wall.transform.localScale.x + changeWidth, wall.transform.localScale.y + changeSize, wall.transform.localScale.z);
            // update the size and width
        }

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        // get all floors
        foreach (GameObject floor in floors)
        // for all floors
        {
            floor.transform.position = new Vector3(-rf.meanX, -wallSize / 2, rf.meanZ);
            floor.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
            // update position and size
            if (old_hide_value != hide_roof)
            // if we pressed the "hide roof" button
            {
                if (hide_roof) floor.GetComponent<MeshRenderer>().enabled = false; // disable the floor renderer
                else floor.GetComponent<MeshRenderer>().enabled = true; // enable the floor renderer
            }
        }

        GameObject[] roofs = GameObject.FindGameObjectsWithTag("roof");
        // get all roofs
        foreach (GameObject roof in roofs)
        // for all roofs
        {
            roof.transform.position = new Vector3(-rf.meanX, wallSize / 2, rf.meanZ);
            roof.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
            // update position and size
            if (old_hide_value != hide_roof)
            // if we pressed the "hide roof" button
            {
                if (hide_roof) roof.GetComponent<MeshRenderer>().enabled = false;// disable the roof renderer
                else roof.GetComponent<MeshRenderer>().enabled = true;// enable the roof renderer
            }
        }

        old_hide_value = hide_roof; // update the hide roof value

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;
        // update the previous values

    }

    private void createMesh()
    // create a mesh for the current file
    {
        if (GameObject.Find("container") != null)
        {
            GameObject.DestroyImmediate(GameObject.Find("container"));
            // if the container object already exists, destroy it
        }
        container = new GameObject("container");
        // create a container
        container.gameObject.tag = "prefab";
        // give it a special tag

        wall_file = filelist_walls[nFile];
        // get the name of the "walls" file from the list

        string cutname = wall_file.Split()[wall_file.Split().Length - 1];
        // get the name without the path
        string justname = cutname.Split()[0];
        // get the name without the extension
        fileTag = justname[0];
        // get the tag in the file name

        rf = new readfile(wall_file, "walls");
        rf.read();
        // read the current "walls" file

        string[] filelist_logos = Directory.GetFiles(Application.dataPath + "/", fileTag + "_items.txt");
        if (filelist_logos.Length > 0) item_file = filelist_logos[0];
        // read the current "items" file

        string[] filelist_doors = Directory.GetFiles(Application.dataPath + "/", fileTag + "_doors.txt");
        if (filelist_doors.Length > 0) door_file = filelist_doors[0];
        // read the current "doors" file

        for (int i = 0; i < rf.myarray.Length; i += 4)
        // move in the corrdinate arrays 4 by 4 (x1 z1 x2 z2 for each wall, 1 start and 2 end of wall)
        {
            createWall(i);
            // create a wall
        }

        make_roof_floor();
        // make the roofs and floors


        previousWallSize = wallSize;
        previousWallWidth = wallWidth;
        previousFloorSize = adjustFloorSize;
        // update all previous values
    }

    void SetTarget(GameObject cube, Vector3 target)
    // give a target to an object
    {
        Vector3 direction = target - cube.transform.position;
        // define the direction vector
        cube.transform.localScale = new Vector3(wallWidth, wallSize, direction.magnitude + 0.5f);
        // adjust object size and width
        cube.transform.position = cube.transform.position + (direction / 2);
        // adjust object position to halfway to the target (Unity objects extend from the middle)
        cube.transform.LookAt(target);
        // turn object to face the target
    }

    private void make_roof_floor()
    // make roofs / floors
    {
        float maxX = -rf.minX;
        float minX = -rf.maxX;
        float maxZ = rf.maxZ;
        float minZ = rf.minZ;
        // get extreme values for this file

        this.rayMaker.GetComponent<RayCaster>().setVal(minX, maxX, minZ, maxZ);
        // set the values in the raymaker and start the recognition of external walls

    }


    void createWall(int index)
    // creaet a wall with a texture
    {
        Vector3 A = new Vector3(-rf.myarray[index], 0, rf.myarray[index + 1]);
        Vector3 B = new Vector3(-rf.myarray[index + 2], 0, rf.myarray[index + 3]);
        // get the start A of the wall and its end B
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // create a cube
        cube.name = "wall_" + index;
        cube.gameObject.tag = "wall";
        // name it as a "wall"
        cube.transform.parent = container.transform;
        // move it into the container
        cube.transform.position = A;
        SetTarget(cube, B);
        // set the cube in position A and make it look to position B
        LoadMaterial(cube, "wall");
        // give the cube the "wall" texture
    }

    void placeDoor(float x, float y)
    // place a door
    {
        ;
    }

    void placeWindow(float x, float y)
    // place a window
    {
        ;
    }

    void placeItem(string itemType, float x, float z)
    // place an item
    {
        ;
    }

    void LoadMaterial(GameObject obj, string mat)
    // give an object a specific texture
    {
        Material[] materials = obj.GetComponent<MeshRenderer>().materials;
        // get the materials of the objects

        if (materials.Length > 0 && ld.hasMat(mat))
        // if there is place to put a materialn and the material asked for exists in the Loader
        {
            if (!ld.mat_is_null(mat))
            // if the material in the Loader is not null
            {
                materials[0] = ld.getMat(mat);
                // get the texture from the Loader
                obj.GetComponent<MeshRenderer>().materials = materials;
                // apply the texture
            }
        }
    }

    void registerPrefab()
    // save current object as a prefab
    {
        string[] parts = wall_file.Split('/');
        string txtname = parts[parts.Length - 1];
        string prefabName = txtname.Split('.')[0];
        string pathname = "Assets/Resources/Prefab/prefab_" + prefabName + ".prefab";
        //create a name for the prefab, and get the path name

        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("container"), pathname);
        // save the prefab
    }

    public void setSwitch(bool b)
    {
        this.just_switched = b;
    }

    public void clear()
    // destroy the current container
    {
        GameObject.Destroy(container);
    }

    public float getMeanX()
    {
        return rf.meanX;
    }

    public float getMeanZ()
    {
        return rf.meanZ;
    }
}
