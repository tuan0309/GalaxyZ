using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

    [Header("Dash Effect")]
    [SerializeField] private TrailRenderer dashTrail;

    private CharacterController controller;

    private Vector3 moveDirection;

    private bool isDashing;
    private bool canDash = true;

    public bool IsDashing => isDashing;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isDashing)
            return;

        HandleMovement();
        HandleDash();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(
            horizontal,
            0f,
            vertical
        );

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        controller.Move(
            moveDirection *
            moveSpeed *
            Time.deltaTime
        );
    }

    private void HandleDash()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        if (!canDash)
            return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dashDirection = new Vector3(
            horizontal,
            0f,
            vertical
        );

        // Nếu không bấm WASD
        // dash theo hướng Player đang nhìn
        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = transform.forward;
            dashDirection.y = 0f;
        }

        dashDirection.Normalize();

        // Bật trail
        if (dashTrail != null)
        {
            dashTrail.Clear();
            dashTrail.emitting = true;
        }

        float dashSpeed =
            dashDistance / dashDuration;

        float timer = 0f;

        while (timer < dashDuration)
        {
            controller.Move(
                dashDirection *
                dashSpeed *
                Time.deltaTime
            );

            timer += Time.deltaTime;

            yield return null;
        }

        // Tắt trail
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        isDashing = false;

        yield return new WaitForSeconds(
            dashCooldown
        );

        canDash = true;
    }
}