using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private int _bulletDamage = 1;
    [SerializeField] private Transform _bulletContainer;

    [SerializeField] private float _fireRate = 0.5f; // Time in seconds between shots

    private float _lastFireTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    void Fire()
    {
        if (Time.time - _lastFireTime < _fireRate)
        {
            // Not enough time has passed since the last shot, return early.
            return;
        }

        // Enough time has passed, fire a bullet and update the last fire time.
        _lastFireTime = Time.time;

        // Instantiate the bullet
        GameObject bulletInstance = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, transform.rotation, _bulletContainer);

        // Set the bullet speed
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetSpeed(_bulletSpeed);
            bulletScript.SetDamage(_bulletDamage);
        }
    }
}