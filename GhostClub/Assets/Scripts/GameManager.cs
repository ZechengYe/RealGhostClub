using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Defining enum for counting status of the turns
public enum TurnStatus
{
    Start,
    HostTurn, HostBranch,
    SummonTurn, SummonBranch,
    DirectorTurn, DirectorBranch,
    SoundTurn, SoundBranch,
    CameraTurn, CameraBranch,
    InternTurn, InternBranch,
    BossTurn
}

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;
    
    // For wiring up GameObjects in the scene
    public GameObject boss;
    public GameObject host;
    public GameObject summon;
    public GameObject sound;
    
    // For displaying Button-mapping 
    public GameObject buttonMapping;
    
    // For recording the turn state
    public TurnStatus turnStatus = TurnStatus.Start;

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

        // Show the Tutorial of button mapping at the beginning of the game
        isStarted = false;
        
        // Resetting turn status
        turnStatus = TurnStatus.Start;
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

            if (Input.GetKeyDown(KeyCode.F) || 
                Gamepad.all[0].startButton.isPressed)
            {
                isStarted = true;
            }
        }
        // During the gameplay, hold the key to show the interface temporarily 
        else
        {
            if (Input.GetKey(KeyCode.F) || 
                Gamepad.all[0].startButton.isPressed)
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
