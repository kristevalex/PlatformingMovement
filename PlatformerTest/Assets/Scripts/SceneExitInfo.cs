using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneExitInfo : MonoBehaviour
{
    [SerializeField]
    SceneTransitions sceneTransitions;
    [SerializeField]
    string nextSceneName;
    [SerializeField]
    int nextSceneEnterenceId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            SceneEnters.enteranceId = nextSceneEnterenceId;
            sceneTransitions.LoadScene(nextSceneName);
            //SceneManager.LoadScene(nextSceneName);
        }
    }
}
