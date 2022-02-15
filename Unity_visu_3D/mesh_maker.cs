using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


public class mesh_maker : MonoBehaviour
{
    GameObject container;
    // contient tout les nouveaux objets qui vont former notre mesh

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
    // permet de creer des prefab automatiquement

    [SerializeField]
    bool cycle;
    // permet de passer au fichier de valeurs suivant s'il y a

    readfile rf;
    // permet de lire le fichier actuel
    string[] filelist;
    // liste des fichiers
    string filename;
    // nom fichier actuel

    int nFile;
    // numéro fichier actuel dans la liste

    // Start is called before the first frame update
    void Start()
    {
        string[] taglist = {"wall","floor","roof"};
        // liste de tous les tags existants dans la mesh.
        // utile pour atteindre les objets créés

        foreach(string tag in taglist){
            // ajoute les tags s'ils n'existent pas
            tagManagement(tag);
        }
        nFile = 0;
        // on commence avec le 1er fichier

        filelist = Directory.GetFiles(Application.dataPath + "/", "*.txt");
        // on récupere tous les fichiers présents

        while(!GetComponent<materialLoader>().isFinished());
        // on attend que le dictionnaire des textures ait fini d'initialiser

        createMesh();
        // on cree la premiere mesh
    }

    private void Update()
    {
        currentWallSize = wallSize;
        currentWallWidth = wallWidth;
        currentFloorSize = adjustFloorSize;
        // update la hauteur et largeur des murs et sol/plafond

        float changeSize = currentWallSize - previousWallSize;
        float changeWidth = currentWallWidth - previousWallWidth;
        float changeFloorSize = currentFloorSize - previousFloorSize;
        // défini la quantité par laquelle on doit changer ces valeurs


        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        // récupere tous les murs
        foreach (GameObject wall in walls)
        {
            wall.transform.localScale = new Vector3(wall.transform.localScale.x + changeWidth, wall.transform.localScale.y + changeSize, wall.transform.localScale.z);
            // change la hauteur/largeur des murs
        }

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        // récupere les sols
        foreach (GameObject floor in floors)
        {
            floor.transform.position = new Vector3(-rf.meanX, -wallSize / 2, rf.meanZ);
            floor.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
            // change la longueur du sol
        }

        GameObject[] roofs = GameObject.FindGameObjectsWithTag("roof");
        // récupere les plafonds
        foreach (GameObject roof in roofs){
            roof.transform.position = new Vector3(-rf.meanX, wallSize / 2, rf.meanZ);
            roof.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
            // change la longueur du plafond
        }

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;
        // update les valeurs précédentes

        if(makePrefab) {
            registerPrefab();
            // fait un préfab de l'objet actuel
        }

        if(cycle && nFile<filelist.Length-1){
            cycle = false;
            if(nFile+1<filelist.Length) nFile += 1;
            else nFile = 0;
            createMesh();
            // passe au fichier suivant dans la liste et créé le mesh correspondant
        }
        else if(cycle) UnityEditor.EditorApplication.isPlaying = false;
        // si il n'y a pas d'autre fichier, ferme le programme
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

    private void createMesh(){
        // créé le mesh pour le fichier actuel
        if(GameObject.Find("container") != null)
        {
            DestroyImmediate(GameObject.Find("container"));
            // si il existe un objet "container", détruit le
        }
        container = new GameObject("container");
        // créé un conteneur pour nos murs et plafonds et tout

        filename = filelist[0];
        rf = new readfile(filename);
        rf.read();
        // lis le fichier actuel

        for (int i = 0; i < rf.myarray.Length; i+=4)
        {
            createCube(i);
            // créé autant de murs que nécessaire
        }

        previousWallSize = wallSize;
        previousWallWidth = wallWidth;
        previousFloorSize = adjustFloorSize;
        // update les valeurs précédentes de taille et épaisseur des murs et plafond/sol

        GameObject cube_floor_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_floor_center.gameObject.tag = "floor";
        cube_floor_center.transform.parent = container.transform;
        // créé le sol et met le dans le conteneur

        GameObject cube_roof_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_roof_center.gameObject.tag = "roof";
        cube_roof_center.transform.parent = container.transform;
        // créé le plafond et met le dans le conteneur

        LoadMaterial(cube_floor_center,"floor");
        LoadMaterial(cube_roof_center,"roof");
        // donne une texture au sol et au plafond
    }

    void SetTarget(GameObject cube, Vector3 target)
    {
        Vector3 direction = target - cube.transform.position;
        cube.transform.localScale = new Vector3(1,wallSize,direction.magnitude + 0.5f);
        cube.transform.position = cube.transform.position + (direction / 2);
        cube.transform.LookAt(target);
        // orient le mur dans la bonne direction
    }


    void createCube(int index)
    {
        // créé un mur et donne lui une texture.
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
        // donne une texture a un objet
        Material[] materials = obj.GetComponent<MeshRenderer>().materials;
        if(materials.Length > 0)
        {
            materials[0] = GetComponent<materialLoader>().DicoMat[mat];
            // récupere la texture correspondante au tag demandé
            obj.GetComponent<MeshRenderer>().materials = materials;
            // applique la texture
        }
    }

    void registerPrefab(){
        string[] parts = filename.Split('/');
        string txtname = parts[parts.Length - 1];
        string prefabName = txtname.Split('.')[0];
        string pathname = "Assets/Prefab/prefab_" + prefabName + ".prefab";
        //créé un nom pour la préfab par rapport au nom du fichier correspondant

        if(!Directory.Exists("Assets/Prefab"))
        {
            Directory.CreateDirectory("Assets/Prefab");
            // créé un dossier pour stocker la préfab si besoin
        }

        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("container"),pathname);
        // sauvegarde un préfab de l'objet actuel
        makePrefab = false;
        // makeprefab, comme cycle, sert comme un bouton. Il se met directement a "false" apres avoir effectué son opération
    }

}
