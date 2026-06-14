using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloonsTD.Core
{
    public class SceneLoader : SingletonMono<SceneLoader>
    {
        public static string NextScene { get; private set; }

        public void Load(string sceneName)
        {
            NextScene = sceneName;
            Debug.Log($"[SceneLoader] → Loading scene, target={sceneName}");
            SceneManager.LoadScene("Loading");
        }

        // Direct load — bypass loading screen (used for Loading scene itself)
        public static void LoadDirect(string sceneName)
        {
            Debug.Log($"[SceneLoader] Direct load: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
