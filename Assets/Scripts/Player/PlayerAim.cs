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
            mainCamera = Camera.main;

        // Mặt phẳng ngang tại Y = 0
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void Update()
    {
        AimToMouse();
    }

    private void AimToMouse()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}