using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int physicalDamage;
    public int magicalDamage;

    public int maxPhysicalHP;
    public int maxMagicalHP;

    public int currentPhysicalHP;
    public int currentMagicalHP;
    
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
        currentPhysicalHP -= magicalDmg;
        if (currentPhysicalHP <= 0)
        {
            return true;
        }
        else
            return false;
    }
}
