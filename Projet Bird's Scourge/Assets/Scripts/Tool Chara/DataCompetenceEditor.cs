using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(DataCompetence))]
public class DataCompetenceEditor : Editor
{
    private SerializedObject so;
    private GUIStyle moduleNameStyle = new GUIStyle();
    private GUIStyle titreStyle = new GUIStyle();
    private DataCompetence currentScript;

    [Header("General")] 
    private SerializedProperty propName;
    private SerializedProperty VFXType;
    
    private SerializedProperty propDegatsMax;
    private SerializedProperty propCooldown;
    
    [Header("Paterne")]
    private SerializedProperty propHasCustomPaterne;
    private SerializedProperty propPaterne;
    private SerializedProperty propPaternePrefab;
    private SerializedProperty propPortee;
    
    [Header("Effet / Alteration")]
    private SerializedProperty propDoEffet;
    private SerializedProperty propEffet;
    private SerializedProperty propDoAlteration;
    private SerializedProperty propAlteration;


    [Header("Levels")] 
    private SerializedProperty listLevels;
    DataCompetence t;
    SerializedObject GetTarget;
    private int levelSize;
    

    private void OnEnable()
    {
        so = serializedObject;
        currentScript = target as DataCompetence;

        propName = so.FindProperty("competenceName");
        VFXType = so.FindProperty("VFXType");
        
        propDegatsMax = so.FindProperty("degatsMax");
        propCooldown = so.FindProperty("cooldown");
        
        propHasCustomPaterne = so.FindProperty("hasCustomPaterne");
        propPaterne = so.FindProperty("customPaterne");
        propPaternePrefab = so.FindProperty("paternePrefab");
        propPortee = so.FindProperty("portee");
        
        propDoEffet = so.FindProperty("doEffet");
        propEffet = so.FindProperty("effet");
        propDoAlteration = so.FindProperty("doAlteration");
        propAlteration = so.FindProperty("alteration");
        
        listLevels = so.FindProperty("levels");

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
        
        
        // GENERAL
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("General", moduleNameStyle);
            GUILayout.Space(3);
            
            EditorGUILayout.PropertyField(propName);
            EditorGUILayout.PropertyField(VFXType);
        }
        
        GUILayout.Space(10);

        
        // LEVELS
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Levels", moduleNameStyle);
            GUILayout.Space(3);
            
            //EditorGUILayout.PropertyField(listLevels);
            
            // Adaptation de la taille de la liste
            levelSize =  listLevels.arraySize;
            levelSize = EditorGUILayout.IntField ("NbrNiveaux", levelSize);
            
            if(levelSize != listLevels.arraySize)
            {
                while(levelSize > listLevels.arraySize)
                {
                    listLevels.InsertArrayElementAtIndex(listLevels.arraySize);

                    SerializedProperty currentLevel = listLevels.GetArrayElementAtIndex(listLevels.arraySize - 1);
                    SerializedProperty currentPaterne = currentLevel.FindPropertyRelative("newPaterne");
                    
                    for (int i = 0; i < 9; i++)
                    {
                        currentPaterne.InsertArrayElementAtIndex(currentPaterne.arraySize);

                        SerializedProperty newList = currentPaterne.GetArrayElementAtIndex(currentPaterne.arraySize - 1).FindPropertyRelative("list");

                        for (int j = 0; j < 9; j++)
                        {
                            newList.InsertArrayElementAtIndex(newList.arraySize);
                        }
                    }
                    
                }
                while(levelSize < listLevels.arraySize) {
                    listLevels.DeleteArrayElementAtIndex(listLevels.arraySize - 1);
                }
            }
            
            
            so.ApplyModifiedProperties();
            
            for (int i = 0; i < listLevels.arraySize; i++)
            {
                GUILayout.Space(12);
                
                listLevels = so.FindProperty("levels");
                SerializedProperty MyListRef = listLevels.GetArrayElementAtIndex(i);
                
                SerializedProperty damageMultiplier = MyListRef.FindPropertyRelative("damageMultiplier");
                SerializedProperty baseHitRate = MyListRef.FindPropertyRelative("baseHitRate");
                SerializedProperty criticalMultiplier = MyListRef.FindPropertyRelative("criticalMultiplier");
                SerializedProperty competenceDescription = MyListRef.FindPropertyRelative("competenceDescription");
                SerializedProperty competenceManaCost = MyListRef.FindPropertyRelative("competenceManaCost");
               

                SerializedProperty isCustom = MyListRef.FindPropertyRelative("isCustom");
                SerializedProperty newPaterne = MyListRef.FindPropertyRelative("newPaterne");
                SerializedProperty newPaternePrefab = MyListRef.FindPropertyRelative("newPaternePrefab");
                SerializedProperty newPortee = MyListRef.FindPropertyRelative("newPortee");
                
                SerializedProperty newEffet = MyListRef.FindPropertyRelative("newEffet");
                SerializedProperty summonedUnit = MyListRef.FindPropertyRelative("summonedUnit");
                SerializedProperty healedPV = MyListRef.FindPropertyRelative("healedPV");
                SerializedProperty createdBuff = MyListRef.FindPropertyRelative("createdBuff");
                
                SerializedProperty newAlteration = MyListRef.FindPropertyRelative("newAlteration");

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Level " + (i + 1), moduleNameStyle);
                    GUILayout.Space(10);
                    
                    
                    // Partie Dégâts
                    GUILayout.Label("Degats", titreStyle);
                    
                    EditorGUILayout.PropertyField(damageMultiplier);
                    EditorGUILayout.PropertyField(baseHitRate);
                    EditorGUILayout.PropertyField(criticalMultiplier);
                    EditorGUILayout.PropertyField(competenceDescription);
                    EditorGUILayout.PropertyField(competenceManaCost);
                  

                    GUILayout.Space(10);
                    
                    
                    // Partie Paterne
                    GUILayout.Label("Paterne", titreStyle);
                    
                    EditorGUILayout.PropertyField(isCustom);

                    if (currentScript.levels[i].isCustom)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox, new [] {GUILayout.MinWidth(200)}))
                        {
                            for (int k = 0; k < 9; k++) 
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    SerializedProperty currentList = newPaterne.GetArrayElementAtIndex(k).FindPropertyRelative("list");
                                    
                                    for (int j = 0; j < 9; j++)
                                    {
                                        if (k != 4 || j != 4)
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
                        
                    else
                    {
                        GUILayout.Space(3);
                            
                        EditorGUILayout.PropertyField(newPaternePrefab);
                        EditorGUILayout.PropertyField(newPortee);
                    }
                    
                    
                    GUILayout.Space(10);
                    
                    
                    // Partie Effets / Alteration
                    GUILayout.Label("Effet / Alteration", titreStyle);
                    
                    EditorGUILayout.PropertyField(newEffet);

                    switch (currentScript.levels[i].newEffet)
                    {
                        case DataCompetence.Effets.invocation : 
                            EditorGUILayout.PropertyField(summonedUnit);
                            break;
                        
                        case DataCompetence.Effets.soin :
                            EditorGUILayout.PropertyField(healedPV);
                            break;
                        
                        case DataCompetence.Effets.buff :
                            EditorGUILayout.PropertyField(createdBuff);
                            break;
                    }


                    GUILayout.Space(5);
                    
                    EditorGUILayout.PropertyField(newAlteration);
                    
                }
            }
        }
        
        EditorUtility.SetDirty(target);
        
        
        so.ApplyModifiedProperties();
    }
}


