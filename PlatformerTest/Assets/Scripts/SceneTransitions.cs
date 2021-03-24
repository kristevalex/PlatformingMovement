using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitions : MonoBehaviour
{
    AsyncOperation asyncOperation;
    string nextScene;

    public void LoadScene(string name)
    {
        nextScene = name;
        StartCoroutine(load());
    }

    IEnumerator load()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        asyncOperation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;
        yield return asyncOperation;
        SceneManager.UnloadSceneAsync(currentScene);
    }
}