using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[ExecuteInEditMode]
public class test : MonoBehaviour
{

    //creates an array of arrays
    float[] myarray;
    float[] myarrayX;
    float[] myarrayZ;

    GameObject container;

    GameObject cube;

    GameObject cube_floor_center;

    GameObject cube_roof_center;

    GameObject[] walls;

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

    float maxX;
    float minX;
    float maxZ;
    float minZ;

    float meanX;
    float meanZ;

    [SerializeField]
    [Range(0, 60)]
    float adjustFloorSize = 0;

    float previousFloorSize;

    float currentFloorSize;

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("container") != null)
        {
            DestroyImmediate(GameObject.Find("container"));
        }
        container = new GameObject("container");

        ReadString();

        for (int i = 0; i < myarray.Length; i+=4)
        {
            createCube(i);
            //Debug.Log(i);
        }

        previousWallSize = wallSize;
        previousWallWidth = wallWidth;
        previousFloorSize = adjustFloorSize;

        meanX = (minX + maxX) / 2;
        meanZ = (minZ + maxZ) / 2;

        cube_floor_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        cube_floor_center.transform.parent = container.transform;

        cube_roof_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        cube_roof_center.transform.parent = container.transform;

    }

    private void Update()
    {
        currentWallSize = wallSize;
        currentWallWidth = wallWidth;
        currentFloorSize = adjustFloorSize;

        float changeSize = currentWallSize - previousWallSize;
        float changeWidth = currentWallWidth - previousWallWidth;
        float changeFloorSize = currentFloorSize - previousFloorSize;


        walls = GameObject.FindGameObjectsWithTag("wall");
        foreach (GameObject wall in walls)
        {
            wall.transform.localScale = new Vector3(wall.transform.localScale.x + changeWidth, wall.transform.localScale.y + changeSize, wall.transform.localScale.z);
        }

        cube_floor_center.transform.position = new Vector3(meanX, -wallSize / 2, meanZ);
        cube_roof_center.transform.position = new Vector3(meanX, wallSize / 2, meanZ);

        cube_floor_center.transform.localScale = new Vector3(meanX * 2 - adjustFloorSize, 1, meanZ * 2 - adjustFloorSize);
        cube_roof_center.transform.localScale = new Vector3(meanX * 2 - adjustFloorSize, 1, meanZ * 2 - adjustFloorSize);

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;

        Vector3 ground_rect = new Vector3(minX, 0, minZ);
        Vector3 roof_rect = new Vector3(maxX, 1, maxZ);
    }

    void SetTarget(Vector3 target)
    {
        Vector3 direction = target - cube.transform.position;
        cube.transform.localScale = new Vector3(1,wallSize,direction.magnitude + 0.5f);
        cube.transform.position = cube.transform.position + (direction / 2);
        cube.transform.LookAt(target);
    }


    void createCube(int index)
    {
        Vector3 A = new Vector3(myarray[index], 0, myarray[index + 1]);
        Vector3 B = new Vector3(myarray[index + 2], 0, myarray[index + 3]);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.tag = "wall";
        cube.transform.parent = container.transform;
        cube.transform.position = A;
        SetTarget(B);
    }

    void ReadString()
    {
        string path = "Assets/result.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        var fileContents = reader.ReadToEnd();
        reader.Close();
        var lines = fileContents.Split("\n"[0]);
        myarray = new float[lines.Length * 4];
        myarrayX = new float[lines.Length * 2];
        myarrayZ = new float[lines.Length * 2];
        int index = 0;
        foreach(var line in lines)
        {
            var temps = line.Split(';');
            
            foreach(var temp in temps)
            {
                myarray[index] = float.Parse(temp);
                if(index%2 == 0)
                {
                    myarrayX[(int)(index / 2)] = float.Parse(temp);
                }
                else
                {
                    myarrayZ[(int)(index / 2)] = float.Parse(temp);
                }
                index++;
                Debug.Log(temp);
            }
            
        }

        maxX = myarrayX.Max();
        minX = myarrayX.Min();

        maxZ = myarrayZ.Max();
        minZ = myarrayZ.Min();

        Debug.Log(myarray);
    }



}
