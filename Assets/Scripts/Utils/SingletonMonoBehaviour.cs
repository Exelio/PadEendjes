﻿using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_ShuttingDown = false;

    private static T m_Instance;

    private static readonly object m_Lock = new object();

    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    if (m_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }

                    DontDestroyOnLoad(m_Instance);
                }

                return m_Instance;
            }
        }
    }

    private void OnApplicationQuit() => m_ShuttingDown = true;
    private void OnDestroy() => m_ShuttingDown = true;
}