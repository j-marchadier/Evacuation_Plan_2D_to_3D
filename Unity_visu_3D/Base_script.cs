using UnityEngine;
using UnityEditor;

public class Base_script : MonoBehaviour
{
    Prefab_make pmake;
    Prefab_visu pvisu;

    bool visualizing = false;
    bool making = true;

    // Start is called before the first frame update
    void Start()
    {
        pmake = new Prefab_make();
        pvisu = new Prefab_visu();
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
            pmake.setSwitch(true);
            if(old_making_val != making) pvisu.clear();
            pmake.update_make();
        }
        if(visualizing){
            if(old_visualizing_val != visualizing) pmake.clear();
            pvisu.update_visu();
        }

    }
}
