using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class Prefab_visu
{

    GameObject[] prefabList;
    int prefabNum = 0;
    GameObject currentPrefab;

    public Prefab_visu()
    {
        prefabList = Resources.LoadAll<GameObject>("Prefab");

        if(prefabList.Length <= 0)
        {
            Debug.Log("no prefab");
        }

    }

    public void update_visu()
    {
        bool right_cycle = false; // au suivant
        bool left_cycle = false;

        if (prefabList.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                right_cycle = true;

            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                left_cycle = true;

            }
            if (right_cycle)
            {
                right_cycle = false;
                clear();
                prefabNum += 1;
                if(prefabNum >= prefabList.Length)
                {
                    prefabNum = 0;
                }
            }
            if (left_cycle)
            {
                left_cycle = false;
                clear();
                prefabNum -= 1;
                if(prefabNum < 0)
                {
                    prefabNum = prefabList.Length - 1;
                }
            }
            visuPrefab();
        }
    }

    public void visuPrefab()
    {
        if(currentPrefab == null) currentPrefab = GameObject.Instantiate(prefabList[prefabNum]);
    }

    public void clear(){
        GameObject.Destroy(this.currentPrefab);
    }
}
