using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialLoader: MonoBehaviour
{
    [SerializeField]
    public Material wall_material;
    [SerializeField]
    public Material roof_material;
    [SerializeField]
    public Material floor_material;

    public Dictionary<string,Material> DicoMat;

    bool finished;

    public void Start()
    {
        DicoMat = new Dictionary<string,Material>();
        InitMaterialList();

    }

    public bool isFinished(){
        return finished;
    }

    void InitMaterialList(){
        DicoMat.Add("wall",wall_material);
        DicoMat.Add("roof",roof_material);
        DicoMat.Add("floor",floor_material);

        finished = true;
    }


}
