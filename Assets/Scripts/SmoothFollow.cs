using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{

    // Transform of target object, defaults to player
    public Transform target;

    public Vector3 offset = Vector3.zero;
    // Time in seconds for smoothing
    public float smoothTime = 0.5f;
    // Distance from target
    public float distance = 5.0f;

    public float minimumY = 0.0f;
    public float maximumY = 200f;

    // Local copy of transform to reduce component lookups
    Transform _transform;
    // Velocity of camera smoothing
    Vector3 _smoothVelocity;

    void Start()
    {
        _transform = transform;

        if (target == null)
        {
            // Point at player by default
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Point camera towards target
            Vector3 targetPosition = target.position + offset;

            targetPosition = new Vector3(targetPosition.x, Mathf.Max(Mathf.Min(targetPosition.y, maximumY), minimumY), targetPosition.z);



            targetPosition.z -= distance;

            _transform.position = Vector3.SmoothDamp(_transform.position, targetPosition, ref _smoothVelocity, smoothTime);
        }
    }
}
