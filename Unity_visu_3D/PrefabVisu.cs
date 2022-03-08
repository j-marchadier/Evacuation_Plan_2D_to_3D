using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class PrefabVisu : MonoBehaviour
{

    GameObject[] prefabList;
    bool listEmpty = false;
    int prefabNum = 0;
    bool cycle = false;
    GameObject currentPrefab;
    // Start is called before the first frame update
    void Start()
    {
         prefabList = Resources.LoadAll<GameObject>("Prefab");
        
        if(prefabList.Length <= 0) 
        {
            listEmpty = true;
            Debug.Log("no prefab");
        }
        else 
        {
            /*foreach(string name in prefabNameList)
            {
                Debug.Log(name);
                GameObject temp = new GameObject();
                temp = Resources;
                temp.GetComponent<MeshRenderer>().enabled = false;
                prefabList.Add(temp);
            }*/
            visuPrefab();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (!listEmpty)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                cycle = true;

            }
            if (cycle)
            {
                cycle = false;
                Destroy(currentPrefab);
                prefabNum += 1;
                if(prefabNum >= prefabList.Length)
                {
                    prefabNum = 0;
                }
                visuPrefab();
            }
            

        }
    }

    private void visuPrefab()
    {
        currentPrefab = Instantiate(prefabList[prefabNum]);
    }
}
