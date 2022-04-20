using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class Utilities
{
    /*     MATERIALS     */

    public static string RED_MAT = "red_ball";
    public static string WALL_MAT = "wall_material";
    public static string FLOOR_MAT = "floor_material";
    public static string ROOF_MAT = "roof_material";
    public static void loadMaterial(GameObject go, string mat)
    // give a material to an object
    {
        go.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/" + mat, typeof(Material));
    }

    public static void loadImg(GameObject go, string str)
    {
        go.GetComponent<Renderer>().material.mainTexture = LoadPNG(str);
    }

    public static string getMatfromTag(string tag){

        if(tag == TAG_WALL){
            return WALL_MAT;
        }
        else if (tag == TAG_ROOF)
        {
            return ROOF_MAT;
        }
        else if (tag == TAG_FLOOR)
        {
            return FLOOR_MAT;
        }
        else return RED_MAT;
    }


    /*     TAGS     */
    public static string TAG_START = "start";
    public static string TAG_CONTAINER = "container";
    public static string TAG_ROTATION = "rotation";
    public static string TAG_PREFAB = "prefab";
    public static string TAG_WALL = "wall";
    public static string TAG_FLOOR = "floor";
    public static string TAG_ROOF = "roof";
    public static string[] taglist = { TAG_START, TAG_CONTAINER, TAG_ROTATION, TAG_PREFAB, TAG_WALL, TAG_FLOOR, TAG_ROOF };
    // list of all tags

/*
    public static void loadAllTags()
    // initialize all tags
    {
        foreach (string tag in taglist)
        {
            // add tags to the project if they do not exist
            tagManagement(tag);
        }
    }


    private static void tagManagement(string tag)
    // add a tag to Unity
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        // open the tag manager
        string s = tag;

        bool found = false; // does the tag exit ? true / false
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(s))
            {
                found = true;
                break;
                // if the tag already exists, do nothing
            }
        }

        if (!found)
        {
            // else, add it
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
        }

        tagManager.ApplyModifiedProperties();
        // update the tag manager to accept the modifications
    }*/


    /*     CAMERA     */
    public static int farClipPlane = 10000; // camera far view limit
    public static Vector3 camera_position = new Vector3(-528, 449, -369); // camera position or best view
    public static float camera_speed_x = 5;
    public static float camera_speed_y = 5;
    public static float camera_scroll = 10;


    /*     KEY CODES     */

    public static KeyCode MAKE_PREFAB = KeyCode.Return; // key to make a prefab
    public static KeyCode CYCLE_RIGHT = KeyCode.RightArrow; // key to cycle right in list
    public static KeyCode CYCLE_LEFT = KeyCode.LeftArrow; // key to cycle left in list
    public static KeyCode HIDE_ROOF = KeyCode.H; // key to hide roof
    public static KeyCode DEC_WALLSIZE = KeyCode.A; // key to decrease wall size
    public static KeyCode INC_WALLSIZE = KeyCode.Z; // key to increase wall size
    public static KeyCode DEC_WALLWIDTH = KeyCode.Q; // key to decrease wall width
    public static KeyCode INC_WALLWIDTH = KeyCode.S;// key to increase wall width
    public static KeyCode DEC_FLOORSIZE = KeyCode.W; // key to decrease floor size
    public static KeyCode INC_FLOORSIZE = KeyCode.X;// key to increase floor size
    public static KeyCode MAKE_PREFAB_MODE = KeyCode.P; // key to go into prefab making mode
    public static KeyCode VISU_PREFAB_MODE = KeyCode.V;// key to go into visualization mode
    public static KeyCode QUIT = KeyCode.Escape; // key to close the program
    public static KeyCode CAMERA_ROTATION_MODE = KeyCode.R; // key to make the camera rotate
    public static KeyCode CAMERA_INVERT_ROTATION = KeyCode.T; // key to make the camera rotate


    /*     WALLS / FLOORS / ROOFS     */

    public static float wallSize = 20;
    public static float previousWallSize = 20;
    public static float WallSizeMin = 1;
    public static float WallSizeMax = 250;

    public static float wallWidth = 6.35f;
    public static float previousWallWidth = 6.35f;
    public static float WallWidthMin = 1;
    public static float WallWidthMax = 10;

    public static float adjustFloorSize= 52;
    public static float previousFloorSize = 52;
    public static float FloorSizeMin = 0;
    public static float FloorSizeMax = 60;

    public static float GROW_SPEED = 0.3f; // speed to inrease/decrease all parameters

    public static GameObject remakeObject(string tag)
        // remake an object if it already exists
        {
        if (GameObject.Find(tag) != null)
        {
            GameObject.DestroyImmediate(GameObject.FindWithTag(tag));
            // if the container object already exists, destroy it
        }
        GameObject obj = new GameObject(tag);
        obj.tag = tag;
        return obj;
    }

    public static GameObject createCube(string tag)
    // create a cube with a tag
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = tag;
        cube.gameObject.tag = tag;
        return cube;
    }


    public static void setTarget(GameObject cube, Vector3 target, float width, float size)
    // give a target to an object (make it look towards it)
    {
        Vector3 direction = target - cube.transform.position;
        // define the direction vector
        cube.transform.localScale = new Vector3(width, size, direction.magnitude + 0.5f);
        // adjust object size and width
        cube.transform.position = cube.transform.position + (direction / 2);
        // adjust object position to halfway to the target (Unity objects extend from the middle)
        cube.transform.LookAt(target);
        // turn object to face the target
    }

    public static void childToParent(GameObject child, GameObject parent)
    // make an object into another object's child
    {
        child.transform.parent = parent.transform;
    }


    /*     SAVE/LOAD PREFABS     */

    public static void savePrefab(GameObject go, int tag){
        string filepath = getPath() + OUTPUT_FOLDER_NAME +  "/prefab_" + tag + ".txt";
        if(File.Exists(filepath))
            File.Delete(filepath);

        StreamWriter writer = File.CreateText(filepath);
        foreach (Transform child in go.transform)
        {
            GameObject c = child.gameObject;
            writer.Write(c.name + "\n");
            writer.Write(c.tag + "\n");
            writer.Write(c.transform.position.x + "\n");
            writer.Write(c.transform.position.y + "\n");
            writer.Write(c.transform.position.z + "\n");
            writer.Write(c.transform.eulerAngles.x + "\n");
            writer.Write(c.transform.eulerAngles.y + "\n");
            writer.Write(c.transform.eulerAngles.z + "\n");
            writer.Write(c.transform.localScale.x + "\n");
            writer.Write(c.transform.localScale.y + "\n");
            writer.Write(c.transform.localScale.z + "\n");
            writer.Write(c.GetComponent<MeshRenderer>().enabled + "\n");
        }
        writer.Close();
        //AssetDatabase.Refresh();

    }

    public static void loadPrefab(GameObject collector, string filepath){
        if (!File.Exists(filepath))
            Debug.Log("Error File !");

        StreamReader reader = new StreamReader(filepath);
        string content = reader.ReadToEnd();
        string[] lines = content.Split('\n');
        for(int i = 0; i<lines.Length;i+=0)
        {
            string name = lines[i++];
            if(name.Length<=1) break;
            string tag = lines[i++];
            float posx = float.Parse(lines[i++]);
            float posy = float.Parse(lines[i++]);
            float posz = float.Parse(lines[i++]);
            float rotx = float.Parse(lines[i++]);
            float roty = float.Parse(lines[i++]);
            float rotz = float.Parse(lines[i++]);
            float scalex = float.Parse(lines[i++]);
            float scaley = float.Parse(lines[i++]);
            float scalez = float.Parse(lines[i++]);
            bool enabool = bool.Parse(lines[i++]);

            Vector3 pos = new Vector3(posx,posy,posz);
            Vector3 rot = new Vector3(rotx,roty,rotz);
            Vector3 scale = new Vector3(scalex,scaley,scalez);

            GameObject cube = createCube(tag);
            cube.name = name;
            cube.transform.position = pos;
            cube.transform.eulerAngles = rot;
            cube.transform.localScale = scale;
            cube.GetComponent<MeshRenderer>().enabled = enabool;
            string mat = getMatfromTag(tag);
            if (tag == TAG_FLOOR)
                loadImg(cube, getPath()+ INPUT_FOLDER_NAME+ '/' + IMG_FOLDER_NAME + "/plan.jpg");
            else
                loadMaterial(cube, getMatfromTag(tag));

            childToParent(cube,collector);
        }

        reader.Close();


    }

    /*       PATH         */

    public static string INPUT_FOLDER_NAME = "data";
    public static string OUTPUT_FOLDER_NAME = "data";
    public static string IMG_FOLDER_NAME = "plans";
    public static string MAC_PATH = "/../../";
    public static string WINDOWS_PATH = "/../../";

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        tex = FlipTexture(tex);
        return tex;
    }

    public static Texture2D FlipTexture(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        Array.Reverse(pixels);
        texture.SetPixels(pixels);
        return texture;
    }


    public static List<string> getFilesAt(string path, string ext){
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles(ext);
        List<string> allfiles = new List<string>();
        foreach(var f in fileInfo){
            string filename = f.FullName;
            filename = filename.Replace('\\','/');
            allfiles.Add(filename);
        }
        return allfiles;
    }

    public static string getPath(){
        string path = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            path += MAC_PATH;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path += WINDOWS_PATH;
        }
        return path;
    }

    public static void createDir(string dirname, string filePath){
        if (!Directory.Exists(filePath+dirname))
        {
            Directory.CreateDirectory(filePath+ dirname);
        }
    }

    /*      CAMERA      */
    public static float CAMERA_ROTATION_SPEED = 0.05f;


}
