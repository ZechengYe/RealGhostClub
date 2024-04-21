using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem; 

public enum BattleState 
{
    START,
    HOSTTURN, HOSTMENU,
    SUMMONTURN, SUMMONMENU,
    DIRECTORTURN, DIRECTORMENU,
    SOUNDTURN, SOUNDMENU,
    CAMERATURN, CAMERAMENU,
    INTERNTURN, INTERNMENU,
    BOSSTURN,
    WON, LOST
}

public class BattleSystem : MonoBehaviour
{
    #region Declaration of variables
    public bool DamageMonitorOn = false;
    public BattleState state;

    public GameObject hostPrefab;
    public GameObject summonPrefab;
    public GameObject directorPrefab;
    public GameObject soundPrefab;
    public GameObject cameraPrefab;
    public GameObject internPrefab;
    public GameObject bossPrefab;
    
    //we can seperate the slider code later?
    public Slider SoundInspiration;
    
    //health sliders
    
    public Slider bossHealth01;
    public Slider bossHealth02;
    public Slider bossHealth03;
    public Slider bossHealth04;
    
    public Slider bossSpirit01;
    public Slider bossSpirit02;
    public Slider bossSpirit03;
    public Slider bossSpirit04;

    public Slider TeamHealth;
    
    public Transform hostBattleStation;
    public Transform summonBattleStation;
    public Transform directorBattleStation;
    public Transform soundBattleStation;
    public Transform cameraBattleStation;
    public Transform internBattleStation;
    public Transform bossBattleStation;

    Unit hostUnit;
    Unit summonUnit;
    Unit directorUnit;
    Unit soundUnit;
    Unit cameraUnit;
    Unit internUnit;
    Unit bossUnit;
    
    //Cameras for Cameraman
    public CinemachineVirtualCamera vCam1;
    public CinemachineVirtualCamera vCam2;
    bool vCam1On = true;
    
    //Music for Soundguy
    public AudioSource MusicSource;//The AudioSource component for playing music
    public List<AudioClip> songs = new List<AudioClip>(); // List of songs to play

    public float fadeTime = 0.05f; //Duration for the fade effect, Ezreal made it 0.05
    
    private int currentSongIndex = -1; //Keeps track of the currently playing song
    public int songPlaying = 0;
    
    bool inspirationFull = false;
    private bool isGamePaused = false;
    #endregion
    private bool isTurnStart = true;
    private bool canUseAlpha1 = true; 

    //I put the shared health management system here instead of unit, so it's more manageable
    public int teamphysicalHP = 1000;
    public int maxTeamPhysicalHP = 1000;
    public int teamMagicalHP = 500;
    public int maxTeamMagicalHP = 500;
    
    private string uiMessage; 
    
    //testing
    public TextMeshProUGUI testingPrompts;
    public bool TakeSharedPhysicalDamage(int physicalDmg)
    {
        teamphysicalHP -= physicalDmg;
        if (teamphysicalHP <= 0)
        {
            teamphysicalHP = 0;
            EndBattle();
            return true; // Indicates death
        }
        return false;
    }
    public bool TakeSharedMagicalDamage(int magicalDmg)
    {
        teamMagicalHP -= magicalDmg;
        if (teamMagicalHP <= 0)
        {
            teamMagicalHP = 0;
            EndBattle();
            return true; // Indicates death
        }
        return false;
    }
    
    void Start()
    {
        //this function show the name of all connected controllers
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i].name);
        }

        //set game state
        state = BattleState.START;
        Setup();
        
        // Optionally, start playing the initial song here
        if (songs.Count > 0)
        {
            ChangeSong(0); // Start with the first song
        }
        
    }

    void Update()
    {
        switch (state)
        {
            //connect everyone's turn and branch
            //add actual damage monitor and function into new state machine
            case BattleState.HOSTTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Host's turn, Press Q/leftShoulder to open skill tree");
                    isTurnStart = false;
                }

                if(Input.GetKeyDown(KeyCode.Q) || Gamepad.all[0].leftShoulder.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;
                
            case BattleState.HOSTMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Host Press 1/left to use skill 1, Press 2/right to use skill2");
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    HostSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    HostSkill_B();
                }
                break;

            case BattleState.SUMMONTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Summon's turn, Press E/rightshoulder to open skill tree");
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.E) || Gamepad.all[0].rightShoulder.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SUMMONMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Summon Press 1/left to use skill 1, Press 2/right to use skill2");
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    SummonSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    SummonSkill_B();
                }
                break;

            case BattleState.DIRECTORTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Director's turn, Press W/triangle to open skill tree");
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.W) || Gamepad.all[0].buttonNorth.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.DIRECTORMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Director Press 1/left or 2/right to attack");
                    isTurnStart = false;
                    canUseAlpha1 = true;
                }
                if ((Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed) && canUseAlpha1)
                {
                    DirectorSkill_A();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)//it's the pause switch
                {
                    if(!isGamePaused)
                    {
                        Time.timeScale = 0f;
                        isGamePaused = true;
                        canUseAlpha1 = false;
                        UpdateUIText("Paused, Press 3/triangle again to proceed");
                        //you can put the pause UI here
                    }
                }
                //the player has to press 3 to proceed to next stage
                if ((Input.GetKeyDown(KeyCode.Alpha3) || Gamepad.all[0].buttonNorth.isPressed) && isGamePaused)
                {   
                    //resume the game & does something such as stop boss behavior 
                    Time.timeScale = 1.0f;
                    isGamePaused = false;
                    UpdateUIText("Game Resumed");
                    
                    //we only need transition function here, but I also want it to do some damage check or functions, transition included in this function
                    DirectorSkill_B();
                }       
                break;
            
            case BattleState.SOUNDTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Sound's turn, Press S/circle to open skill tree");
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.S) || Gamepad.all[0].buttonEast.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SOUNDMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Sound, Press 1/left & 2/right to attack");
                    if (inspirationFull)
                    {
                        UpdateUIText("Inspiration Full, press 2/right to use unique skill");
                    }
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    SoundSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    SoundSkill_B();
                    Debug.Log("dpad pressed");
                }
                break;

            case BattleState.CAMERATURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Camera's turn, Press C/square to open skill tree");
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.C) || Gamepad.all[0].buttonWest.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.CAMERAMENU:
                if (isTurnStart)
                {
                    UpdateUIText("It's Camera's turn, Press 1/left & 2/right to use skills");
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    CameraSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    CameraSkill_B();
                }
                break;
                
            case BattleState.INTERNTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Intern's turn, Press I/ X Button to open skill tree");
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.I) || Gamepad.all[0].buttonSouth.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.INTERNMENU:
                if (isTurnStart)
                {
                    UpdateUIText("It's Intern's turn, Press 1/left & 2/right to use skills");
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    InternSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    InternSkill_B();
                }
                break;

            case BattleState.BOSSTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Boss's turn");
                    isTurnStart = false;
                    BossSkill_A();
                }
                break;

            case BattleState.WON:
                UpdateUIText("You Win The JRPG Battle!");
                break;

            case BattleState.LOST:
                UpdateUIText("You LOST!");
                break;
        }

        if (inspirationFull == true)
        {
            Unit soundUnit = GameManager.instance.sound.GetComponent<Unit>();
            int inspiration = soundUnit.inspirationBar;
            SoundInspiration.value = inspiration;
        }
        else
        {
            SoundInspiration.value = 0;
        }
        if (vCam1On == true)
        {
            vCam1.Priority = 1;
            vCam2.Priority = 0;
        }
        else
        {
            vCam2.Priority = 1;
            vCam1.Priority = 0;
        }
        //team health bar

        TeamHealth.value = teamphysicalHP;
        
    //this is tracking the health of boss to the sliders
        if (bossUnit.currentPhysicalHP >= 300 & bossUnit.currentPhysicalHP <= 400)
        {
            bossHealth04.value = bossUnit.currentPhysicalHP;
        }
        if (bossUnit.currentPhysicalHP >= 200 & bossUnit.currentPhysicalHP <= 300)
        {
            bossHealth03.value = bossUnit.currentPhysicalHP;
        }
        if (bossUnit.currentPhysicalHP >= 100 & bossUnit.currentPhysicalHP <= 200)
        {
            bossHealth02.value = bossUnit.currentPhysicalHP;
        }
        if (bossUnit.currentPhysicalHP >= 0 && bossUnit.currentPhysicalHP <= 100)
        {
            bossHealth02.value = bossUnit.currentPhysicalHP;
        }
    //this is tracking the spirit health of boss to the sliders    
    if (bossUnit.currentMagicalHP >= 300 & bossUnit.currentMagicalHP <= 400)
    {
        bossSpirit04.value = bossUnit.currentMagicalHP;
    }
    if (bossUnit.currentMagicalHP >= 200 & bossUnit.currentMagicalHP <= 300)
    {
        bossSpirit03.value = bossUnit.currentMagicalHP;
    }
    if (bossUnit.currentMagicalHP >= 100 & bossUnit.currentMagicalHP <= 200)
    {
        bossSpirit02.value = bossUnit.currentMagicalHP;
    }
    if (bossUnit.currentMagicalHP >= 0 && bossUnit.currentMagicalHP <= 100)
    {
        bossSpirit02.value = bossUnit.currentMagicalHP;
    }
        
        
    }

#region Character Skills
    void HostSkill_A()
    {
        Debug.Log("Host Damaged Boss by Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(hostUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(hostUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
            DamageMonitor();
    }
    void HostSkill_B()
    {
        Debug.Log("Host Damaged Boss by Skill 2");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(hostUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(hostUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
            DamageMonitor();
    }
    
    void SummonSkill_A()
    {   
        Debug.Log("Summon Damaged Boss by Skill 1");

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(summonUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(summonUnit.magicalDamage);
        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void SummonSkill_B()
    {
        Debug.Log("Summon Damaged Boss by Skill 2");

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(summonUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(summonUnit.magicalDamage);
        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void DirectorSkill_A()
    {
        Debug.Log("Director Damaged Boss Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(directorUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(directorUnit.magicalDamage);
            //check if boss is dead or if the current bar reaches 0

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();    
    }
    void DirectorSkill_B()
    {
        //add future function to stop boss behavior
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(directorUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(directorUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
            DamageMonitor();   
    }

    void SoundSkill_A()
    {
        Debug.Log("Sound Damaged Boss Skill 1");

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(soundUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(soundUnit.magicalDamage);
            
        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }

        DamageMonitor();
    }
    void SoundSkill_B()
    {
        if (inspirationFull)
        {   
            if (songPlaying >= songs.Count -1)
            {
                songPlaying = 0;
            }
            else
            {
                songPlaying += 1;
            }
        
            ChangeSong(songPlaying);

            Debug.Log("Music Changed");
                //reset the inspiration bar
            soundUnit.inspirationBar = 0;
            inspirationFull = false;

            bool physicalIsDead = bossUnit.TakePhysicalDamage(soundUnit.physicalDamage, this);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(soundUnit.magicalDamage, this);

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
                // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                TransitionToNextTurn();
            }
            else
            {
                TransitionToNextTurn();
            }

            DamageMonitor();
        }
    }
    
    //I made switch camera skill into skill_B, so that every Skill_A is normal attack and Skill_B is skill
    void CameraSkill_A()
    {   
        Debug.Log("Camera Uses Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(cameraUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(cameraUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void CameraSkill_B()
    {
        vCam1On = !vCam1On;
        Debug.Log("Camera Uses Skill 2");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(cameraUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(cameraUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void InternSkill_A()
    {
        Debug.Log("Intern Uses Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(internUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(internUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void InternSkill_B()
    {
        Debug.Log("Intern Lucky Time Skill 2");
        //basically have a 50% chance to either increase or decrease health
        //I didn't include magical health because it doesn't make sense for an intern to be able to influence our majesty (summon);
        int chance = Random.Range(0, 2);
        if (chance == 0)
        {
            teamphysicalHP -= 100;
            Debug.Log("A o! Damn!");
        }
        
        else if (teamphysicalHP + 50 > maxTeamPhysicalHP )
        {
            teamphysicalHP = maxTeamPhysicalHP; 
            Debug.Log("Capped Max Health");
        }

        else
        {
            teamphysicalHP += 100;
            Debug.Log("Yeah Nice Intern but we underpay you!");
        }

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(internUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(internUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
#endregion
 
#region Boss Behavior & Skills
    void BossSkill_A()
    {
        Debug.Log("Boss uses Skill 1");
        //deal both physical and magical damage for now
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
#endregion
    
    void Setup()
    {
        //this function spawns every unit at the beginning of the game
        GameObject playerHost = Instantiate(hostPrefab, hostBattleStation);
        hostUnit = playerHost.GetComponent<Unit>();

        GameObject playerSummon = Instantiate(summonPrefab, summonBattleStation);
        summonUnit = playerSummon.GetComponent<Unit>();

        GameObject playerDirector = Instantiate(directorPrefab, directorBattleStation);
        directorUnit = playerDirector.GetComponent<Unit>();

        GameObject playerSound = Instantiate(soundPrefab, soundBattleStation);
        soundUnit = playerSound.GetComponent<Unit>();

        GameObject playerCamera = Instantiate(cameraPrefab, cameraBattleStation);
        cameraUnit = playerCamera.GetComponent<Unit>();

        GameObject playerIntern = Instantiate(internPrefab, internBattleStation);
        internUnit = playerIntern.GetComponent<Unit>();

        //GO stands for gameobject
        GameObject bossGO = Instantiate(bossPrefab, bossBattleStation);
        bossUnit = bossGO.GetComponent<Unit>();

        state = BattleState.HOSTTURN;
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You Win The JRPG Battle!");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You Are So WEAK! You Lost!");
        }
    }

    void TransitionToNextTurn() //this function now is a state machine switch
    {
        //Debug.Log($"Current state before transition: {state}");
        switch (state)
        {
            case BattleState.HOSTTURN:
                state = BattleState.HOSTMENU;
                break;
            case BattleState.HOSTMENU:
                state = BattleState.SUMMONTURN;
                break;
            case BattleState.SUMMONTURN:
                state = BattleState.SUMMONMENU;
                break;
            case BattleState.SUMMONMENU:
                state = BattleState.DIRECTORTURN;
                break;
            case BattleState.DIRECTORTURN:
                state = BattleState.DIRECTORMENU;
                break;
            case BattleState.DIRECTORMENU:
                state = BattleState.SOUNDTURN;
                break;
            case BattleState.SOUNDTURN:
                state = BattleState.SOUNDMENU;
                break;
            case BattleState.SOUNDMENU:
                state = BattleState.CAMERATURN;
                break;
            case BattleState.CAMERATURN:
                state = BattleState.CAMERAMENU;
                break;
            case BattleState.CAMERAMENU:
                state = BattleState.INTERNTURN;
                break;
            case BattleState.INTERNTURN:
                state = BattleState.INTERNMENU;
                break;
            case BattleState.INTERNMENU:
                state = BattleState.BOSSTURN;
                break;
            case BattleState.BOSSTURN:
                state = BattleState.HOSTTURN;
                break;
        }

        isTurnStart = true;
        //Debug.Log($"Transitioning to: {state}");
    }
   
    void DamageMonitor()
    {
        if (DamageMonitorOn == true)
        {
            //show all stats
            Debug.Log("Team Physical HP: " + teamphysicalHP + " " + "Team Magical HP: " + teamMagicalHP);
            Debug.Log("Boss physical HP: " + bossUnit.currentPhysicalHP + " " + "Boss magical HP: " + bossUnit.currentMagicalHP);
            //Debug.Log("Inspiration:" + soundUnit.inspirationBar + " " + "InspirationFull is" + inspirationFull);
        }
    }
    
    //took this from our studio 2 project, we can adjust it later and maybe simplify it if it's weird
    public void ChangeSong(int newSongIndex)
    {
        if (newSongIndex != currentSongIndex && newSongIndex < songs.Count)
        {
            StartCoroutine(FadeChangeSong(newSongIndex));
        }
    }    
    private IEnumerator FadeChangeSong(int newSongIndex)
    {
        // Fade out the current song if it's playing
        if (currentSongIndex != -1)
        {
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                MusicSource.volume = (1 - t / fadeTime);
                yield return null;
            }
            MusicSource.Stop();
            MusicSource.volume = 1; // Reset volume
        }

        // Update the current song index
        currentSongIndex = newSongIndex;

        // Set the new song and fade it in
        MusicSource.clip = songs[newSongIndex];
        MusicSource.Play();

        MusicSource.volume = 0; // Start at 0 volume to fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        { MusicSource.volume = t / fadeTime;
            yield return null;
        }

        MusicSource.volume = 1; // Ensure volume is back to full after fade in
    }
    
    private void UpdateUIText(string message)//this method sync debug and ui message
    {
        uiMessage = message;
        testingPrompts.text = uiMessage; //update ui message 
        Debug.Log(uiMessage); //show the updated message in console as well
    }


}  