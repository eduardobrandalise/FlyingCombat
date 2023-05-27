using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Branda.Utils;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] private Transform enemySpawner;
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }
}
