using System.Collections;
using System.Collections.Generic;

public class SceneLoader : Singleton<SceneLoader> {

    //Enum for our different loadtypes
    public enum LoadType {

        Persistant          = 0,
        DynamicPersistant   = 1,
        Dynamic             = 2,
        DynamicAdditive     = 3,

    }

    //Create lists to manage the different kind of scenes
    private List<SceneSO> persistantScenes = new List<SceneSO>();
    private List<SceneSO> dynamicPersistantScenes = new List<SceneSO>();
    private List<SceneSO> dynamicScenes = new List<SceneSO>();
    //You may have noticed that we don't have a DynamicAdditive list, that's because they are treated the same as Dynamic scenes

    //The load function that can be called publicly
    public void LoadScene(SceneSO sceneSO, LoadType loadType) {
        StartCoroutine(LoadSceneRoutine(sceneSO, loadType));
    }

    //Adds the scene to their respective list, this can be used when u loaded a scene without using the SceneLoader.cs
    public void AddLoadedSceneToType(SceneSO sceneSO, LoadType loadType) {
        switch (loadType) {
            case LoadType.Persistant:
                persistantScenes.Add(sceneSO);
                break;
            case LoadType.DynamicPersistant:
                dynamicPersistantScenes.Add(sceneSO);
                break;
            case LoadType.Dynamic:
                dynamicScenes.Add(sceneSO);
                break;
            case LoadType.DynamicAdditive:
                dynamicScenes.Add(sceneSO);
                break;
        }
    }

    //The coroutine load function, which we can use to load new scenes with the correct type
    private IEnumerator LoadSceneRoutine(SceneSO sceneSO, LoadType loadType) {
        switch (loadType) {
            case LoadType.Persistant:
                LoadSceneInList(sceneSO, persistantScenes);
                break;
            case LoadType.DynamicPersistant:
                LoadSceneInList(sceneSO, dynamicPersistantScenes);
                break;
            case LoadType.Dynamic:
                //Unloading all other dynamic scenes, since this scene is not additive
                yield return StartCoroutine(UnloadScenesRoutine(dynamicScenes));

                LoadSceneInList(sceneSO, dynamicScenes);
                break;
            case LoadType.DynamicAdditive:
                LoadSceneInList(sceneSO, dynamicScenes);
                break;
            default:
                break;
        }
    }

    //Loads the scene and adds it to the scenelist
    private void LoadSceneInList(SceneSO sceneSO, List<SceneSO> sceneList) {
        sceneSO.Load();

        sceneList.Add(sceneSO);
    }

    //Unloads a list of scenes
    private IEnumerator UnloadScenesRoutine(List<SceneSO> scenes) {
        for (int i = 0; i < scenes.Count; i++) {
            yield return scenes[i].Unload();
        }
    }

}