using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField] private float _gravity = -9.81f;

    [SerializeField] private GameObject _gFXObject; //The 3D model or object that will rotate to face the mouse.
    [SerializeField] private float _speed; //The speed at which the player moves.
    [SerializeField] private float _runMultiplier = 2.5f; //Multiplier applied to speed when the player is running.
    [SerializeField] private float _accelerationTime = 0.2f; //Time it takes to reach full speed.

    private float _fallSpeed = 0f;
    private float _defaultSpeed; //The base speed of the player, stored to revert to after running.
    private float _targetSpeed; // The speed we are currently trying to reach.
    private float _currentSpeed; // The current actual speed
    private bool _canSprint = true;

    private Plane _plane; //A horizontal plane at y = 0 used for determining the position of the mouse cursor in world space.

    [SerializeField] private Slider _staminaSlider; // The Stamina Meter which is also a slider
    [SerializeField] private Image _staminaFill; // The fill component of the slider
    private float _stamina = 1f; // Initialize to full stamina
    private float _staminaDepletionRate = 0.2f; // Amount to decrease stamina per second while running
    private float _staminaRegenerationRate = 0.2f; // Amount to increase stamina per second while not running
    private Color _lowStaminaColor = new Color(245f / 255f, 35f / 255f, 0f); // Color when stamina is .3 or below
    private Color _highStaminaColor = new Color(0f, 245f / 255f, 57f / 255f); // Color when stamina is above .3

    [SerializeField] private int _currentPlayerHealth;
    [SerializeField] private int _maxPlayerHealth = 100;



    void Start()
    {
        _currentPlayerHealth = _maxPlayerHealth;


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

        bool isRunning = _canSprint && Input.GetKey(KeyCode.LeftShift) && _stamina > 0;

        _targetSpeed = isRunning ? _defaultSpeed * _runMultiplier : _defaultSpeed;
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime / _accelerationTime);

        if (isRunning)
        {
            _stamina = Mathf.Max(0, _stamina - _staminaDepletionRate * Time.deltaTime);
            if (_stamina <= 0)
            {
                _canSprint = false;
                StartCoroutine(SprintCooldown());
            }
        }
        else
        {
            _stamina = Mathf.Min(1, _stamina + _staminaRegenerationRate * Time.deltaTime);
        }

        _staminaSlider.value = _stamina;
        _staminaFill.color = _stamina <= 0.3f ? _lowStaminaColor : _highStaminaColor;
        _staminaSlider.gameObject.SetActive(_stamina < 1);

        if (!_controller.isGrounded)
        {
            direction.y = _fallSpeed;
        }

        _controller.Move(direction * (_currentSpeed * Time.deltaTime));

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -500, 500);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -500, 500);
        transform.position = clampedPosition;
    }

    IEnumerator SprintCooldown()
    {
        yield return new WaitForSeconds(5);
        _canSprint = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "BasicZombieHands")
        {
            TakeDamage(25);
        }
    }

    private void TakeDamage(int damage)
    {
        _currentPlayerHealth -= damage;

        if(_currentPlayerHealth <= 25)
        {
            UIManager.Instance.InjuredBloodyScreen(true);
        }
        else if(_currentPlayerHealth <= 0)
        {
            //Play death animation
            //Transition Player
        }
    }
}