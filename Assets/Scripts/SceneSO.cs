using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadCallback {

    //Scene we're trying to load
    public SceneSO Scene;
    public AsyncOperation AsyncOperation;
    public AsyncOperationHandle<SceneInstance> OperationHandle;

    //Callback event
    public event Action<SceneSO> Completed;

    public SceneLoadCallback(SceneSO scene, AsyncOperation asyncOperation) {
        Completed = delegate (SceneSO scene) { };
        Scene = scene;
        AsyncOperation = asyncOperation;
        OperationHandle = new AsyncOperationHandle<SceneInstance>();

        asyncOperation.completed += OnAsyncOperationCompleted;
    }

    public SceneLoadCallback(SceneSO scene, AsyncOperationHandle<SceneInstance> operationHandle) {
        Completed = delegate (SceneSO scene) { };
        Scene = scene;
        AsyncOperation = null;
        OperationHandle = operationHandle;

        OperationHandle.Completed += OnAsyncOperationHandleCompleted;
    }

    private void OnAsyncOperationCompleted(AsyncOperation asyncOperation) {
        Completed.Invoke(Scene);
    }

    private void OnAsyncOperationHandleCompleted(AsyncOperationHandle<SceneInstance> operationHandle) {
        Completed.Invoke(Scene);
    }

}

[CreateAssetMenu(fileName = "NewScene", menuName = "CustomSO/SceneSO")]
public class SceneSO : ScriptableObject {

    public enum LoadState {

        Unloaded = 0,
        Loaded = 1,

    }

    public readonly UnityEvent<SceneSO> OnLoadedCallback = new UnityEvent<SceneSO>();
    public readonly UnityEvent<SceneSO> OnUnloadedCallback = new UnityEvent<SceneSO>();

    public LoadState State { get; private set; } = LoadState.Unloaded;
    public bool IsActive => SceneManager.GetActiveScene() == SceneInstance.Scene;
    public SceneInstance SceneInstance => SceneReference.OperationHandle.Convert<SceneInstance>().Result;

    public AssetReference SceneReference;

    public AsyncOperationHandle<SceneInstance> Preload() {
        if (State != LoadState.Unloaded) { return default; }
        AsyncOperationHandle<SceneInstance> operationHandle = SceneReference.LoadSceneAsync(LoadSceneMode.Additive, false);
        return operationHandle;
    }

    public SceneLoadCallback Load() {
        switch (State) {
            case LoadState.Unloaded:
                return LoadScene();
        }

        Debug.LogWarning("You are trying to load a loaded scene");
        return default;
    }

    public AsyncOperationHandle<SceneInstance> Unload() {
        AsyncOperationHandle<SceneInstance> operationHandle = SceneReference.UnLoadScene();
        operationHandle.Completed += OnUnloaded;
        return operationHandle;
    }

    private void OnLoaded(AsyncOperationHandle<SceneInstance> operationHandle) {
        State = LoadState.Loaded;
        OnLoadedCallback.Invoke(this);
    }

    private void OnUnloaded(AsyncOperationHandle<SceneInstance> operationHandle) {
        State = LoadState.Unloaded;
        OnUnloadedCallback.Invoke(this);
    }

    private SceneLoadCallback LoadScene() {
        AsyncOperationHandle<SceneInstance> operationHandle = SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        operationHandle.Completed += OnLoaded;
        return new SceneLoadCallback(this, operationHandle);
    }

#if UNITY_EDITOR
    public void EditorSetState(LoadState newState) {
        State = newState;
        //if (newState == LoadState.Unloaded) {
        //    Addressables.Release(SceneReference.OperationHandle);
        //}
    }

#endif

}