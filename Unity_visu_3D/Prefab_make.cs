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

    Readfile rf;
    // a file reader

    List<string> filelist_walls;
    // list of all "walls" files

    char fileTag;
    // tag of the actual file considered (0, 1, 2, ...)
    int nFile;
    // placement of the current file in the list

    string wall_file;
    // name of the actual "walls" file
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

    Texture2D floor_mat;

    public Prefab_make()
    // make a prefab
    {
        filelist_walls = new List<string>();
        nFile = 0;
        // start with the first file in the list

        filelist_walls = Utilities.getFilesAt(Utilities.getPath()+Utilities.INPUT_FOLDER_NAME,"*_mur.txt");
        // get all "walls" files
        if (filelist_walls.Count <= 0)
        // if there is no "walls" file at all
        {
            Debug.Log("No file to read ! Add files to Assets/");
            //UnityEditor.EditorApplication.isPlaying = false; // stop the application
            Application.Quit();
        }

        wallSize = Utilities.wallSize;
        previousWallSize = Utilities.previousWallSize;
        wallWidth = Utilities.wallWidth;
        previousWallWidth = Utilities.previousWallWidth;
        adjustFloorSize = Utilities.adjustFloorSize;
        previousFloorSize = Utilities.previousFloorSize;
        // set basic wall size, width, and floor size


        just_switched = true;
        // we just switched to making mode
        mesh_made = false;
        // so the mesh is not made yet

        floor_mat = Utilities.LoadPNG(Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + '/' + Utilities.IMG_FOLDER_NAME + "/plan.jpg");
        
    }

    public void update_make()
    // update the current mesh
    {
        if (just_switched)
        // if we just switched to making mode (or to a new "walls" file)
        {
            //this.rayMaker = new GameObject();
            //this.rayMaker.AddComponent<RayCaster>();
            // recreate the raymaker and give it the script
            createMesh();
            // create the current mesh
            just_switched = false;
            // get out of the "just switched" state
        }
        if (!mesh_made)// && this.rayMaker.GetComponent<RayCaster>().OpIsDone())
        // if the mesh is not made yet, and the operation of the raymaker is done
        {
            mesh_made = true; // the mesh is now made
            //GameObject.Destroy(this.rayMaker); // destroy the raymaker
        }
        else if (mesh_made)
        // if the mesh is made
        {
            bool right_cycle = false;
            bool left_cycle = false;
            bool makePrefab = false;
            // various booleans to switch betweeen objects and save a prefab

            if (Input.GetKeyDown(Utilities.MAKE_PREFAB)) makePrefab = true; // make prefab if press Enter
            if (Input.GetKeyDown(Utilities.CYCLE_RIGHT)) right_cycle = true; // right arrow switch rigth
            if (Input.GetKeyDown(Utilities.CYCLE_LEFT)) left_cycle = true; // left arrow switch left

            if (Input.GetKeyDown(Utilities.HIDE_ROOF)) hide_roof = !hide_roof; // H hides the roof

            if (Input.GetKey(Utilities.DEC_WALLSIZE)) // A decreases wall size
            {
                if (wallSize >= Utilities.WallSizeMin) wallSize -= Utilities.GROW_SPEED;
            }
            else if (Input.GetKey(Utilities.INC_WALLSIZE)) // Z increases wall size
            {
                if (wallSize <= Utilities.WallSizeMax) wallSize += Utilities.GROW_SPEED;
            }

            if (Input.GetKey(Utilities.DEC_WALLWIDTH)) // Q decreases wall width
            {
                if (wallWidth >= Utilities.WallWidthMin) wallWidth -= Utilities.GROW_SPEED;
            }
            else if (Input.GetKey(Utilities.INC_WALLWIDTH)) // S increases wall width
            {
                if (wallWidth <= Utilities.WallWidthMax) wallWidth += Utilities.GROW_SPEED;
            }

            if (Input.GetKey(Utilities.DEC_FLOORSIZE)) // W decreases floor / roof size
            {
                if (adjustFloorSize >= Utilities.FloorSizeMin) adjustFloorSize -= Utilities.GROW_SPEED;
            }
            else if (Input.GetKey(Utilities.INC_FLOORSIZE)) // X increases floor / roof size
            {
                if (adjustFloorSize <= Utilities.FloorSizeMax) adjustFloorSize += Utilities.GROW_SPEED;
            }

            if (right_cycle)
            // if cycling right
            {
                right_cycle = false; // not cycling right anymore
                nFile += 1; // increase file position in list
                if (nFile >= filelist_walls.Count)
                {
                    nFile = 0; // go back to the start if going too far
                }
                clear();
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
                    nFile = filelist_walls.Count - 1;// go back to the end if going too far
                }
                clear();
                mesh_made = false; // mesh is not made anymore
                just_switched = true;// we just switched
            }
            if (makePrefab)
            // if we make a prefab
            {
                Utilities.savePrefab(container, int.Parse(fileTag+""));
                //registerPrefab(); // save a prefab
                GameObject.FindGameObjectWithTag(Utilities.TAG_ROTATION).GetComponent<CameraLookAt>().updatePrefabList();
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


        GameObject[] walls = GameObject.FindGameObjectsWithTag(Utilities.TAG_WALL);
        // get all walls
        foreach (GameObject wall in walls)
        // for all walls
        {
            wall.transform.localScale = new Vector3(wall.transform.localScale.x + changeWidth, wall.transform.localScale.y + changeSize, wall.transform.localScale.z);
            // update the size and width
        }

        GameObject[] floors = GameObject.FindGameObjectsWithTag(Utilities.TAG_FLOOR);
        // get all floors
        // update position and size
        foreach (GameObject floor in floors)
        // for all walls
        {
            floor.transform.localScale = new Vector3(floor.transform.localScale.x, floor.transform.localScale.y, floor.transform.localScale.z + changeFloorSize);
            // update the size and width
            floor.transform.position = new Vector3(floor.transform.position.x, floor.transform.position.y - changeSize/2, floor.transform.position.z);
        }

        GameObject[] roofs = GameObject.FindGameObjectsWithTag(Utilities.TAG_ROOF);
        foreach (GameObject roof in roofs)
        // for all walls
        {
            roof.transform.localScale = new Vector3(roof.transform.localScale.x, roof.transform.localScale.y, roof.transform.localScale.z + changeFloorSize);
            // update the size and width
            roof.transform.position = new Vector3(roof.transform.position.x, roof.transform.position.y + changeSize / 2, roof.transform.position.z);
            if (hide_roof) roof.GetComponent<MeshRenderer>().enabled = false; // disable the floor renderer
            else roof.GetComponent<MeshRenderer>().enabled = true; // enable the floor renderer
        }

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;
        // update the previous values

    }

    private void createMesh()
    // create a mesh for the current file
    {
        this.container = Utilities.remakeObject(Utilities.TAG_CONTAINER);
        // create a container

        wall_file = filelist_walls[nFile];
        // get the name of the "walls" file from the list

        string cutname = wall_file.Split('/')[wall_file.Split('/').Length - 1];
        // get the name without the path
        string justname = cutname.Split('.')[0];
        // get the name without the extension
        fileTag = justname[0];
        // get the tag in the file name

        rf = new Readfile(wall_file, "walls");
        rf.read();
        // read the current "walls" file

        /*
        string[] filelist_logos = Directory.GetFiles(Application.dataPath + "/", fileTag + "_items.txt");
        if (filelist_logos.Length > 0) item_file = filelist_logos[0];
        // read the current "items" file

        string[] filelist_doors = Directory.GetFiles(Application.dataPath + "/", fileTag + "_doors.txt");
        if (filelist_doors.Length > 0) door_file = filelist_doors[0];
        // read the current "doors" file
        */

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

    public void loadtext(GameObject obj)
    {
        if (obj.tag == Utilities.TAG_FLOOR)
            Utilities.loadImg(obj, Utilities.getPath() + Utilities.INPUT_FOLDER_NAME + '/' + Utilities.IMG_FOLDER_NAME + "/plan.jpg");
        else
            Utilities.loadMaterial(obj, Utilities.getMatfromTag(obj.tag));
    }

    private void make_roof_floor()
    // make roofs / floors
    {
        float maxX = rf.minX;
        float minX = rf.maxX;
        float maxZ = rf.maxZ;
        float minZ = rf.minZ;
        // get extreme values for this file

       // this.rayMaker.GetComponent<RayCaster>().setVal(minX, maxX, minZ, maxZ);
        // set the values in the raymaker and start the recognition of external walls

        float meanX = (minX + maxX) / 2;
        float meanZ = (minZ + maxZ) / 2;

        GameObject cube_floor_center = Utilities.createCube(Utilities.TAG_FLOOR);
        cube_floor_center.transform.parent = container.transform;
        cube_floor_center.GetComponent<Renderer>().material.mainTexture = floor_mat;

        GameObject cube_roof_center = Utilities.createCube(Utilities.TAG_ROOF);
        cube_roof_center.transform.parent = container.transform;
        //loadtext(cube_roof_center);
        cube_roof_center.GetComponent<MeshRenderer>().enabled = hide_roof;

        cube_floor_center.transform.position = new Vector3(-meanX, -wallSize / 2, meanZ);
        cube_roof_center.transform.position = new Vector3(-meanX, wallSize / 2, meanZ);

        cube_floor_center.transform.localScale = new Vector3(meanX * 2 - adjustFloorSize, 1, meanZ * 2 - adjustFloorSize);
        cube_roof_center.transform.localScale = new Vector3(meanX * 2 - adjustFloorSize, 1, meanZ * 2 - adjustFloorSize);

        //Mesh _mesh = cube_floor_center.GetComponent<MeshFilter>().mesh;
        //Vector3[] normals

    }


    void createWall(int index)
    // creaet a wall with a texture
    {
        Vector3 A = new Vector3(-rf.myarray[index], 0, rf.myarray[index + 1]);
        Vector3 B = new Vector3(-rf.myarray[index + 2], 0, rf.myarray[index + 3]);
        // get the start A of the wall and its end B

        GameObject cube = Utilities.createCube(Utilities.TAG_WALL);
        // create a cube
        cube.name = "wall_" + index;
        // name it as a "wall_index"
        Utilities.childToParent(cube,container);
        cube.transform.parent = container.transform;
        // move it into the container
        cube.transform.position = A;
        Utilities.setTarget(cube, B, wallWidth, wallSize);
        // set the cube in position A and make it look to position B
        Utilities.loadMaterial(cube, Utilities.WALL_MAT);
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

/*
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
*/

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
