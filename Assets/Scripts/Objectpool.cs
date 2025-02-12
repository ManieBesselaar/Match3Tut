using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Objectpool : MonoBehaviour
{
    public static Objectpool instance;
   List<GameObject> pooledObjects = new ();
   [SerializeField] GameObject _objectPrefab;
    int poolSize = 10;

    public static string GetAgeCategory(int age)
    {
        return age switch
        {
            < 2 => "infant",
            >= 2 and < 5 => "toddler",
            >= 5 and < 10 => "youngster",
            >= 10 and < 13 => "pre-teen",
            >= 13 and < 20 => "teenager",
            >= 20 => "adult"
        };
    }

    private void Awake()
    {
        if (instance == null)
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(_objectPrefab, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            pooledObjects[i] = obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
