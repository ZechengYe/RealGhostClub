using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int physicalDamage;
    public int magicalDamage;

    public int maxPhysicalHP;
    public int maxMagicalHP;
    
    public int currentPhysicalHP;
    public int currentMagicalHP;

    public int inspirationBar;
    public int maxInspirationBar;

    //these are for team
    public bool TakePhysicalDamage(int physicalDmg, BattleSystem battleSystem)
    {
        return battleSystem.TakeSharedPhysicalDamage(physicalDmg);
    }
    public bool TakeMagicalDamage(int magicalDmg, BattleSystem battleSystem)
    {
        return battleSystem.TakeSharedMagicalDamage(magicalDmg);
    }

    //these are for boss
    public bool bossTakePhysicalDamage(int physicalDmg)
    {

        if(BattleSystem.bossWeak)
        {
            Debug.Log("Boss is weak now, double physical damage");
            currentPhysicalHP -= physicalDmg * 2;
        }
        else 
        {
            currentPhysicalHP -= physicalDmg;
            Debug.Log("Boss is not weak now, music matters.");
        }

        if (currentPhysicalHP <= 0)
        {
            return true; // Indicates death
        }
        else 
            return false;
    }
    public bool bossTakeMagicalDamage(int magicalDmg)
    {
        if(BattleSystem.bossWeak)
        {
            Debug.Log("Boss is weak now, double magical damage");
            currentMagicalHP -= magicalDmg * 2;
        }
        else
        {
            currentMagicalHP -= magicalDmg;
        }
        if (currentMagicalHP <= 0)
        {
            return true; // Indicates death
        }
        else 
            return false;
    }

    //inspiration bar
    public bool UpdateInspirationBar(int physicalDmg)
    {
        inspirationBar += (physicalDmg);
        if (inspirationBar >= maxInspirationBar)
        {
            //cap the bar so the value won't keep incrementing
            inspirationBar = maxInspirationBar;
            return true;
        }
        else
            return false;
    }
}
