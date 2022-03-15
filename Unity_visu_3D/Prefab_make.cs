using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class Prefab_make
{
    GameObject container;
    // contient tout les nouveaux objets qui vont former notre mesh

    public bool hide_roof;
    bool old_hide_value;

    float wallSize;

    float previousWallSize;
    float currentWallSize;

    float wallWidth;

    float previousWallWidth;
    float currentWallWidth;

    float adjustFloorSize;

    float previousFloorSize;
    float currentFloorSize;


    readfile rf;
    // permet de lire le fichier actuel

    Loader ld;
    // fait toutes les initialisations dont on a besoin
    string[] filelist_walls;
    // liste des fichiers lisibles
    string filename;
    // nom fichier actuel

    char fileTag;
    // le tag du fichier (0, 1, 2, ...)

    string logofile;

    int nFile;
    // numéro fichier actuel dans la liste

    bool just_switched;


    // Start is called before the first frame update
    public Prefab_make()
    {

        nFile = 0;
        // on commence avec le 1er fichier

        filelist_walls = Directory.GetFiles(Application.dataPath + "/", "*_mur.txt");
        // on récupere tous les fichiers présents contenant des murs
        if(filelist_walls.Length <= 0){
            Debug.Log("No file to read ! Add files to Assets/");
            UnityEditor.EditorApplication.isPlaying = false;
        }

        wallSize = 100;
        previousWallSize = wallSize;
        wallWidth = 6.35f;
        previousWallWidth = wallWidth;
        adjustFloorSize = 52;
        previousFloorSize = adjustFloorSize;

        ld = new Loader();

        while(!ld.isFinished()) Debug.Log("Waiting for textures...");
        // on attend que le dictionnaire des textures ait fini d'initialiser

        old_hide_value = hide_roof;

        createMesh();
        // on cree la premiere mesh
    }

    public void update_make()
    {
        if(just_switched){
            createMesh();
            just_switched = false;
        }
        bool right_cycle = false; // passe au suivant
        bool left_cycle = false;
        bool makePrefab = false; // fait un prefab

        if (Input.GetKeyDown(KeyCode.Return)) makePrefab = true;
        if(Input.GetKeyDown(KeyCode.RightArrow)) right_cycle = true;
        if(Input.GetKeyDown(KeyCode.LeftArrow)) left_cycle = true;

        if (Input.GetKeyDown(KeyCode.H)) hide_roof = !hide_roof; // hide roof button

        if (Input.GetKey(KeyCode.A)) {
            if (wallSize >= 1) wallSize-=0.1f;
        }
        else if (Input.GetKey(KeyCode.Z)) {
            if (wallSize <= 100) wallSize+=0.1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (wallWidth >= 1) wallWidth-=0.1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (wallWidth <= 10) wallWidth+=0.1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (adjustFloorSize >= 0) adjustFloorSize-=0.1f;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            if (adjustFloorSize <= 60) adjustFloorSize+=0.1f;
        }

        if (right_cycle)
        {
            right_cycle = false;
            nFile += 1;
            if(nFile >= filelist_walls.Length)
            {
                nFile = 0;
            }
            createMesh();
        }
        if (left_cycle)
        {
            left_cycle = false;
            nFile -= 1;
            if(nFile < 0)
            {
                nFile = filelist_walls.Length - 1;
            }
            createMesh();
        }
        if(makePrefab){
            registerPrefab();
        }

        updateMesh();
    }

    private void updateMesh(){
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
            if(old_hide_value != hide_roof){
                if(hide_roof) floor.GetComponent<MeshRenderer>().enabled = false;
                else floor.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        GameObject[] roofs = GameObject.FindGameObjectsWithTag("roof");
        // récupere les plafonds
        foreach (GameObject roof in roofs){
            roof.transform.position = new Vector3(-rf.meanX, wallSize / 2, rf.meanZ);
            roof.transform.localScale = new Vector3(rf.meanX * 2 - adjustFloorSize, 1, rf.meanZ * 2 - adjustFloorSize);
            // change la longueur du plafond
            if(old_hide_value != hide_roof){
                if(hide_roof) roof.GetComponent<MeshRenderer>().enabled = false;
                else roof.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        old_hide_value = hide_roof;

        previousWallSize = currentWallSize;
        previousWallWidth = currentWallWidth;
        previousFloorSize = currentFloorSize;
        // update les valeurs précédentes

    }

    private void createMesh(){
        // créé le mesh pour le fichier actuel
        if(GameObject.Find("container") != null)
        {
            GameObject.DestroyImmediate(GameObject.Find("container"));
            // si il existe un objet "container", détruit le
        }
        container = new GameObject("container");
        // créé un conteneur pour nos murs et plafonds et tout

        filename = filelist_walls[nFile];
        // nom du fichier

        string cutname = filename.Split()[filename.Split().Length - 1];
        string justname = cutname.Split()[0];
        fileTag = justname[0];
        // recupere le Tag de ce fichier

        rf = new readfile(filename,"walls");
        rf.read();
        // lis le fichier actuel

        string[] filelist_logos = Directory.GetFiles(Application.dataPath + "/", fileTag + "_logo.txt");
        if(filelist_logos.Length>0) logofile = filelist_logos[0];



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
        if (hide_roof)
        {
            cube_floor_center.GetComponent<MeshRenderer>().enabled = false;
        }
        // créé le sol et met le dans le conteneur

        GameObject cube_roof_center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_roof_center.gameObject.tag = "roof";
        cube_roof_center.transform.parent = container.transform;
        if (hide_roof)
        {
            cube_roof_center.GetComponent<MeshRenderer>().enabled = false;
        }
        // créé le plafond et met le dans le conteneur

        LoadMaterial(cube_floor_center,"floor");
        LoadMaterial(cube_roof_center,"roof");
        // donne une texture au sol et au plafond

    }

    void SetTarget(GameObject cube, Vector3 target)
    {
        Vector3 direction = target - cube.transform.position;
        cube.transform.localScale = new Vector3(wallWidth, wallSize, direction.magnitude + 0.5f);
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

        if(materials.Length > 0 && ld.hasMat(mat))
        {
            if(!ld.mat_is_null(mat))
            {
                materials[0] = ld.getMat(mat);
                // récupere la texture correspondante au tag demandé
                obj.GetComponent<MeshRenderer>().materials = materials;
                // applique la texture
            }
        }
    }

    void registerPrefab(){
        string[] parts = filename.Split('/');
        string txtname = parts[parts.Length - 1];
        string prefabName = txtname.Split('.')[0];
        string pathname = "Assets/Resources/Prefab/prefab_" + prefabName + ".prefab";
        //créé un nom pour la préfab par rapport au nom du fichier correspondant

        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("container"),pathname);
        // sauvegarde un préfab de l'objet actuel
    }

    public void setSwitch(bool b){
        this.just_switched = b;
    }

    public void clear(){
        GameObject.Destroy(container);
    }
}
