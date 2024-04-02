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
    public bool TakePhysicalDamage (int physicalDmg)
    {
        currentPhysicalHP -= physicalDmg;
        if (currentPhysicalHP <= 0)
        {
            return true;
        }
        else 
            return false;
    }

    public bool TakeMagicalDamage(int magicalDmg)
    {
        currentMagicalHP -= magicalDmg;
        if (currentMagicalHP <= 0)
        {
            return true;
        }
        else
            return false;
    }

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
