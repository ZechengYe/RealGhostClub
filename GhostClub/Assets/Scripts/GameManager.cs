using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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

    public GameObject UIButtonLeft;
    public GameObject UIButtonRight;

    public GameObject UIButtonMain; // Triangle, Circle, X, Square
    
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
    
    // Buttons for UI
    [FormerlySerializedAs("buttonL2")] public Sprite buttonL1;
    [FormerlySerializedAs("buttonR2")] public Sprite buttonR1;

    public Sprite buttonLeft;
    public Sprite buttonRight;
    
    public Sprite buttonTriangle;
    public Sprite buttonCircle;
    public Sprite buttonX;
    public Sprite buttonSquare;
    
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

        UIButtonMain = GameObject.Find("ButtonMain");
        
        UIRoot = GameObject.Find("UIRoot");
        UIMain = GameObject.Find("UIMain");
        UISkillA = GameObject.Find("SkillA");
        UISkillB = GameObject.Find("SkillB");

        UIButtonLeft = GameObject.Find("ButtonL");
        UIButtonRight = GameObject.Find("ButtonR");
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
        // Controlling ON/OFF of UI slots 
        switch (turnStatus)
        { 
            case TurnStatus.HostTurn or 
                 TurnStatus.SummonTurn or 
                 TurnStatus.DirectorTurn or 
                 TurnStatus.SoundTurn or 
                 TurnStatus.InternTurn or 
                 TurnStatus.CameraTurn:
                
                UIMain.SetActive(true);
                UISkillA.SetActive(false);
                UISkillB.SetActive(false);

                UIButtonLeft.GetComponent<Image>().sprite = buttonL1;
                UIButtonRight.GetComponent<Image>().sprite = buttonR1;
                
                break;
            case TurnStatus.HostBranch or 
                 TurnStatus.SummonBranch or 
                 TurnStatus.DirectorBranch or 
                 TurnStatus.SoundBranch or 
                 TurnStatus.InternBranch or 
                 TurnStatus.CameraBranch:
                
                UIMain.SetActive(false);
                UISkillA.SetActive(true);
                UISkillB.SetActive(true);
                
                UIButtonLeft.GetComponent<Image>().sprite = buttonLeft;
                UIButtonRight.GetComponent<Image>().sprite = buttonRight;
                
                break;
            
            default:
                UIMain.SetActive(false);
                UISkillA.SetActive(false);
                UISkillB.SetActive(false);
                break;
        }
        
        // Changing sprites for UI slots
        switch (turnStatus)
        {
            case TurnStatus.HostTurn or TurnStatus.HostBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(250, 310);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonL1;
                
                UIMain.GetComponent<Image>().sprite = hostMain;
                UISkillA.GetComponent<Image>().sprite = hostSkillA;
                UISkillB.GetComponent<Image>().sprite = hostSkillB;
                break;
            
            case TurnStatus.SummonTurn or TurnStatus.SummonBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(110, 250);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonR1;
                
                UIMain.GetComponent<Image>().sprite = summonMain;
                UISkillA.GetComponent<Image>().sprite = summonSkillA;
                UISkillB.GetComponent<Image>().sprite = summonSkillB;
                break;
            
            case TurnStatus.DirectorTurn or TurnStatus.DirectorBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-580, 340);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonTriangle;
                
                UIMain.GetComponent<Image>().sprite = directorMain;
                UISkillB.GetComponent<Image>().sprite = directorSkillA;
                UISkillA.GetComponent<Image>().sprite = questionMark;
                break;
            
            case TurnStatus.SoundTurn or TurnStatus.SoundBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-210, 330);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonCircle;
                
                UIMain.GetComponent<Image>().sprite = soundMain;
                UISkillB.GetComponent<Image>().sprite = soundSkillA;
                UISkillA.GetComponent<Image>().sprite = questionMark;
                break;
            
            case TurnStatus.InternTurn or TurnStatus.InternBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-785, 280);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonX;
                
                UIMain.GetComponent<Image>().sprite = internMain;
                UISkillB.GetComponent<Image>().sprite = interSkillA;
                UISkillA.GetComponent<Image>().sprite = questionMark;
                break;
            
            // NOTE: ICONS FOR CAMERA GUY MIGHT NEED TO BE MODIFIED BASED ON THE POS OF INSPIRATION BAR
            case TurnStatus.CameraTurn or TurnStatus.CameraBranch:
                // Position
                UIRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-380, 310);
                
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = buttonSquare;
                
                UIMain.GetComponent<Image>().sprite = cameraMain;
                UISkillB.GetComponent<Image>().sprite = cameraSkillA;
                UISkillA.GetComponent<Image>().sprite = questionMark;
                break;
            
            default:
                // Sprites
                UIButtonMain.GetComponent<Image>().sprite = null;
                
                UIMain.GetComponent<Image>().sprite = null;
                UISkillA.GetComponent<Image>().sprite = null;
                UISkillB.GetComponent<Image>().sprite = null;
                break;
        }
    }
}
