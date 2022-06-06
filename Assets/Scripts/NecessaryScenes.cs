#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class NecessaryScenes : MonoBehaviour {

    ///Put scene paths that you want to auto load when opening the scene here.",
    ///You can get a scenes' path by following these steps
    ///[1] finding it in the project folders
    ///[2] right-clicking any scene
    ///[3] copy path
    ///[4] You can then paste the path into this list.
    [SerializeField] public List<string> ScenePaths;

}
#endif