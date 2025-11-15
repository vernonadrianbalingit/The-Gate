using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabDetection : MonoBehaviour
{
    // Trying to make a script to be able to change prefab (for different towers)
    //Going to try using tags but Not sure if going to work
    //Need to find a more efficient way

    //private GameObject[] objects;
    private List<GameObject> objectsList;
    private string objType;
    private string[] guids;
    

    //creates array of objects that share a tag
    void Start()
    {
        if (objType != null)
        {
            objectsList = new List<GameObject>(GameObject.FindGameObjectsWithTag(objType));
        }
        else
        {
            objectsList = new List<GameObject>();

            guids = AssetDatabase.FindAssets("t:Object", new[] { "Assets/Master/Assets/MOBA and Tower Defense/Prefabs_Turrets" });

            foreach (string guid in guids)
            {

                string objectPath = AssetDatabase.GUIDToAssetPath(guid);
                Object[] objects  = AssetDatabase.LoadAllAssetsAtPath(objectPath);

                foreach (Object obj in objects)
                {

                    if (obj is GameObject)
                    {
                        if (((GameObject)obj).GetComponent<Animator>())
                        {
                            //Debug.Log(obj.name);

                            objectsList.Add((GameObject)obj);
                                
                        }
                        
                    }
                }
            }
        }
        //printObjects();
        //changeFolder("Assets/Master/Assets/Monster Bee/Prefabs");
        //printObjects();
    }

    //changes the folder to check
    public void changeFolder(string path)
    {
        guids = AssetDatabase.FindAssets("t:Object", new[] { path });
        updateObjects();
    }


    //updates the objectList
    public void updateObjects()
    {
        objectsList.Clear();


        foreach (string guid in guids)
        {

            string objectPath = AssetDatabase.GUIDToAssetPath(guid);
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(objectPath);

            foreach (Object obj in objects)
            {

                if (obj is GameObject)
                {
                    if (((GameObject)obj).GetComponent<Animator>())
                    {
                        //Debug.Log(obj.name);

                        objectsList.Add((GameObject)obj);

                    }

                }
            }
        }
    }

    //prints list of objects 
    public void printObjects()
    {
        Debug.LogWarning("The number of objects are: " + objectsList.Count);
        for(var i = 0; i < objectsList.Count; i++)
        {
            Debug.Log(objectsList[i]);
        }
    }

    //access certain objects when scripted
    public GameObject getObject(int i)
    {
        if(i < objectsList.Count)
        {
            return objectsList[i];
        } 
        
        else
        {
            return null;
        }
    }

}
