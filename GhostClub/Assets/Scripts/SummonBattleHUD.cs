using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonBattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI magicalDamageText;
    public TextMeshProUGUI magicalHPtext;

    public Unit currentUnit;

    public void Start()
    {
        //NS: this part needs work...how to find the specific instantiated Object?
        //VC: I added tags for the Boss, Host and Summon prefabs,
        //    and wired them up in the GameManager
        currentUnit = GameManager.instance.summon.GetComponent<Unit>();
    }
    
    public void Update()
    {
        SetupSummonHUD(currentUnit);
    }

    public void SetupSummonHUD(Unit unit)
    {
        unit = currentUnit;
        
        nameText.text = "Name: " + unit.unitName;
        magicalDamageText.text = "MP DMG: " + unit.magicalDamage;
        magicalHPtext.text = "MP HP: " +  unit.currentMagicalHP;
    }


}
