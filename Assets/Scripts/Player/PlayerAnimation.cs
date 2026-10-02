using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public enum PlayerAnimState
    {
        Normal,
        Reloading,
        Dead
    }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerRoot;

    [Header("Movement Animations")]
    [SerializeField] private string idleAnimation = "Idle_Shoot_Ar";
    [SerializeField] private string walkForwardAnimation = "WalkFront_Shoot_AR";
    [SerializeField] private string walkBackwardAnimation = "WalkBack_Shoot_AR";
    [SerializeField] private string walkLeftAnimation = "WalkLeft_Shoot_AR";
    [SerializeField] private string walkRightAnimation = "WalkRight_Shoot_AR";

    [Header("Action Animations")]
    [SerializeField] private string reloadAnimation = "Reload";
    [SerializeField] private string deathAnimation = "Die";

    [Header("Transition")]
    [SerializeField] private float crossFadeTime = 0.1f;

    private PlayerAnimState currentState = PlayerAnimState.Normal;
    private string currentAnimation;

    public PlayerAnimState CurrentState => currentState;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (playerRoot == null)
            playerRoot = transform;
    }

    private void Update()
    {
        if (currentState != PlayerAnimState.Normal)
            return;

        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude < 0.01f)
        {
            PlayAnimation(idleAnimation);
            return;
        }

        inputDirection.Normalize();

        Vector3 localDirection =
            playerRoot.InverseTransformDirection(inputDirection);

        float forward = localDirection.z;
        float right = localDirection.x;

        if (Mathf.Abs(forward) >= Mathf.Abs(right))
        {
            if (forward >= 0f)
                PlayAnimation(walkForwardAnimation);
            else
                PlayAnimation(walkBackwardAnimation);
        }
        else
        {
            if (right >= 0f)
                PlayAnimation(walkRightAnimation);
            else
                PlayAnimation(walkLeftAnimation);
        }
    }

    public void StartReload()
    {
        if (currentState == PlayerAnimState.Dead)
            return;

        currentState = PlayerAnimState.Reloading;

        PlayAnimation(
            reloadAnimation,
            true
        );
    }

    public void FinishReload()
    {
        if (currentState == PlayerAnimState.Dead)
            return;

        currentState = PlayerAnimState.Normal;

        // Xóa tên animation hiện tại
        // để movement update ngay animation mới.
        currentAnimation = "";
    }

    public void PlayDeath()
    {
        currentState = PlayerAnimState.Dead;

        PlayAnimation(
            deathAnimation,
            true
        );
    }

    private void PlayAnimation(
        string animationName,
        bool forceRestart = false
    )
    {
        if (animator == null)
            return;

        if (string.IsNullOrEmpty(animationName))
            return;

        if (!forceRestart &&
            currentAnimation == animationName)
        {
            return;
        }

        currentAnimation = animationName;

        animator.CrossFade(
            animationName,
            crossFadeTime,
            0,
            0f
        );
    }
}