using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Loader
{
    private Dictionary<string, Material> DicoMat;
    // dictionary where textures are associated to names
    private bool finished = false;
    // is the Loader finished ? true / false

    public Loader() {; } // constructor

    public void LoadAll()
    {
        // load everything


        // TAGS
        string[] taglist = { "start", "rotation", "prefab", "wall", "floor", "roof" };
        // List if all existing tags in the project.
        // useful to get specific items

        foreach (string tag in taglist)
        {
            // add tags to the project if they do not exist
            tagManagement(tag);
        }


        // FOLDERS
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
            // create the Resource folder if it does not exist
        }
        if (!Directory.Exists("Assets/Resources/Prefab"))
        {
            Directory.CreateDirectory("Assets/Resources/Prefab");
            // create the Prefab folder if it does not exist
        }
        if (!Directory.Exists("Assets/Resources/Materials"))
        {
            Directory.CreateDirectory("Assets/Resources/Materials");
            // create the MAterials folder if it does not exist

            Debug.Log("Add textures in the folder !");
            UnityEditor.EditorApplication.isPlaying = false;
            // inform that there are no textures and stop the program
        }

        // TEXTURES
        DicoMat = new Dictionary<string, Material>();
        // initialise the dictionary
        InitMaterialList();
        // initialise the dictionary with the textures

        this.finished = true; // the Loader is finished
    }

    public void tagManagement(string tag)
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
    }
    public bool isFinished()
    {
        return finished;
    }


    public void InitMaterialList()
    // add all materials to the dictinary with a tag
    {
        DicoMat.Add("wall", (Material)Resources.Load("Materials/wall_material", typeof(Material)));
        DicoMat.Add("roof", (Material)Resources.Load("Materials/roof_material", typeof(Material)));
        DicoMat.Add("floor", (Material)Resources.Load("Materials/floor_material", typeof(Material)));
        // manual operation
    }

    public Material getMat(string mat)
    {
        return this.DicoMat[mat];
    }

    public bool hasMat(string mat)
    // does the Loader contain a specific material ? true / false
    {
        return DicoMat.ContainsKey(mat);
    }

    public bool mat_is_null(string mat)
    // is the materials null ? true / false
    {
        return getMat(mat) == null;
    }
}
