using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI physicalDamageText;
    public TextMeshProUGUI magicalDamageText;
    public TextMeshProUGUI physicalHPtext;
    public TextMeshProUGUI magicalHPtext;

    public void SetupHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        physicalDamageText.text = unit.physicalDamage.ToString();
        magicalDamageText.text = unit.magicalDamage.ToString();
        physicalHPtext.text = unit.currentPhysicalHP.ToString();
        magicalHPtext.text = unit.currentMagicalHP.ToString();
    }

}
