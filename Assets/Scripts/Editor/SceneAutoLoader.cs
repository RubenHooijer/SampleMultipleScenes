#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
static class SceneAutoLoader {

    static SceneAutoLoader() {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    //Function that gets called whenever a scene is opened
    private static void OnSceneOpened(Scene scene, OpenSceneMode mode) {
        if (!TryGetNecessaryScenes(scene, out NecessaryScenes necessaryScenes)) { return; }

        for (int i = 0; i < necessaryScenes.ScenePaths.Count; i++) {
            Scene sceneFromPath = EditorSceneManager.GetSceneByPath(necessaryScenes.ScenePaths[i]);
            if (sceneFromPath.isLoaded) { continue; }

            Scene autoloadScene = EditorSceneManager.OpenScene(necessaryScenes.ScenePaths[i], OpenSceneMode.Additive);
            EditorSceneManager.MoveSceneAfter(scene, autoloadScene);
            SetExpanded(autoloadScene, false);
        }
    }

    //Returns NecessaryScenes object if available
    private static bool TryGetNecessaryScenes(Scene scene, out NecessaryScenes editorSceneReference) {
        List<GameObject> rootGameobjects = new List<GameObject>();
        scene.GetRootGameObjects(rootGameobjects);

        NecessaryScenes necessaryScenes = null;
        rootGameobjects.FirstOrDefault(x => x.TryGetComponent(out necessaryScenes));
        editorSceneReference = necessaryScenes;

        return editorSceneReference != null;
    }

    //Sets the scene to expanded
    ///Credit to Peter77 https://forum.unity.com/threads/how-to-collapse-hierarchy-scene-nodes-via-script.605245/
    private static void SetExpanded(Scene scene, bool expand) {
        foreach (var window in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>()) {
            if (window.GetType().Name != "SceneHierarchyWindow")
                continue;

            var method = window.GetType().GetMethod("SetExpandedRecursive",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance, null,
                new[] { typeof(int), typeof(bool) }, null);

            if (method == null) {
                Debug.LogError("Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                return;
            }

            var field = scene.GetType().GetField("m_Handle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field == null) {
                Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                return;
            }

            var sceneHandle = field.GetValue(scene);
            method.Invoke(window, new[] { sceneHandle, expand });
        }
    }

}
#endif