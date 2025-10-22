using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Mono_Method<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T d_Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindAnyObjectByType<T>();
            }
            return _Instance;
        }
        set
        {
            _Instance = value;
        }
    }
    static T _Instance = default(T);
}