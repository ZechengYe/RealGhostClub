using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostBattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI physicalDamageText;
    public TextMeshProUGUI physicalHPtext;
    

    public Unit currentUnit;

    public void Start()
    {
        //NS: this part needs work...how to find the specific instantiated Object?
        //VC: I added tags for the Boss, Host and Summon prefabs,
        //    and wired them up in the GameManager
        currentUnit = GameManager.instance.host.GetComponent<Unit>();
    }
    
    public void Update()
    {
        SetupHostHUD(currentUnit);
    }

    public void SetupHostHUD(Unit unit)
    {
        unit = currentUnit;
        
        nameText.text = "Name: " + unit.unitName;
        physicalDamageText.text = "Phys DMG: " + unit.physicalDamage;
        physicalHPtext.text = "Phys HP: " + unit.currentPhysicalHP;
        
    }


}
