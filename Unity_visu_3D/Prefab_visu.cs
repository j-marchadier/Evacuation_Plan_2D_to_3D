using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class Prefab_visu
{

    GameObject[] prefabList;
    // list of all saved prefab object
    int prefabNum = 0;
    // numebr of the current prefab we are looking at
    GameObject currentPrefab;
    // the current prefab we are looking at


    public Prefab_visu()
    // visualize the prefab
    {
        charge_prefabs();
        // charge all currently saved prefabs

    }

    public void update_visu()
    // update the visualization
    {
        bool right_cycle = false; // next to the right
        bool left_cycle = false; // next to the left

        if (prefabList.Length > 0)
        // if we do have some saved prefabs
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            // move to the right with the rigt arrow
            {
                right_cycle = true;

            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            // move to the left with the left arrow
            {
                left_cycle = true;

            }
            if (right_cycle)
            // if we are moving to the right
            {
                right_cycle = false; // we are not moving to the right anymore
                clear(); // clear the current visualization
                prefabNum += 1; // move one prefab to the right in the list
                if (prefabNum >= prefabList.Length)
                {
                    prefabNum = 0; // if we go to far, go back the start of the list
                }
            }
            if (left_cycle)
            // if we are moving to the left
            {
                left_cycle = false;// we are not moving to the left anymore
                clear(); // clear the current visualization
                prefabNum -= 1;  // move one prefab to the left in the list
                if (prefabNum < 0)
                {
                    prefabNum = prefabList.Length - 1; // if we go to far, go back the end of the list
                }
            }
            visuPrefab(); // visualize the current prefab
        }
    }

    public void visuPrefab()
    // visualize the current prefab
    {
        if (currentPrefab == null) // if we have a target to visualize
        {
            currentPrefab = GameObject.Instantiate(prefabList[prefabNum]); // get the object from the list and instantiate it
            currentPrefab.gameObject.tag = "prefab"; // give it a special tag
        }
    }

    public void clear()
    // clear the visualization
    {
        GameObject.Destroy(this.currentPrefab); // destroy the current obeject
    }

    public void charge_prefabs()
    // charge all existing prefabs
    {
        prefabList = Resources.LoadAll<GameObject>("Prefab"); // search for and load all prefab files

        if (prefabList.Length <= 0)
        // if there are no prefabs
        {
            Debug.Log("No prefab to visualize");
        }
    }

    public bool prefab_list_is_empty()
    {
        return prefabList.Length <= 0; // is the prefab list empty ?
    }

    public GameObject visualizedPrefab()
    {
        return currentPrefab; // return the currently visualized prefab
    }
}
