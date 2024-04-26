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
    public GameObject sound;
    
    // For displaying Button-mapping 
    public GameObject buttonMapping;

    public bool isStarted = false;

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
        
        // Button Mapping 
        buttonMapping = GameObject.Find("ControlMaps");

        isStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Wiring the characters' GameObjects
        SetUpCharacters();
        
        // Button Mapping
        ButtonMapping();
    }

    private void SetUpCharacters()
    {
        // Wiring the characters' GameObjects
        boss = GameObject.FindGameObjectWithTag("BossUnit");
        host = GameObject.FindGameObjectWithTag("HostUnit");
        summon = GameObject.FindGameObjectWithTag("SummonUnit");
        sound = GameObject.FindGameObjectWithTag("SoundUnit");
    }

    private void ButtonMapping()
    {
        // At the beginning of the game, show the interface
        if (!isStarted)
        {
            buttonMapping.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                isStarted = true;
            }
        }
        // During the gameplay, hold the key to show the interface temporarily 
        else
        {
            if (Input.GetKey(KeyCode.F))
            {
                buttonMapping.SetActive(true);
            }
            else
            {
                buttonMapping.SetActive(false);
            }
        }
    }
}
