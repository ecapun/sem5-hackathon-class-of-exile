using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;
    public Vector3 offset = new Vector3(5f, 6f, -5f);
    public float smoothSpeed = 10f;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // WICHTIG: Kamera NICHT drehen!
        // Rotation bleibt so wie sie im Inspector steht.
    }
}
