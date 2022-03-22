using UnityEngine;
using UnityEditor;

public class Base_script : MonoBehaviour
{
    Prefab_make pmake;
    Prefab_visu pvisu;

    GameObject rotation;

    Loader ld;

    bool visualizing = false;
    bool making = true;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        ld = new Loader();
        ld.LoadAll(); // load tout ce qu'il faut loader
        transform.tag = "start"; // change le tag du gameobject de base

        pmake = new Prefab_make(ld);
        pvisu = new Prefab_visu();
        rotation = new GameObject("Rotation");

        GameObject mc = GameObject.FindGameObjectWithTag("MainCamera");
        mc.transform.position = new Vector3(-528, 449, -369); //c'est la position à avoir au début
        rotation.AddComponent<CameraLookAt>();
        rotation.transform.tag = "rotation";
        rotation.GetComponent<CameraLookAt>()._camera = mc;
    }

    private void Update()
    {
        bool old_making_val = making;
        bool old_visualizing_val = visualizing;

        if(Input.GetKeyDown(KeyCode.P)){
            visualizing = false;
            making = true;
        }
        if(Input.GetKeyDown(KeyCode.V)){
            visualizing = true;
            making = false;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if(making){
            if(old_making_val != making){
                pmake.setSwitch(true);
                pvisu.clear();
            }
            pmake.update_make();
            transform.position = new Vector3(-pmake.getMeanX(), 0, pmake.getMeanZ());
        }
        if(visualizing){
            if(old_visualizing_val != visualizing){
                pmake.clear();
                pvisu.charge_prefabs();
            }
            pvisu.update_visu();
            if(!pvisu.prefab_list_is_empty())
            {
                transform.position = new Vector3(-pvisu.visualizedPrefab().transform.position.x, 0, pvisu.visualizedPrefab().transform.position.z);
            }
        }

    }
}
