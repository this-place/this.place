using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

// COMMUNITY THREAD LINK https://forum.unity.com/threads/replace-game-object-with-prefab.24311/
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010
//Modified by Kristian Helle Jespersen
//June 2011
//Modified by Connor Cadellin McKee for Excamedia
//April 2015
//Modified by Fernando Medina (fermmmm)
//April 2015
//Modified by Julien Tonsuso (www.julientonsuso.com)
//July 2015
//Changed into editor window and added instant preview in scene view
//Modified by Alex Dovgodko
//June 2017
//Made changes to make things work with Unity 5.6.1
//March 2018
//Added link to community thread, booleans to chose if scale and rotation are applied, mark scene as dirty, changed menu item to tools. By Hyper

public class ReplaceWithPrefab : EditorWindow
{
    public GameObject Prefab;
    public GameObject[] ObjectsToReplace;
    public List<GameObject> TempObjects = new List<GameObject>();
    public bool KeepOriginalNames = true;
    public bool EditMode = false;
    public bool ApplyRotation = true;
    public bool ApplyScale = true;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/ReplaceWithPrefab")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ReplaceWithPrefab window = (ReplaceWithPrefab)EditorWindow.GetWindow(typeof(ReplaceWithPrefab));
        window.Show();
    }
    void OnSelectionChange()
    {
        GetSelection();
        Repaint();
    }
    void OnGUI()
    {
        EditMode = GUILayout.Toggle(EditMode, "Edit");
        if (GUI.changed)
        {
            if (EditMode)
                GetSelection();
            else
                ResetPreview();
        }
        KeepOriginalNames = GUILayout.Toggle(KeepOriginalNames, "Keep names");
        ApplyRotation = GUILayout.Toggle(ApplyRotation, "Apply rotation");
        ApplyScale = GUILayout.Toggle(ApplyScale, "Apply scale");
        GUILayout.Space(5);
        if (EditMode)
        {
            ResetPreview();

            GUI.color = Color.yellow;
            if (Prefab != null)
            {
                GUILayout.Label("Prefab: ");
                GUILayout.Label(Prefab.name);
            }
            else
            {
                GUILayout.Label("No prefab selected");
            }
            GUI.color = Color.white;

            GUILayout.Space(5);
            GUILayout.BeginScrollView(new Vector2());
            foreach (GameObject go in ObjectsToReplace)
            {
                GUILayout.Label(go.name);
                if (Prefab != null && go.name.Contains(Prefab.name))
                {
                    GameObject newObject;
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
                    newObject.transform.SetParent(go.transform.parent, true);
                    newObject.transform.localPosition = go.transform.localPosition;
                    if (ApplyRotation)
                    {
                        newObject.transform.localRotation = go.transform.localRotation;
                    }
                    if (ApplyScale)
                    {
                        newObject.transform.localScale = go.transform.localScale;
                    }
                    TempObjects.Add(newObject);
                    if (KeepOriginalNames)
                        newObject.transform.name = go.transform.name;
                    go.SetActive(false);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
            {
                foreach (GameObject go in ObjectsToReplace)
                {
                    if (go.name.Contains(Prefab.name))
                    {
                        DestroyImmediate(go);
                    }
                }
                EditMode = false;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); // So that we don't forget to save...
            };
            if (GUILayout.Button("Cancel"))
            {
                ResetPreview();
                EditMode = false;
            };
            GUILayout.EndHorizontal();
        }
        else
        {
            ObjectsToReplace = new GameObject[0];
            TempObjects.Clear();
            Prefab = null;
        }

    }
    void OnDestroy()
    {
        ResetPreview();
    }
    void GetSelection()
    {
        if (EditMode && Selection.activeGameObject != null)
        {
            PrefabType t = PrefabUtility.GetPrefabType(Selection.activeGameObject);
            if (t == PrefabType.Prefab) //Here goes the fix
            {
                Prefab = Selection.activeGameObject;
            }
            else
            {
                ResetPreview();
                ObjectsToReplace = Selection.gameObjects;
            }
        }
    }
    void ResetPreview()
    {
        if (TempObjects != null)
        {
            foreach (GameObject go in TempObjects)
            {
                DestroyImmediate(go);
            }
        }
        foreach (GameObject go in ObjectsToReplace)
        {
            go.SetActive(true);
        }
        TempObjects.Clear();
    }
}