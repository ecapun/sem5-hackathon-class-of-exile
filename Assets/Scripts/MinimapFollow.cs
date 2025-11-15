using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;
    public float height = 40f;

    void LateUpdate()
    {
        var pos = player.position;
        pos.y += height;
        transform.position = pos;
    }
}
