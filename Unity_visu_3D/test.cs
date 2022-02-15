using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


public class test : MonoBehaviour
{
    GameObject container;

    [SerializeField]
    [Range(1, 100)]
    float wallSize = 1;

    float previousWallSize;
    float currentWallSize;

    [SerializeField]
    [Range(1, 10)]
    float wallWidth = 1;

    float previousWallWidth;
    float currentWallWidth;

    [SerializeField]
    [Range(0, 60)]
    float adjustFloorSize = 0;

    float previousFloorSize;
    float currentFloorSize;

    [SerializeField]
    bool makePrefab;

    [SerializeField]
    bool cycle;

    readfile rf;
    string[] filelist;
    string filename;

    int nFile;

    // Start is called before the first frame update
    void Start()
    {
        string[] taglist = {"wall","floor","roof"};
        foreach(string tag in taglist){
            tagManagement(tag);
        }
        nFile = 0;
        filelist = Directory.GetFiles(Application.dataPath + "/", "*.txt");

        while(!GetComponent<materialLoader>().isFinished());

        createMesh();
    }

    private void Update()
    {
        currentWallSize = wallSize;
        currentWallWidth = wallWidth;
        currentFloorSize = adjustFloorSize;

        float changeSize = currentWallSize - previousWallSize;
        float changeWidth = currentWallWidth - previousWallWidth;
        float changeFloorSize = currentFloorSize - previousFloorSize;


        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        foreach (GameObject wall in walls)
        {
            wall.transform.localScale = new Vector3(wall.transform.localScale.x + changeWidth, wall.transform.localScale.y + changeSize, wall.transform.localScale.z);
        }

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        foreach (GameObject floor in floors)
        {
            floor.transform.position = new Vector3(-rf.meanX, -wallSize / 2, rf.meanZ);
            floor.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
        }

        GameObject[] roofs = GameObject.FindGameObjectsWithTag("roof");
        foreach (GameObject roof in roofs){
            roof.transform.position = new Vector3(-rf.meanX, wallSize / 2, rf.meanZ);
            roof.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);

        }

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;

        if(makePrefab) {
            registerPrefab();
        }

        if(cycle && nFile<filelist.Length-1){
            cycle = false;
            if(nFile+1<filelist.Length) nFile += 1;
            else nFile = 0;
            createMesh();
        }
        else if(cycle) UnityEditor.EditorApplication.isPlaying = false;

    }

    private void tagManagement(string tag){
         // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        string s = tag;

        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(s)) {
                found = true;
                break;
            }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
        }

        tagManager.ApplyModifiedProperties();
    }

    private void createMesh(){
        if(GameObject.Find("container") != null)
        {
            DestroyImmediate(GameObject.Find("container"));
        }
        container = new GameObject("container");

        filename = filelist[0];
        rf = new readfile(filename);
        rf.read();

        for (int i = 0; i < rf.myarray.Length; i+=4)
        {
            createCube(i);
            //Debug.Log(i);
        }

        previousWallSize = wallSize;
        previousWallWidth = wallWidth;
        previousFloorSize = adjustFloorSize;

        GameObject cube_floor_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_floor_center.gameObject.tag = "floor";
        cube_floor_center.transform.parent = container.transform;

        GameObject cube_roof_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_roof_center.gameObject.tag = "roof";
        cube_roof_center.transform.parent = container.transform;

        LoadMaterial(cube_floor_center,"floor");
        LoadMaterial(cube_roof_center,"roof");
    }

    void SetTarget(GameObject cube, Vector3 target)
    {
        Vector3 direction = target - cube.transform.position;
        cube.transform.localScale = new Vector3(1,wallSize,direction.magnitude + 0.5f);
        cube.transform.position = cube.transform.position + (direction / 2);
        cube.transform.LookAt(target);
    }


    void createCube(int index)
    {
        Vector3 A = new Vector3(-rf.myarray[index], 0, rf.myarray[index + 1]);
        Vector3 B = new Vector3(-rf.myarray[index + 2], 0, rf.myarray[index + 3]);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.gameObject.tag = "wall";
        cube.transform.parent = container.transform;
        cube.transform.position = A;
        SetTarget(cube, B);
        LoadMaterial(cube,"wall");
    }

    void LoadMaterial (GameObject obj, string mat)
    {
        Material[] materials = obj.GetComponent<MeshRenderer>().materials;
        if(materials.Length > 0)
        {
            materials[0] = GetComponent<materialLoader>().DicoMat[mat];
            obj.GetComponent<MeshRenderer>().materials = materials;
        }
    }

    void registerPrefab(){
        string[] parts = filename.Split('/');
        string txtname = parts[parts.Length - 1];
        string prefabName = txtname.Split('.')[0];
        string pathname = "Assets/Prefab/prefab_" + prefabName + ".prefab";

        if(!Directory.Exists("Assets/Prefab"))
        {
            Directory.CreateDirectory("Assets/Prefab");
        }

        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("container"),pathname);
        makePrefab = false;
    }

}
