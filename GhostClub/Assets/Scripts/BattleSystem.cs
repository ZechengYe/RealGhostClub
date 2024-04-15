using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

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

    bool inspirationFull = false;
    #endregion
    private bool isTurnStart = true;

    //I put the shared health management system here instead of unit, so it's more manageable
    public int teamphysicalHP = 1000;
    public int teamMagicalHP = 500;
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
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    DirectorSkill_A();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    DirectorSkill_B();  
                }
                break;
            
            case BattleState.SOUNDTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Sound's turn, Press S to open skill tree");
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
                    if (inspirationFull)
                    {
                     Debug.Log("Inspiration Full, press 2 to use unique skill");
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
                    isTurnStart = false;
                    BossSkill_A();
                }
                break;

            case BattleState.WON:
                Debug.Log("You Win The JRPG Battle!");
                break;

            case BattleState.LOST:
                Debug.Log("You LOST!");
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
        Debug.Log("Director Damaged Boss Skill 2");
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
            Debug.Log("Sound Damaged Boss Skill 2");
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
        Debug.Log("Intern Uses Skill 2");
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
}  