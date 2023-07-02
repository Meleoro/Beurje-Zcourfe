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

        
        
        
        
        //EditorGUILayout.PropertyField(propPaterne);
        
        /*if (propPaterne.arraySize == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                propPaterne.InsertArrayElementAtIndex(propPaterne.arraySize);

                SerializedProperty newList = propPaterne.GetArrayElementAtIndex(propPaterne.arraySize - 1).FindPropertyRelative("list");

                for (int j = 0; j < 7; j++)
                {
                    newList.InsertArrayElementAtIndex(newList.arraySize);
                }
            }
        }
        
        GUILayout.Space(5);*/
        
        // GENERAL
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("General", moduleNameStyle);
            GUILayout.Space(3);
            
            EditorGUILayout.PropertyField(propName);
        }
        
        GUILayout.Space(10);
        
        /*
        GUILayout.Space(20);
        
        // PATERNE
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Paterne", moduleNameStyle);
            GUILayout.Space(3);

            EditorGUILayout.PropertyField(propHasCustomPaterne);

            if (currentScript.hasCustomPaterne)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox, new [] {GUILayout.MinWidth(150)}))
                {
                    for (int i = 0; i < 7; i++)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            SerializedProperty currentList =
                                propPaterne.GetArrayElementAtIndex(i).FindPropertyRelative("list");
                            
                            for (int j = 0; j < 7; j++)
                            {
                                if (i != 3 || j != 3)
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
                EditorGUILayout.PropertyField(propPaternePrefab);
                EditorGUILayout.PropertyField(propPortee);

                if (currentScript.portee < 0)
                {
                    currentScript.portee = 0;
                }
            }
        }
        
        GUILayout.Space(20);
        
        // EFFETS / ALTERATIONS
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Effets / Alterations", moduleNameStyle);
            GUILayout.Space(3);

            EditorGUILayout.PropertyField(propDoEffet);
            if (currentScript.doEffet)
            {
                EditorGUILayout.PropertyField(propEffet);
                GUILayout.Space(10);
            }
            
            EditorGUILayout.PropertyField(propDoAlteration);
            if (currentScript.doAlteration)
            {
                EditorGUILayout.PropertyField(propAlteration);
            }
        }
        
        GUILayout.Space(20);*/
        
        
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
                
                SerializedProperty newDegatsMin = MyListRef.FindPropertyRelative("newDegatsMin");
                SerializedProperty newDegatsMax = MyListRef.FindPropertyRelative("newDegatsMax");
                
                SerializedProperty isCustom = MyListRef.FindPropertyRelative("isCustom");
                SerializedProperty newPaterne = MyListRef.FindPropertyRelative("newPaterne");
                SerializedProperty newPaternePrefab = MyListRef.FindPropertyRelative("newPaternePrefab");
                SerializedProperty newPortee = MyListRef.FindPropertyRelative("newPortee");
                
                SerializedProperty newEffet = MyListRef.FindPropertyRelative("newEffet");
                
                SerializedProperty newAlteration = MyListRef.FindPropertyRelative("newAlteration");

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Level " + (i + 1), moduleNameStyle);
                    GUILayout.Space(10);
                    
                    
                    // Partie Dégâts
                    GUILayout.Label("Degats", titreStyle);
                    
                    EditorGUILayout.PropertyField(newDegatsMin);
                    EditorGUILayout.PropertyField(newDegatsMax);

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
                    
                    
                    GUILayout.Space(3);
                    
                    EditorGUILayout.PropertyField(newAlteration);
                    
                }
            }
        }
        
        EditorUtility.SetDirty(target);
        
        
        so.ApplyModifiedProperties();
    }
}


