using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;

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
                    Debug.Log("It's Host's turn, Press Q to open skill tree");
                    testingPrompts.text = "It's Host's turn, Press Q to open skill tree";
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    TransitionToNextTurn();
                }    
                break;
                
            case BattleState.HOSTMENU:
                if (isTurnStart)
                {
                    Debug.Log("Host Press 1 to use skill 1, Press 2 to use skill2");
                    testingPrompts.text = "Host Press 1 to use skill 1, Press 2 to use skill2";
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    HostSkill_A();// Moves to SUMMONTURN after action
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    HostSkill_B();// Moves to SUMMONTURN after action
                }
                break;

            case BattleState.SUMMONTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Summon's turn, Press E to open skill tree");
                    testingPrompts.text = "It's Summon's turn, Press E to open skill tree";
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.E))
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SUMMONMENU:
                if (isTurnStart)
                {
                    Debug.Log("Summon Press 1 to use skill 1, Press 2 to use skill2");
                    testingPrompts.text = "Summon Press 1 to use skill 1, Press 2 to use skill2";
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SummonSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SummonSkill_B();
                }
                break;

            case BattleState.DIRECTORTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Director's turn, Press W to open skill tree");
                    testingPrompts.text = "It's Director's turn, Press W to open skill tree";
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.W))
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.DIRECTORMENU:
                if (isTurnStart)
                {
                    Debug.Log("Director Press 1 & 2 to attack");
                    testingPrompts.text = "Director Press 1 & 2 to attack";
                    isTurnStart = false;
                    canUseAlpha1 = true;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) && canUseAlpha1)
                {
                    DirectorSkill_A();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))//it's the pause switch
                {
                    if(!isGamePaused)
                    {
                        Time.timeScale = 0f;
                        isGamePaused = true;
                        canUseAlpha1 = false;
                        Debug.Log("Paused, Press 3 to proceed");
                        //you can put the pause UI here
                    }
                }
                //the player has to press 3 to proceed to next stage
                if (isGamePaused && Input.GetKeyDown(KeyCode.Alpha3))
                {   
                    //resume the game & does something such as stop boss behavior 
                    Time.timeScale = 1.0f;
                    isGamePaused = false;
                    Debug.Log("Game Resumed");
                    
                    //we only need transition function here, but I also want it to do some damage check or functions, transition included in this function
                    DirectorSkill_B();
                }       
                break;
            
            case BattleState.SOUNDTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Sound's turn, Press S to open skill tree");
                    testingPrompts.text = "It's Sound's turn, Press S to open skill tree";
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.S))
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SOUNDMENU:
                if (isTurnStart)
                {
                    Debug.Log("Sound, Press 1 & 2 to attack");
                    testingPrompts.text = "Sound, Press 1 & 2 to attack";
                    if (inspirationFull)
                    {
                     Debug.Log("Inspiration Full, press 2 to use unique skill");
                     testingPrompts.text = "Inspiration Full, press 2 to use unique skill";
                    }
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SoundSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SoundSkill_B();
                }
                break;

            case BattleState.CAMERATURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Camera's turn, Press C to open skill tree");
                    testingPrompts.text = "It's Camera's turn, Press C to open skill tree";
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.C))
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.CAMERAMENU:
                if (isTurnStart)
                {
                    Debug.Log("It's Camera's turn, Press 1 & 2 to use skills");
                    testingPrompts.text = "It's Camera's turn, Press 1 & 2 to use skills";
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    CameraSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    CameraSkill_B();
                }
                break;
                
            case BattleState.INTERNTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Intern's turn, Press I to open skill tree");
                    testingPrompts.text = "It's Intern's turn, Press I to open skill tree";
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.I))
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.INTERNMENU:
                if (isTurnStart)
                {
                    Debug.Log("It's Intern's turn, Press 1 & 2 to use skills");
                    testingPrompts.text = "It's Intern's turn, Press 1 & 2 to use skills";
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    InternSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    InternSkill_B();
                }
                break;

            case BattleState.BOSSTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Boss's turn");
                    testingPrompts.text = "It's Boss's turn";
                    isTurnStart = false;
                    BossSkill_A();
                }
                break;

            case BattleState.WON:
                Debug.Log("You Win The JRPG Battle!");
                testingPrompts.text = "You Win The JRPG Battle!";
                break;

            case BattleState.LOST:
                Debug.Log("You LOST!");
                testingPrompts.text = "You LOST!";
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
    
}  