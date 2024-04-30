using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

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
    
    // For setting up UI
    public Canvas turnDisplay;
    
    public GameObject UIRoot; // The GameObject with Horizontal Layout Group
    public GameObject UIMain; // Indicating whose turn it is
    public GameObject UISkillA; // Skill A of the branch
    public GameObject UISkillB; // Skill B of the branch
    
    // Loading UI sprites from resource folder
    public Sprite hostMain;
    public Sprite hostSkillA;
    public Sprite hostSkillB;
    
    public Sprite summonMain;
    public Sprite summonSkillA;
    public Sprite summonSkillB;

    public Sprite directorMain;
    public Sprite directorSkillA;
    
    public Sprite soundMain;
    public Sprite soundSkillA;

    public Sprite internMain;
    public Sprite interSkillA;
    
    public Sprite cameraMain;
    public Sprite cameraSkillA;
    
    public Sprite questionMark;
    
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
        
        // Set Up
        turnDisplay = GameObject.Find("TurnIndicators").GetComponent<Canvas>();
        
        UIRoot = GameObject.Find("UIRoot");
        UIMain = GameObject.Find("UIMain");
        UISkillA = GameObject.Find("SkillA");
        UISkillB = GameObject.Find("SkillB");
    }

    // Update is called once per frame
    void Update()
    {
        // Wiring the characters' GameObjects
        SetUpCharacters();
        
        // Button Mapping
        ButtonMapping();

        // Switching UI display
        TurnUIDisplay();
        
        //UIMain.GetComponent<Image>().sprite = soundMain;
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
            turnDisplay.enabled = false;

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
            
            turnDisplay.enabled = true;
        }
    }

    public void TurnUIDisplay()
    {
        switch (turnStatus)
        { 
            case TurnStatus.HostTurn or 
                 TurnStatus.SummonTurn or 
                 TurnStatus.DirectorTurn or 
                 TurnStatus.SoundTurn or 
                 TurnStatus.InternTurn or 
                 TurnStatus.CameraTurn:
                
                UIMain.SetActive(true);
                instance.UISkillA.SetActive(false);
                instance.UISkillB.SetActive(false);
                
                break;
            case TurnStatus.HostBranch or 
                 TurnStatus.SummonBranch or 
                 TurnStatus.DirectorBranch or 
                 TurnStatus.SoundBranch or 
                 TurnStatus.InternBranch or 
                 TurnStatus.CameraBranch:
                
                UIMain.SetActive(false);
                instance.UISkillA.SetActive(true);
                instance.UISkillB.SetActive(true);
                
                break;
            
            default:
                UIMain.SetActive(false);
                instance.UISkillA.SetActive(false);
                instance.UISkillB.SetActive(false);
                break;
        }
    }
}
