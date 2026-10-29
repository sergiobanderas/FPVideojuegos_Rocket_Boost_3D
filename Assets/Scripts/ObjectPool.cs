using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int poolSize = 10;

    private List<GameObject> objectPoolList = new List<GameObject>();

    void Start()
    {
        CreatePool();
    }


    /// <summary>
    /// Create a pool of objects based on the specified pool size and object prefab
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }
    
    /// <summary>
    /// Instantiate new Object and add it to the pool list
    /// </summary>
    /// <returns>GameObject</returns>
    private GameObject CreateNewObject()
    {
        // Instantiate a new object from the prefab
        GameObject newObject = Instantiate(objectPrefab);
        // Set the new object to inactive and add it to the pool list
        newObject.SetActive(false);
        // Add the new object to the pool list
        objectPoolList.Add(newObject);
        //return the new object
        return newObject;     
    }

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject GetObjectFromPool()
    {
        // Find an inactive object in the pool list
        GameObject theObject = objectPoolList.Find(obj => !obj.activeInHierarchy);

        if (theObject == null)
        {
            theObject = CreateNewObject();
        }

        theObject.SetActive(true);
        return theObject;
    }
    

}
