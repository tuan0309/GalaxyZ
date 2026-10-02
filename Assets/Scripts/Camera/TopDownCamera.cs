using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    [SerializeField] private float smoothSpeed = 8f;

    [Header("Look At")]
    [SerializeField] private bool lookAtTarget = true;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        if (lookAtTarget)
        {
            transform.LookAt(target.position);
        }
    }
}