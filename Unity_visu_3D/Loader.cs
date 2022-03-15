using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Loader
{
    // Start is called before the first frame update
    private Dictionary<string,Material> DicoMat;
    private bool finished = false;

    public Loader()
    {
        // TAGS
        string[] taglist = {"wall","floor","roof"};
        // liste de tous les tags existants dans la mesh.
        // utile pour atteindre les objets créés

        foreach(string tag in taglist){
            // ajoute les tags s'ils n'existent pas
            tagManagement(tag);
        }


        // FOLDERS
        if(!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
            // créé un dossier pour stocker nos resources si besoin
        }
        if (!Directory.Exists("Assets/Resources/Prefab"))
        {
            Directory.CreateDirectory("Assets/Resources/Prefab");
            // créé un dossier pour stocker les materiaux si besoin
        }
        if (!Directory.Exists("Assets/Resources/Materials"))
        {
            Directory.CreateDirectory("Assets/Resources/Materials");
            // créé un dossier pour stocker les materiaux si besoin

            Debug.Log("Add textures in the folder !");
            UnityEditor.EditorApplication.isPlaying = false;
        }

        // TEXTURES
        DicoMat = new Dictionary<string,Material>();
        InitMaterialList();

        this.finished = true;
    }

    public void tagManagement(string tag){
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
                // si le tag est déja dans la liste, ne fait rien
            }
        }

        if (!found)
        {
            // si le tag n'est pas dans la liste, on l'ajoute
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
        }

        tagManager.ApplyModifiedProperties();
        // update les tags
    }
    public bool isFinished(){
        return finished;
    }

    //ajoute les matériaux au dictionnaire avec un tag
    public void InitMaterialList(){
        DicoMat.Add("wall",(Material)Resources.Load("Materials/wall_material", typeof(Material)));
        DicoMat.Add("roof",(Material)Resources.Load("Materials/roof_material", typeof(Material)));
        DicoMat.Add("floor",(Material)Resources.Load("Materials/floor_material", typeof(Material)));

    }

    public Material getMat(string mat){
        return this.DicoMat[mat];
    }

    public bool hasMat(string mat){
        return DicoMat.ContainsKey(mat);
    }

    public bool mat_is_null(string mat){
        return getMat(mat) == null;
    }
}
