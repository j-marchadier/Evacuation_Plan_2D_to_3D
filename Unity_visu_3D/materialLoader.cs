using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialLoader: MonoBehaviour
{
    // tous les matériaux
    [SerializeField]
    public Material wall_material;
    [SerializeField]
    public Material roof_material;
    [SerializeField]
    public Material floor_material;

    // dictionnaire qui associe les matériaux à des tags
    public Dictionary<string,Material> DicoMat;

    // "le dictionnaire est initailisé - false, true
    bool finished;

    public void Start()
    {
        DicoMat = new Dictionary<string,Material>();
        InitMaterialList();

    }

    // le dico est initialisé
    public bool isFinished(){
        return finished;
    }

    //ajoute les matériaux au dictionnaire avec un tag
    void InitMaterialList(){
        DicoMat.Add("wall",wall_material);
        DicoMat.Add("roof",roof_material);
        DicoMat.Add("floor",floor_material);

        finished = true;
    }


}
