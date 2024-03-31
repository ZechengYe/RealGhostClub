using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject boss;
    public GameObject host;
    public GameObject summon;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            // Don't destroy the holder
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SetUpCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        // Wiring the characters' GameObjects
        SetUpCharacters();
    }

    private void SetUpCharacters()
    {
        // Wiring the characters' GameObjects
        boss = GameObject.FindGameObjectWithTag("BossUnit");
        host = GameObject.FindGameObjectWithTag("HostUnit");
        summon = GameObject.FindGameObjectWithTag("SummonUnit");
    }
}
