using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneExitInfo : MonoBehaviour
{
    [SerializeField]
    string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>())
            SceneManager.LoadScene(nextSceneName);
    }
}
