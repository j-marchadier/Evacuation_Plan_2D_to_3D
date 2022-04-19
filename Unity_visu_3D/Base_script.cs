using UnityEngine;
using UnityEditor;

public class Base_script : MonoBehaviour
{
    Prefab_make pmake;
    // the prefab maker
    Prefab_visu pvisu;
    // the prefab visualizer

    GameObject rotation;
    // the camera carrier object

    bool visualizing = false;
    // we do not start visualizing
    bool making = true;
    // we start making prefabs

    void Start()
    // called at object creation
    {
        Utilities.createDir(Utilities.INPUT_FOLDER_NAME,Utilities.getPath());
        Utilities.createDir(Utilities.OUTPUT_FOLDER_NAME, Utilities.getPath());


        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        // cursor is locked in place and invisible.

        //Utilities.loadAllTags(); // load all the tags
        transform.tag = Utilities.TAG_START; // give a special tag this object

        pmake = new Prefab_make(); // initialize the prefab maker
        pvisu = new Prefab_visu(); // initialize the prefab visualizer
        rotation = Utilities.remakeObject(Utilities.TAG_ROTATION); // make the camera holder

        GameObject mc = GameObject.FindGameObjectWithTag("MainCamera"); // find the main camera
        mc.transform.position = Utilities.camera_position; // define the starting position to have a good viewing angle
        rotation.AddComponent<CameraLookAt>(); // add a component that makes the camera look towards the camera holder
        rotation.GetComponent<CameraLookAt>()._camera = mc; // give the main camera to the camera holder
    }

    private void Update()
    // called every few moments
    {
        bool old_making_val = making; // the old "I am making a prefab" value
        bool old_visualizing_val = visualizing; // the old "I am looking at prefabs" value

        if (Input.GetKeyDown(Utilities.MAKE_PREFAB_MODE)) // if the key is the prefab mode
        {
            visualizing = false; // get out of visualizing mode
            making = true; // get into making mode
        }
        if (Input.GetKeyDown(Utilities.VISU_PREFAB_MODE)) // if the key is the visualization mode
        {
            visualizing = true;// get into visualizing mode
            making = false; // get out of making mode
        }
        if (Input.GetKeyDown(Utilities.QUIT)) // if the key is to close the program
        {
            //UnityEditor.EditorApplication.isPlaying = false; // stop the program
            Application.Quit();
        }
        if(Input.GetKeyDown(Utilities.CAMERA_ROTATION_MODE)){ // if the key is camera rotation mode
            rotation.GetComponent<CameraLookAt>().switchMode();
        }
        if (Input.GetKeyDown(Utilities.CAMERA_INVERT_ROTATION))
        { // if the key is camera invert rotation
            rotation.GetComponent<CameraLookAt>().invertRotation();
        }

        if (making) // if currently in making mode
        {
            if (old_making_val != making) // if we just switched to making mode
            {
                rotation.GetComponent<CameraLookAt>().setMaking(true);
                pmake.setSwitch(true); // tell the prefab maker that we just switched
                pvisu.clear(); // clear the visualizer
            }
            pmake.update_make(); // update the current prefab we are making
            transform.position = new Vector3(-pmake.getMeanX(), 0, pmake.getMeanZ());
            // move the current object to the center of the prefab we are making
        }
        if (visualizing) // if currently in making mode
        {
            if (old_visualizing_val != visualizing) // if we just switched to making mode
            {
                rotation.GetComponent<CameraLookAt>().setMaking(false);
                pmake.clear(); // clear the maker
                pvisu.charge_prefabs();// tell the prefab visualizer to update the list of existing prefabs
            }
            pvisu.update_visu(); // update the current visualization
            if (!pvisu.prefab_list_is_empty())
            // if there are actually some saved prefabs
            {
                transform.position = new Vector3(-pvisu.visualizedPrefab().transform.position.x, 0, pvisu.visualizedPrefab().transform.position.z);
                // move the current object to the center of the prefab

            }
        }

    }
}
