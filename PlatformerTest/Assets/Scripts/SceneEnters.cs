using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnters : MonoBehaviour
{
    [SerializeField]
    EnteranceInfo[] enterances;
    [SerializeField]
    Player player;

    public static int enteranceId;
    public static int framesSinceEnter;
    public static bool enterAnimationOver;

    void Start()
    {
        player.SetPosition(enterances[enteranceId].position.position);
        framesSinceEnter = 0;
        enterAnimationOver = true;
    }

    void FixedUpdate()
    {
        
    }
}


[System.Serializable]
class EnteranceInfo
{
    public Transform position;
}
