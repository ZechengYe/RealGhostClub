using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossBattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI physicalDamageText;
    public TextMeshProUGUI magicalDamageText;
    public TextMeshProUGUI physicalHPtext;
    public TextMeshProUGUI magicalHPtext;

    public Unit currentUnit;

    public void Start()
    {
        //NS: this part needs work...how to find the specific instantiated Object?
        //VC: I added tags for the Boss, Host and Summon prefabs,
        //    and wired them up in the GameManager
        currentUnit = GameManager.instance.boss.GetComponent<Unit>();
    }
    
    public void Update()
    {
        SetupHUD(currentUnit);
    }

    public void SetupHUD(Unit unit)
    {
        unit = currentUnit;
        
        nameText.text = "Name: " + unit.unitName;
        physicalDamageText.text = "Phys DMG: " + unit.physicalDamage;
        magicalDamageText.text = "MP DMG: " + unit.magicalDamage;
        physicalHPtext.text = "Phys HP: " + unit.currentPhysicalHP;
        magicalHPtext.text = "MP HP: " +  unit.currentMagicalHP;
    }


}
