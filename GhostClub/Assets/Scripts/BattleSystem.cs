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
                    FirstSkill();
                    TransitionToNextTurn();  // Moves to SUMMONTURN after action
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  // Moves to SUMMONTURN after action
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
                    FirstSkill();
                    TransitionToNextTurn();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  
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
                    FirstSkill();
                    TransitionToNextTurn();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  
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
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    FirstSkill();
                    TransitionToNextTurn();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  
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
                    FirstSkill();
                    TransitionToNextTurn();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  
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
                    FirstSkill();
                    TransitionToNextTurn();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SecondSkill();
                    TransitionToNextTurn();  
                }
                break;

            case BattleState.BOSSTURN:
                if (isTurnStart)
                {
                    Debug.Log("It's Boss's turn, Press C");
                    isTurnStart = false;
                    TransitionToNextTurn();
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

    void FirstSkill()
    {
         Debug.Log("Damaged Boss by Skill 1");
         DamageMonitor();
    }
    void SecondSkill()
    {
        Debug.Log("Damaged Boss by Skill 2");
        DamageMonitor();
    }
    void TransitionToNextTurn() //this function now is a state machine switch
    {
        Debug.Log($"Current state before transition: {state}");
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
        Debug.Log($"Transitioning to: {state}");
    }

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
    void BossTurn()
    {
        Debug.Log("It's boss's turn");
        //deal both physical and magical damage for now
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            //if anyone in the party died the game is over
            //at least for now
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.HOSTTURN;
            HostTurn();
        }
        DamageMonitor();
    }

    void HostTurn()
    {
        Debug.Log("It's Host's turn, Press Q to deal physical damage");
    }
    void HostAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool physicalIsDead = bossUnit.bossTakePhysicalDamage(hostUnit.physicalDamage);
            bool magicalIsDead = bossUnit.bossTakeMagicalDamage(hostUnit.magicalDamage);
            //check if boss is dead or if the current bar reaches 0

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
            // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                // go to summon's turn
                state = BattleState.SUMMONTURN;
                SummonTurn();
            }
            else
            {
                // Both physical and magical damage are alive, proceed to summon's turn
                //state = BattleState.SUMMONTURN;
                state = BattleState.SUMMONTURN;
                SummonTurn();
            }
            DamageMonitor();
        }
    }

    void SummonTurn()
    {
        Debug.Log("It's Summon's turn, Press E to deal magical damage");
    }
    void SummonAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool physicalIsDead = bossUnit.bossTakePhysicalDamage(summonUnit.physicalDamage);
            bool magicalIsDead = bossUnit.bossTakeMagicalDamage(summonUnit.magicalDamage);
            //check if boss is dead or if the current bar reaches 0

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
            // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                // Return to boss's turn
                state = BattleState.DIRECTORTURN;
                DirectorTurn();
            }
            else
            {
                // Both physical and magical damage are alive, proceed to summon's turn
                // for now it's boss turn
                //state = BattleState.SUMMONTURN;
                state = BattleState.DIRECTORTURN;
                DirectorTurn();
            }
            DamageMonitor();
        }
    }

    void DirectorTurn()
    {
        Debug.Log("It's Director's turn, Press W to cut and pause the game");
    }
    void DirectorAttack()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //future function
            //director pauses the game or serving as pause function
            //if pause, boss spell chanting will be stopped
            //Time.timeScale = 0f;
            Debug.Log("Cut");

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
                // Return to boss's turn
                state = BattleState.SOUNDTURN;
                SoundTurn();
            }
            else
            {
                // Both physical and magical damage are alive, proceed to summon's turn
                // for now it's boss turn
                //state = BattleState.SUMMONTURN;
                state = BattleState.SOUNDTURN;
                SoundTurn();
            }
            DamageMonitor();
        }
    }

    void SoundTurn()
    {
        Debug.Log("It's Sound guy's turn, Press S to change music");
        if (inspirationFull)
        {
            Debug.Log("Inspiration Full, press L to use unique skill");
        }
    }
    void SoundAttack()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            bool physicalIsDead = bossUnit.bossTakePhysicalDamage(soundUnit.physicalDamage);
            bool magicalIsDead = bossUnit.bossTakeMagicalDamage(soundUnit.magicalDamage);
            
            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
            // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                state = BattleState.CAMERATURN;
                CameraTurn();
            }
            else
            {
                state = BattleState.CAMERATURN;
                CameraTurn();
            }

            DamageMonitor();
        }
        //I really should write this as a sepreate function
        if (inspirationFull)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
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
                    state = BattleState.CAMERATURN;
                    CameraTurn();
                }
                else
                {
                    state = BattleState.CAMERATURN;
                    CameraTurn();
                }

                DamageMonitor();
            }
        }

    }

    void CameraTurn()
    {
        Debug.Log("It's Camera crew's turn, Press C to change camera angle");
    }
    void CameraAttack()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            bool physicalIsDead = bossUnit.bossTakePhysicalDamage(cameraUnit.physicalDamage);
            bool magicalIsDead = bossUnit.bossTakeMagicalDamage(cameraUnit.magicalDamage);

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
            // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                state = BattleState.INTERNTURN;
                InternTurn();
            }
            else
            {
                state = BattleState.INTERNTURN;
                InternTurn();
            }
            DamageMonitor();
        }
    }

    void InternTurn()
    {
        Debug.Log("It's Intern's turn, Press O to do random things");
    }
    void InternAttack()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            bool physicalIsDead = bossUnit.bossTakePhysicalDamage(internUnit.physicalDamage);
            bool magicalIsDead = bossUnit.bossTakeMagicalDamage(internUnit.magicalDamage);

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
            // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                state = BattleState.BOSSTURN;
                BossTurn();
            }
            else
            {
                state = BattleState.BOSSTURN;
                BossTurn();
            }
            DamageMonitor();
        }
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

    void DamageMonitor()
    {
        if (DamageMonitorOn == true)
        {
            //show all stats
            Debug.Log("Team Physical HP: " + teamphysicalHP + " " + "Team Magical HP: " + teamMagicalHP);
            Debug.Log("Boss physical HP: " + bossUnit.currentPhysicalHP + " " + "Boss magical HP: " + bossUnit.currentMagicalHP);
            Debug.Log("Inspiration:" + soundUnit.inspirationBar + " " + "InspirationFull is" + inspirationFull);
        }
    }
}  
