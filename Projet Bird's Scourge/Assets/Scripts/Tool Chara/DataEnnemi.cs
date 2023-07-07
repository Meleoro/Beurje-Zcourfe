using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataEnnemy")]
public class DataEnnemi : ScriptableObject
{
    [Header("General")] public string charaName;
    public int maxHealth;

    [Header("Sprites")] public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite damageSprite;

    [Header("Competences")] public DataCompetence attaqueData;
    public DataCompetence competenceData;

    [Header("Stats")] public int force;
    public int defense;
    public int vitesse;
    public int agilite;
    public int chance;
}
