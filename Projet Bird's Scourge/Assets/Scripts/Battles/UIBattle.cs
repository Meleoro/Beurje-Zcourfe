using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle : MonoBehaviour
{
    [Header("Buttons")]
    public TextMeshProUGUI attackButton;
    public TextMeshProUGUI competence1Button;
    public TextMeshProUGUI competence2Button;

    
    // CHANGE THE UI TO SHOW THE INFOS OF THE CURRENTLY SELECTED UNIT
    public void OpenUnitInfos(DataUnit unitInfos)
    {
        ActualiseButtons(unitInfos);
    }

    
    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos)
    {
        if(unitInfos.attaqueData != null)
            attackButton.text = unitInfos.attaqueData.competenceName;
        
        if(unitInfos.competence1Data != null)
           competence1Button.text = unitInfos.competence1Data.competenceName;
        
        if(unitInfos.competence2Data != null)
           competence2Button.text = unitInfos.competence2Data.competenceName;
    }
    
    
}
