using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField] private Animator _animator;

    [SerializeField] private float _gravity = -9.81f;

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

    //Zombification
    [SerializeField] private GameObject _zombifiedGrannyPrefab;
    [SerializeField] private GameObject _livingGrannyGFX;
    [SerializeField] private GameObject _zombifiedGrannyGFX;
    [SerializeField] private GameObject _livingGrannyCamera;
    [SerializeField] private GameObject _zombifiedGrannyCamera;

    [SerializeField] private Collider _leftHand;
    [SerializeField] private Collider _rightHand;

    private float _xRotation = 0f;
    public float _mouseSensitivity = 100f;
    public float _zombieSpeed = 2f;

    private GameManager _gameManager;
    private UIManager _uiManager;

    public bool isZombified;


    void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _currentPlayerHealth = _maxPlayerHealth;
        _staminaFill = _uiManager.staminaFill;
        _staminaSlider = _uiManager.staminaSlider;

        _defaultSpeed = _speed;
        _targetSpeed = _speed;
        _currentSpeed = _speed;

        _plane = new Plane(Vector3.up, Vector3.zero);

        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if(!isZombified)
        {
            RotateLivingGFXToMouse();
            LivingMovement();
        }
        else
        {
            RotateZombifiedGFXToMouse();
            ZombifiedMovement();

            if(Input.GetButtonDown("Fire1"))
            {
                ZombieAttack();
            }
        }

        
        Gravity();
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

    private void LivingMovement()
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

    private void ZombifiedMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.TransformDirection(new Vector3(horizontalInput, 0, verticalInput));

        if (!_controller.isGrounded)
        {
            moveDirection.y = _fallSpeed;
        }

        _controller.Move(moveDirection * (_zombieSpeed * Time.deltaTime));

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


    void RotateLivingGFXToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - _livingGrannyGFX.transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            _livingGrannyGFX.transform.rotation = rotation;
        }
    }

    void RotateZombifiedGFXToMouse()
    {
        // Make the mouse cursor invisible and keep it centered
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Rotate camera based on mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _xRotation -= mouseY * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _zombifiedGrannyCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX * _mouseSensitivity);
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

        if (_currentPlayerHealth <= 0)
        {
            if(!isZombified)
            {
                ZombifyGranny();
                return;
            }
            //Game Over!
        }
        else if (_currentPlayerHealth <= 25 && _currentPlayerHealth > 0)
        {
            _uiManager.InjuredBloodyScreen(true);
        }
    }

    private void ZombifyGranny()
    {
        StartCoroutine(ZombieStartUpProtocol());

        _livingGrannyGFX.SetActive(false);
        _zombifiedGrannyGFX.SetActive(true);
        isZombified = true;
        _gameManager.isZombified = isZombified;
        _currentPlayerHealth = 100;
    }

    private IEnumerator ZombieStartUpProtocol()
    {
        

        // Get the start position for the "living" camera
        Vector3 cameraStartPosition = _livingGrannyCamera.transform.position;

        // Calculate target position for the "living" camera
        Vector3 cameraEndPosition = _zombifiedGrannyCamera.transform.position;

        // Duration for camera to move from start to end position
        float duration = 2.0f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Compute how far along the duration we are, between 0 and 1
            float t = elapsed / duration;

            // Move the "living" camera towards the zombified camera's position
            _livingGrannyCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // At the end of the movement, set the "living" camera exactly at the target position
        _livingGrannyCamera.transform.position = cameraEndPosition;

        // Switch cameras
        _livingGrannyCamera.SetActive(false);
        _zombifiedGrannyCamera.SetActive(true);

        // Change the tag of the object this script is attached to
        this.gameObject.tag = "ZombiePlayer";

        // Create a new list to store the zombies to be removed
        List<BasicZombie> zombiesToRemove = new List<BasicZombie>();

        foreach (BasicZombie zombie in _gameManager.zombieList)
        {
            zombie.ResetThisZombie();
            zombiesToRemove.Add(zombie); // Add zombie to be removed later
        }

        // Now remove all the zombies that need to be removed
        foreach (BasicZombie zombie in zombiesToRemove)
        {
            _gameManager.zombieList.Remove(zombie);
        }

        _uiManager.InjuredBloodyScreen(false);
    }

    private IEnumerator ZombieAttack()
    {
        _leftHand.enabled = true;
        _rightHand.enabled = true;
        _animator.SetTrigger("ZombieGrannyAttack");
        yield return new WaitForSeconds(.5f);
        _leftHand.enabled = false;
        _rightHand.enabled = false;
    }
}