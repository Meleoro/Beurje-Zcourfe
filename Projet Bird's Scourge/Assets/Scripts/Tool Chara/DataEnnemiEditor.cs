using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DataEnnemi))]
public class DataEnnemiEditor : Editor
{
    private SerializedObject so;
    private GUIStyle moduleNameStyle = new GUIStyle();
    private GUIStyle titreStyle = new GUIStyle();
    private DataEnnemi currentScript;
    
    [Header("General")] 
    public SerializedProperty charaName;
    public SerializedProperty maxHealth;
    
    [Header("Sprites")] 
    public SerializedProperty idleSprite;
    public SerializedProperty attackSprite;
    public SerializedProperty damageSprite;

    
    [Header("Movement")] 
    public SerializedProperty movePatern;
    public SerializedProperty nbrMovements;
    private int paternSize;

    [Header("Competences")] 
    public SerializedProperty attaqueData;
    public SerializedProperty competenceData;

    [Header("Stats")] 
    public SerializedProperty force;
    public SerializedProperty defense;
    public SerializedProperty vitesse;
    public SerializedProperty agilite;
    public SerializedProperty chance;

    
    
    private void OnEnable()
    {
        so = serializedObject;
        currentScript = target as DataEnnemi;
        
        moduleNameStyle.fontSize = 14;
        titreStyle.fontSize = 12;

        moduleNameStyle.fontStyle = FontStyle.Bold;
        titreStyle.fontStyle = FontStyle.Bold;

        moduleNameStyle.normal.textColor = Color.white;
        titreStyle.normal.textColor = Color.white;


        charaName = so.FindProperty("charaName");
        maxHealth = so.FindProperty("maxHealth");
        
        idleSprite = so.FindProperty("idleSprite");
        attackSprite = so.FindProperty("attackSprite");
        damageSprite = so.FindProperty("damageSprite");
        
        movePatern = so.FindProperty("movePatern");
        nbrMovements = so.FindProperty("nbrMovements");
        
        attaqueData = so.FindProperty("attaqueData");
        competenceData = so.FindProperty("competenceData");
        
        force = so.FindProperty("force");
        defense = so.FindProperty("defense");
        vitesse = so.FindProperty("vitesse");
        agilite = so.FindProperty("agilite");
        chance = so.FindProperty("chance");
    }

    
    public override void OnInspectorGUI()
    {
        so.Update();
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("General", moduleNameStyle);
            GUILayout.Space(4);

            EditorGUILayout.PropertyField(charaName);
            EditorGUILayout.PropertyField(maxHealth);
            
            GUILayout.Space(4);
        }
        
        GUILayout.Space(10);
        
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Sprites", moduleNameStyle);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(idleSprite);
            EditorGUILayout.PropertyField(attackSprite);
            EditorGUILayout.PropertyField(damageSprite);
        }
        
        GUILayout.Space(10);
        
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Movements", moduleNameStyle);
            GUILayout.Space(4);

            paternSize = movePatern.arraySize;
            paternSize = EditorGUILayout.IntField ("Paterne Size", paternSize);

            if (paternSize != movePatern.arraySize)
            {
                movePatern.ClearArray();
                
                for (int j = 0; j < paternSize; j++)
                {
                    movePatern.InsertArrayElementAtIndex(movePatern.arraySize);

                    SerializedProperty currentList = movePatern.GetArrayElementAtIndex(movePatern.arraySize - 1)
                        .FindPropertyRelative("list");

                    for (int i = 0; i < paternSize; i++)
                    {
                        currentList.InsertArrayElementAtIndex(currentList.arraySize);
                    }
                }
            }


            using (new GUILayout.VerticalScope(EditorStyles.helpBox, new [] {GUILayout.MinWidth(22 * movePatern.arraySize)}))
            {
                for (int k = 0; k < movePatern.arraySize; k++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        SerializedProperty currentList = movePatern.GetArrayElementAtIndex(k).FindPropertyRelative("list");
                            
                        for (int j = 0; j < movePatern.arraySize; j++)
                        {
                            if (k != movePatern.arraySize / 2 || j != movePatern.arraySize / 2)
                            {
                                EditorGUILayout.PropertyField(currentList.GetArrayElementAtIndex(j), GUIContent.none, GUILayout.MinWidth(EditorGUIUtility.labelWidth - 350));
                            }
                            else
                                GUILayout.Space(21);
                        }
                    }
                }
            }
            
            EditorGUILayout.PropertyField(nbrMovements);
        }
        
        GUILayout.Space(10);
        
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Attacks", moduleNameStyle);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(attaqueData);
            EditorGUILayout.PropertyField(competenceData);
        }
        
        GUILayout.Space(10);
        
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Stats", moduleNameStyle);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(force);
            EditorGUILayout.PropertyField(defense);
            EditorGUILayout.PropertyField(vitesse);
            EditorGUILayout.PropertyField(agilite);
            EditorGUILayout.PropertyField(defense);
        }
        
        
        so.ApplyModifiedProperties();
    }
}
