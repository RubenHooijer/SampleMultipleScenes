using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour {

    [Serializable]
    public struct SceneWithLoadType {

        public SceneSO Scene;
        public SceneLoader.LoadType Type;

    }

    [SerializeField] private SceneSO persistantSceneSO;
    [SerializeField] private List<SceneWithLoadType> scenesInLoadOrder;
    [SerializeField] private SceneSO activeScene;

#if UNITY_EDITOR
    private void Awake() {
        persistantSceneSO.EditorSetState(SceneSO.LoadState.Unloaded);
        for (int i = 0; i < scenesInLoadOrder.Count; i++) {
            scenesInLoadOrder[i].Scene.EditorSetState(SceneSO.LoadState.Unloaded);
        }
    }
#endif

    private IEnumerator Start() {
        //Load persistant scene, where the SceneLoader resides
        yield return persistantSceneSO.Load().AsyncOperation;

        //Load other scenes in order and by LoadType
        for (int i = 0; i < scenesInLoadOrder.Count; i++) {
            SceneLoader.Instance.LoadScene(scenesInLoadOrder[i].Scene, scenesInLoadOrder[i].Type);
        }
        //Unload Initialization manually, since it's not an Addressable scene
        SceneManager.UnloadSceneAsync("Initialization");

        //Wait till activeScene is loaded to activate it
        yield return new WaitUntil(() => activeScene.State == SceneSO.LoadState.Loaded);

        //Activate the activeScene
        SceneManager.SetActiveScene(activeScene.SceneInstance.Scene);
    }

}