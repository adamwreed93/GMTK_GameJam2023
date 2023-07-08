using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField] private float _gravity = -9.81f;

    [SerializeField] private GameObject _gFXObject; //The 3D model or object that will rotate to face the mouse.
    [SerializeField] private float _speed; //The speed at which the player moves.
    [SerializeField] private float _runMultiplier = 2f; //Multiplier applied to speed when the player is running.
    [SerializeField] private float _accelerationTime = 0.2f; //Time it takes to reach full speed.

    private float _fallSpeed = 0f;
    private float _defaultSpeed; //The base speed of the player, stored to revert to after running.
    private float _targetSpeed; // The speed we are currently trying to reach.
    private float _currentSpeed; // The current actual speed

    private Plane _plane; //A horizontal plane at y = 0 used for determining the position of the mouse cursor in world space.



    void Start()
    {
        _defaultSpeed = _speed;
        _targetSpeed = _speed;
        _currentSpeed = _speed;
        //transform.position = new Vector3(0, -0.1f, 0);
        _plane = new Plane(Vector3.up, Vector3.zero);

        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        Gravity();
        Movement();
        RotateGFXToMouse();
    }

    private void Gravity()
    {
        // Check if the player is grounded. If not, apply gravity.
        if (_controller.isGrounded && _fallSpeed < 0)
        {
            _fallSpeed = 0f;
        }
        else
        {
            _fallSpeed += _gravity * Time.deltaTime;
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        _targetSpeed = Input.GetKey(KeyCode.LeftShift) ? _defaultSpeed * _runMultiplier : _defaultSpeed;

        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime / _accelerationTime);

        if (!_controller.isGrounded)
        {
            direction.y = _fallSpeed;
        }

        _controller.Move(direction * (_currentSpeed * Time.deltaTime));

        // After moving, clamp the position
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -500, 500);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -500, 500); // Assuming you want to clamp in the z direction as well
        transform.position = clampedPosition;
    }


    void RotateGFXToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - _gFXObject.transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            _gFXObject.transform.rotation = rotation;
        }
    }
}

