using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleButtons : MonoBehaviour
{
    [Header("Buttons")]
    public Button attackButton;
    public Button skill1Button;
    public Button skill2Button;
    public Button endTurnButton;

    [Header("Names")]
    public TextMeshProUGUI attackName;
    public TextMeshProUGUI competence1Name;
    public TextMeshProUGUI competence2Name;

    [Header("Descriptions")]
    public TextMeshProUGUI attackDescription;
    public TextMeshProUGUI competence1Description;
    public TextMeshProUGUI competence2Description; 
    
    [Header("ManaCosts")]
    public TextMeshProUGUI attackManaCost;
    public TextMeshProUGUI competence1ManaCost;
    public TextMeshProUGUI competence2ManaCost;

    [Header("DamageMultipliers")]
    public TextMeshProUGUI attackDamageMultiplier;
    public TextMeshProUGUI competence1DamageMultiplier;
    public TextMeshProUGUI competence2DamageMultiplier;

    [Header("CancelButtons")]
    public GameObject attackCancelButton;
    public GameObject competence1CancelButton;
    public GameObject competence2CancelButton;


    //----------------------- INFO PART ----------------------------

    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos, Unit unitScript, Ennemy ennemyScript)
    {
        bool isAllié = false;
        if ((unitInfos.isEnnemy)) isAllié = false;
        else isAllié = true;

        if (isAllié)
        {
            if (unitInfos.attaqueData is not null)
            {
                attackName.text = unitInfos.attaqueData.competenceName;
                attackDescription.text = unitInfos.attaqueData.levels[unitScript.AttackLevel].competenceDescription;
                attackManaCost.text = unitInfos.attaqueData.levels[unitScript.AttackLevel].competenceManaCost.ToString();
                attackDamageMultiplier.text = "STR x" + unitInfos.attaqueData.levels[unitScript.AttackLevel].damageMultiplier;
            }

            if (unitInfos.competence1Data is not null)
            {
                competence1Name.text = unitInfos.competence1Data.competenceName;
                competence1Description.text = unitInfos.competence1Data.levels[unitScript.Competence1Level].competenceDescription;
                competence1ManaCost.text = unitInfos.competence1Data.levels[unitScript.Competence1Level].competenceManaCost.ToString();
                competence1DamageMultiplier.text = "STR x" + unitInfos.competence1Data.levels[unitScript.Competence1Level].damageMultiplier;
            }

            if (unitInfos.competence2Data is not null)
            {
                competence2Name.text = unitInfos.competence2Data.competenceName;
                competence2Description.text = unitInfos.competence2Data.levels[unitScript.Competence2Level].competenceDescription;
                competence2ManaCost.text = unitInfos.competence2Data.levels[unitScript.Competence2Level].competenceManaCost.ToString();
                competence2DamageMultiplier.text = "STR x" + unitInfos.competence2Data.levels[unitScript.Competence2Level].damageMultiplier;
            }
        }
        else
        {
            if (unitInfos.attaqueData is not null)
            {
                attackName.text = unitInfos.attaqueData.competenceName;
                attackDescription.text = unitInfos.attaqueData.levels[ennemyScript.CurrentLevel].competenceDescription;
                attackManaCost.text = unitInfos.attaqueData.levels[ennemyScript.CurrentLevel].competenceManaCost.ToString();
                attackDamageMultiplier.text = "STR x" + unitInfos.attaqueData.levels[ennemyScript.CurrentLevel].damageMultiplier;
            }

            if (unitInfos.competence1Data is not null)
            {
                competence1Name.text = unitInfos.competence1Data.competenceName;
                competence1Description.text = unitInfos.competence1Data.levels[ennemyScript.CurrentLevel].competenceDescription;
                competence1ManaCost.text = unitInfos.competence1Data.levels[ennemyScript.CurrentLevel].competenceManaCost.ToString();
                competence1DamageMultiplier.text = "STR x" + unitInfos.competence1Data.levels[ennemyScript.CurrentLevel].damageMultiplier;
            }

            if (unitInfos.competence2Data is not null)
            {
                competence2Name.text = unitInfos.competence2Data.competenceName;
                competence2Description.text = unitInfos.competence2Data.levels[ennemyScript.CurrentLevel].competenceDescription;
                competence2ManaCost.text = unitInfos.competence2Data.levels[ennemyScript.CurrentLevel].competenceManaCost.ToString();
                competence2DamageMultiplier.text = "STR x" + unitInfos.competence2Data.levels[ennemyScript.CurrentLevel].damageMultiplier;
            }
        }

    }



    //----------------------- CONTROL PART ----------------------------

    // TO AVOID INTERACTONG WITH WHAT IS BEHIND THE BUTTONS
    public void EnterButton()
    {
        MouseManager.Instance.isOnButton = true;
    }

    // TO AVOID INTERACTONG WITH WHAT IS BEHIND THE BUTTONS
    public void ExitButton()
    {
        MouseManager.Instance.isOnButton = false;
    }


    // INFORM OTHER SCRIPT THAT A BUTTON HAS BEEN PRESSED
    public void ClickButton(int index)
    {
        MouseManager.Instance.ChangeSelectedCompetence(index);

        ChangeButtonState(index);
    }

    // FAIT APPARAITRE OU DISPARAITRE LES BOUTONS D'ANULATION DES SKILLS
    public void ChangeButtonState(int index)
    {
        if (index == 0)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.attaqueData)
            {
                attackCancelButton.SetActive(false);
            }
            else
            {
                attackCancelButton.SetActive(true);
                competence1CancelButton.SetActive(false);
                competence2CancelButton.SetActive(false);
            }
        }

        if (index == 1)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.competence1Data)
            {
                competence1CancelButton.SetActive(false);
            }
            else
            {
                competence1CancelButton.SetActive(true);
                attackCancelButton.SetActive(false);
                competence2CancelButton.SetActive(false);
            }
        }

        if (index == 2)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.competence2Data)
            {
                competence2CancelButton.SetActive(false);
            }
            else
            {
                competence2CancelButton.SetActive(true);
                attackCancelButton.SetActive(false);
                competence1CancelButton.SetActive(false);
            }
        }
    }

    public void SwitchButtonInteractible(bool Activate)
    {
        attackButton.interactable = Activate;
        skill1Button.interactable = Activate;
        skill2Button.interactable = Activate;
        endTurnButton.interactable = Activate;
    }
}
