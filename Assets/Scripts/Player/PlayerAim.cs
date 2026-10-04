using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [Header("Aim Settings")]
    [SerializeField] private float rotationSpeed = 15f;

    private Plane groundPlane;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        groundPlane = new Plane(
            Vector3.up,
            transform.position
        );
    }

    private void Update()
    {
        AimToMouse();
    }

    private void AimToMouse()
    {
        if (mainCamera == null)
            return;

        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );

        // Cập nhật mặt phẳng theo độ cao Player
        groundPlane.SetNormalAndPosition(
            Vector3.up,
            transform.position
        );

        if (!groundPlane.Raycast(ray, out float enter))
            return;

        Vector3 mouseWorldPosition =
            ray.GetPoint(enter);

        Vector3 direction =
            mouseWorldPosition -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }
}