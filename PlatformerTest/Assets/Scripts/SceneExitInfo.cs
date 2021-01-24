using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneExitInfo : MonoBehaviour
{
    [SerializeField]
    string nextSceneName;
    [SerializeField]
    int nextSceneEnterenceId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            SceneEnters.enteranceId = nextSceneEnterenceId;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
