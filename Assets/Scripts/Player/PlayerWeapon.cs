using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private Camera mainCamera;

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
            playerAnimation = GetComponent<PlayerAnimation>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
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

        nextFireTime = Time.time + (1f / fireRate);
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("PlayerWeapon: Chưa gán Bullet Prefab!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("PlayerWeapon: Chưa gán FirePoint!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("PlayerWeapon: Không tìm thấy Main Camera!");
            return;
        }

        // ========================================
        // 1. Tạo Ray từ Camera qua vị trí chuột
        // ========================================

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Mặt phẳng ngang đi qua Player
        Plane aimPlane = new Plane(
            Vector3.up,
            transform.position
        );

        if (!aimPlane.Raycast(ray, out float enter))
            return;

        Vector3 aimPoint = ray.GetPoint(enter);

        // ========================================
        // 2. Hướng từ nòng súng tới crosshair
        // ========================================

        Vector3 shootDirection =
            aimPoint - firePoint.position;

        // QUAN TRỌNG:
        // khóa trục Y để đạn luôn bay ngang mặt đất
        shootDirection.y = 0f;

        if (shootDirection.sqrMagnitude < 0.001f)
            return;

        shootDirection.Normalize();

        // ========================================
        // 3. Rotation của viên đạn
        // ========================================

        Quaternion bulletRotation =
            Quaternion.LookRotation(
                shootDirection,
                Vector3.up
            );

        // ========================================
        // 4. Tạo Bullet
        // ========================================

        Instantiate(
            bulletPrefab,
            firePoint.position,
            bulletRotation
        );

        // ========================================
        // 5. Muzzle Flash
        // ========================================

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            muzzleFlash.Play();
        }

        // ========================================
        // 6. Trừ đạn
        // ========================================

        currentAmmo--;

        Debug.Log(
            $"Ammo: {currentAmmo}/{magazineSize}"
        );
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

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        Debug.Log("Reloading...");

        if (playerAnimation != null)
        {
            playerAnimation.StartReload();
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;

        isReloading = false;

        if (playerAnimation != null)
        {
            playerAnimation.FinishReload();
        }

        Debug.Log("Reload Complete!");
    }
}