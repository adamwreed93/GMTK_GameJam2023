using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _movementSpeed = 0f;
    [SerializeField] private float _range = 100f;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
        Destroy(gameObject, 7);
    }

    private void Update()
    {
        // Movement
        transform.position += transform.forward * _movementSpeed * Time.deltaTime;

        // Destroy bullet after it has traveled a certain range
        if (Vector3.Distance(_startPosition, transform.position) > _range)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }
}
