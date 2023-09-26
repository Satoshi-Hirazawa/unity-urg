using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GetChildrens
{
    // Start is called before the first frame update    
    public static T[] GetComponentsOnlyChildren<T>(this Transform t)
    {
        return t.GetComponentsInChildren<T>().Skip(1).ToArray<T>();
    }

    public static List<GameObject> GetChildGameObject(GameObject parentObj) {
        List<GameObject> childrenObj = new List<GameObject>();
        foreach (Transform child in parentObj.transform) {
            childrenObj.Add(child.gameObject);
        }    
        return childrenObj;
    }
}
