using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(DataUnit))]
public class DataUnitEditor : Editor
{
    private SerializedObject so;
    private GUIStyle moduleNameStyle = new GUIStyle();
    private GUIStyle titreStyle = new GUIStyle();
    private DataUnit currentScript;

    [Header("General")] 
    private SerializedProperty charaName;
    private SerializedProperty isEnnemy;
    
    private SerializedProperty idleSprite;
    private SerializedProperty attackSprite;
    private SerializedProperty damageSprite;
    
    private SerializedProperty attaqueData;
    private SerializedProperty competence1Data;
    private SerializedProperty competence2Data;
    private SerializedProperty passifData;
    
    private SerializedProperty levelUnlockCompetence1;
    private SerializedProperty levelUnlockCompetence2;
    private SerializedProperty levelUnlockPassif;

    private SerializedProperty levels;
    private int levelSize;
    private int paternSize;
    

    private void OnEnable()
    {
        so = serializedObject;
        currentScript = target as DataUnit;

        charaName = so.FindProperty("charaName");
        isEnnemy = so.FindProperty("isEnnemy");
        
        idleSprite = so.FindProperty("idleSprite");
        attackSprite = so.FindProperty("attackSprite");
        damageSprite = so.FindProperty("damageSprite");
        
        attaqueData = so.FindProperty("attaqueData");
        competence1Data = so.FindProperty("competence1Data");
        competence2Data = so.FindProperty("competence2Data");
        passifData = so.FindProperty("passifData");
        
        levelUnlockCompetence1 = so.FindProperty("levelUnlockCompetence1");
        levelUnlockCompetence2 = so.FindProperty("levelUnlockCompetence2");
        levelUnlockPassif = so.FindProperty("levelUnlockPassif");

        levels = so.FindProperty("levels");
        
        
        moduleNameStyle.fontSize = 14;
        titreStyle.fontSize = 12;

        moduleNameStyle.fontStyle = FontStyle.Bold;
        titreStyle.fontStyle = FontStyle.Bold;

        moduleNameStyle.normal.textColor = Color.white;
        titreStyle.normal.textColor = Color.white;
    }


    public override void OnInspectorGUI()
    {
        so.Update();

        // GENERAL PART
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("General", moduleNameStyle);
            GUILayout.Space(4);


            EditorGUILayout.PropertyField(charaName);
            EditorGUILayout.PropertyField(isEnnemy);
                
            GUILayout.Space(8);
            GUILayout.Label("Sprites", titreStyle);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(idleSprite);
            EditorGUILayout.PropertyField(attackSprite);
            EditorGUILayout.PropertyField(damageSprite);
                
            GUILayout.Space(8);
            GUILayout.Label("Compétences", titreStyle);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(attaqueData);
            GUILayout.Space(4);

            EditorGUILayout.PropertyField(competence1Data);
            EditorGUILayout.PropertyField(levelUnlockCompetence1);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(competence2Data);
            EditorGUILayout.PropertyField(levelUnlockCompetence2);
            GUILayout.Space(4);
                
            EditorGUILayout.PropertyField(passifData);
            EditorGUILayout.PropertyField(levelUnlockPassif);
            GUILayout.Space(4);
        }
        
        GUILayout.Space(15);
        
        // LEVEL PART
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Levels", moduleNameStyle);
            GUILayout.Space(3);

            // Adaptation de la taille de la liste
            levelSize = levels.arraySize;
            levelSize = EditorGUILayout.IntField ("NbrNiveaux", levelSize);
            
            if(levelSize != levels.arraySize)
            {
                while(levelSize > levels.arraySize) {
                    levels.InsertArrayElementAtIndex(levels.arraySize);
                }
                while(levelSize < levels.arraySize) {
                    levels.DeleteArrayElementAtIndex(levels.arraySize - 1);
                }
            }
            
            if(currentScript.isEnnemy)
                paternSize = EditorGUILayout.IntField ("Paterne Size", paternSize);
            
            
            so.ApplyModifiedProperties();
            
            for (int i = 0; i < levels.arraySize; i++)
            {
                GUILayout.Space(12);
                
                levels = so.FindProperty("levels");
                SerializedProperty MyListRef = levels.GetArrayElementAtIndex(i);
                
                SerializedProperty PV = MyListRef.FindPropertyRelative("PV");
                SerializedProperty PM = MyListRef.FindPropertyRelative("PM");
                SerializedProperty movePatern = MyListRef.FindPropertyRelative("movePatern");
                
                SerializedProperty force = MyListRef.FindPropertyRelative("force");
                SerializedProperty defense = MyListRef.FindPropertyRelative("defense");
                SerializedProperty vitesse = MyListRef.FindPropertyRelative("vitesse");
                SerializedProperty agilite = MyListRef.FindPropertyRelative("agilite");
                SerializedProperty chance = MyListRef.FindPropertyRelative("chance");

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Level " + (i + 1), moduleNameStyle);
                    GUILayout.Space(10);
                    
                    
                    // Partie General
                    GUILayout.Label("General", titreStyle);
                    
                    EditorGUILayout.PropertyField(PV);
                    
                    if (!currentScript.isEnnemy)
                    {
                        EditorGUILayout.PropertyField(PM);
                    }
                    else
                    {
                        //paternSize = movePatern.arraySize;
                        
                        if (paternSize != movePatern.arraySize)
                        {
                            movePatern.ClearArray();
                
                            for (int j = 0; j < paternSize; j++)
                            {
                                movePatern.InsertArrayElementAtIndex(movePatern.arraySize);

                                SerializedProperty currentList = movePatern.GetArrayElementAtIndex(movePatern.arraySize - 1)
                                    .FindPropertyRelative("list");

                                for (int k = 0; k < paternSize; k++)
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
                    }

                    GUILayout.Space(10);
                    
                    
                    // Partie Stats
                    GUILayout.Label("Stats", titreStyle);
                    
                    EditorGUILayout.PropertyField(force);
                    EditorGUILayout.PropertyField(defense);
                    EditorGUILayout.PropertyField(vitesse);
                    EditorGUILayout.PropertyField(agilite);
                    EditorGUILayout.PropertyField(chance);
                }
            }
        }
        
        so.ApplyModifiedProperties();
    }
}
