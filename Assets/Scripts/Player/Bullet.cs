using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float shootForce = 35f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }

    private void Start()
    {
        rb.AddForce(
            transform.forward * shootForce,
            ForceMode.Impulse
        );

        Destroy(gameObject, lifeTime);
    }
}