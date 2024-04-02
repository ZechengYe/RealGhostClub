using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public enum BattleState { START, BOSSTURN, HOSTTURN, SUMMONTURN, DIRECTORTURN, SOUNDTURN, CAMERATURN, INTERNTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
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

    private int currentPlayerHP;
    private int currentBossHP;

    bool inspirationFull = false;
    

    // Update is called once per frame
    void Start()
    {
        state = BattleState.START;
        Setup();
    }

    void Update()
    {
        if (state == BattleState.HOSTTURN)
        {
            HostAttack(); 
        }

        if (state == BattleState.SUMMONTURN)
        {
            SummonAttack();
        }

        if (state == BattleState.DIRECTORTURN)
        {
            DirectorAttack();
        }

        if (state == BattleState.SOUNDTURN)
        {
            SoundAttack();
        }

        if (state == BattleState.CAMERATURN)
        {
            CameraAttack();
        }

        if (state == BattleState.INTERNTURN)
        {
            InternAttack();
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

    void Setup()
    {
        //this function spawns every unit at the beginning of the game
        //this unit represents host
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

        //for test purpose I will let the host move first
        //the boss should move first based on our design
        //state = BattleState.BOSSTURN;
        //BossTurn();

        state = BattleState.HOSTTURN;
        HostTurn();
    }

    void BossTurn()
    {
        Debug.Log("It's boss's turn");
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead)
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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(hostUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(hostUnit.magicalDamage);
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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(summonUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(summonUnit.magicalDamage);
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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(directorUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(directorUnit.magicalDamage);
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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(soundUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(soundUnit.magicalDamage);
            
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
                

                bool physicalIsDead = bossUnit.TakePhysicalDamage(soundUnit.physicalDamage);
                bool magicalIsDead = bossUnit.TakeMagicalDamage(soundUnit.magicalDamage);

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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(cameraUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(cameraUnit.magicalDamage);

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
            bool physicalIsDead = bossUnit.TakePhysicalDamage(internUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(internUnit.magicalDamage);

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
        //show all stats
        Debug.Log("Host HP: " + hostUnit.currentPhysicalHP);
        Debug.Log("Summon HP: " + summonUnit.currentMagicalHP);
        Debug.Log("DirectorHP: " + directorUnit.currentPhysicalHP);
        Debug.Log("Boss's physical HP: " + bossUnit.currentPhysicalHP);
        Debug.Log("Boss's magical HP: " + bossUnit.currentMagicalHP);
        Debug.Log("Inspiration:" + soundUnit.inspirationBar);
        Debug.Log("InspirationFull" + inspirationFull);
    }
}  
