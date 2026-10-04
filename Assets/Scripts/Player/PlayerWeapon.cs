using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private PlayerAnimation playerAnimation;

    [Header("Weapon Settings")]
    [SerializeField] private float fireRate = 5f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int currentAmmo = 30;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.5f;

    private float nextFireTime;
    private bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        if (playerAnimation == null)
        {
            playerAnimation =
                GetComponent<PlayerAnimation>();
        }

        currentAmmo = magazineSize;
    }

    private void Update()
    {
        if (isReloading)
            return;

        HandleReload();
        HandleShooting();
    }

    private void HandleShooting()
    {
        // Chuột trái để bắn
        if (!Input.GetMouseButton(0))
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        Shoot();

        nextFireTime =
            Time.time + (1f / fireRate);
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning(
                "PlayerWeapon: Chưa gán Bullet Prefab!"
            );
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                "PlayerWeapon: Chưa gán FirePoint!"
            );
            return;
        }

        // PlayerAim đã xoay Player về phía chuột.
        // Bullet chỉ cần bay theo forward của Player.
        Vector3 shootDirection =
            transform.forward;

        shootDirection.y = 0f;

        if (shootDirection.sqrMagnitude < 0.001f)
            return;

        shootDirection.Normalize();

        Quaternion bulletRotation =
            Quaternion.LookRotation(
                shootDirection,
                Vector3.up
            );

        Instantiate(
            bulletPrefab,
            firePoint.position,
            bulletRotation
        );

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior
                    .StopEmittingAndClear
            );

            muzzleFlash.Play();
        }

        currentAmmo--;
    }

    private void HandleReload()
    {
        if (!Input.GetKeyDown(KeyCode.R))
            return;

        if (currentAmmo >= magazineSize)
            return;

        StartReload();
    }

    private void StartReload()
    {
        if (isReloading)
            return;

        if (currentAmmo >= magazineSize)
            return;

        StartCoroutine(
            ReloadRoutine()
        );
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (playerAnimation != null)
        {
            playerAnimation.StartReload();
        }

        yield return new WaitForSeconds(
            reloadTime
        );

        currentAmmo =
            magazineSize;

        isReloading = false;

        if (playerAnimation != null)
        {
            playerAnimation.FinishReload();
        }
    }
}