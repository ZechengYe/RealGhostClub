using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, BOSSTURN, HOSTTURN, SUMMONTURN, DIRECTORTURN, SOUNDTURN, CAMERATURN, INTERNTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject playerPrefab;
    public GameObject bossPrefab;
    public Transform playerBattleStation;
    public Transform bossBattleStation;
    Unit playerUnit;
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
            HostAttack(); // Call the attack method directly
        }
    }

    void Setup()
    {
        GameObject playerSummon = Instantiate(playerPrefab, playerBattleStation);
        //this player unit represents summon
        playerUnit = playerSummon.GetComponent<Unit>();

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
        bool isDead = playerUnit.TakePhysicalDamage(bossUnit.physicalDamage);
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
            //It should go to Summon Turn
            //but for test it's hostturn for now so I can make sure the game loops between states
            state = BattleState.HOSTTURN;
            HostTurn();
        }
    }

    void HostTurn()
    {
        Debug.Log("It's Host's turn, Press Q to act");
    }

    //I keep this button for now for classic JRPG structure purpose, but will propbably modify it
/*    void HostAttackButton()
    {
        if (state != BattleState.HOSTTURN)
        {
            return;
        }

        HostAttack();
    }*/

    void HostAttack()
    {
        //setup input
        //for test, host just damage the boss
        //change state based on what happened
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool physicalIsDead = bossUnit.TakePhysicalDamage(playerUnit.physicalDamage);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(playerUnit.magicalDamage);
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
            Debug.Log("Host's current HP: " + playerUnit.currentPhysicalHP);
            Debug.Log("Boss's current HP: " + bossUnit.currentPhysicalHP);
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
