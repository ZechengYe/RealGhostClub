using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, BOSSTURN, HOSTTURN, SUMMONTURN, DIRECTORTURN, SOUNDTURN, CAMERATURN, INTERNTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject hostPrefab;
    public GameObject summonPrefab;
    public GameObject bossPrefab;

    public Transform hostBattleStation;
    public Transform summonBattleStation;
    public Transform bossBattleStation;

    Unit hostUnit;
    Unit summonUnit;
    Unit bossUnit;
    private int currentPlayerHP;
    private int currentBossHP;

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
    }

    void Setup()
    {
        //this function spawns every unit at the beginning of the game
        //this unit represents host
        GameObject playerHost = Instantiate(hostPrefab, hostBattleStation);
        hostUnit = playerHost.GetComponent<Unit>();

        //this unit represents summon
        GameObject playerSummon = Instantiate(summonPrefab, summonBattleStation);
        summonUnit = playerSummon.GetComponent<Unit>();

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
        bool isDead = hostUnit.TakePhysicalDamage(bossUnit.physicalDamage);
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
            //It should go to host Turn
            state = BattleState.HOSTTURN;
            HostTurn();
        }
    }

    void HostTurn()
    {
        Debug.Log("It's Host's turn, Press Q to deal physical damage");
    }

    void HostAttack()
    {
        //setup input
        //for test, host just damage the boss
        //change state based on what happened
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
            Debug.Log("Host's physical HP: " + hostUnit.currentPhysicalHP);
            Debug.Log("Summon's magical  HP: " + summonUnit.currentMagicalHP);
            Debug.Log("Boss's physical HP: " + bossUnit.currentPhysicalHP);
            Debug.Log("Boss's magical HP: " + bossUnit.currentMagicalHP);
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
                state = BattleState.BOSSTURN;
                BossTurn();
            }
            else
            {
                // Both physical and magical damage are alive, proceed to summon's turn
                // for now it's boss turn
                //state = BattleState.SUMMONTURN;
                state = BattleState.BOSSTURN;
                BossTurn();
            }
            Debug.Log("Host's physical HP: " + hostUnit.currentPhysicalHP);
            Debug.Log("Summon's magical  HP: " + summonUnit.currentMagicalHP);
            Debug.Log("Boss's physical HP: " + bossUnit.currentPhysicalHP);
            Debug.Log("Boss's magical HP: " + bossUnit.currentMagicalHP);
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
}  
