using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T: ScriptableObject
{
    static Dictionary<string, T> objects;


    public static void Init()
    {
        objects = new Dictionary<string, T>();

        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError($"There are two pokemons with the same name {obj.name}");
                continue;
            }
            objects[obj.name] = obj;
            
        }

    }
    public static T GetObectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"Object with name {name} not found in database.");
            return null;
        }

        return objects[name];
    }
} 


