using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : MonoBehaviour
{
    public enum state
    {
        idle, //Meander during the Daytime
        alert, //When the Player or an NPC enters their "Intuition Bubble" look for them.
        combat, //When the Player or an NPC enters their "Line of Sight" attack
        aggressive //At night the enemy waves will be aggressive and be drawn to the player.
    }

    [SerializeField] private state _enemyState;

    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _leftHand;
    [SerializeField] private Collider _rightHand;
    [SerializeField] private Transform _target; // Target to track and attack
    [SerializeField] private float _attackRange = 4f;
    [SerializeField] private float _nextAttackTime;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;

    private bool _idleCanMove = true;
    private bool _alertCanMove = true;

    [SerializeField] private Transform _idleTargetPoint;

    [SerializeField] private int _enemyHealth;


    private void Update()
    {
        if(!UIManager.Instance.isDaytime && _enemyState != state.alert && _enemyState != state.combat)
        {
            _enemyState = state.aggressive;
        }

        switch (_enemyState)
        {
            case state.idle:
                StartCoroutine(IdleStateMovement());
                return;
            case state.alert:
                StartCoroutine(AlertStateMovement());
                return;
            case state.combat:
                CombatState();
                return;
            case state.aggressive:
                AggressiveState();
                return;
        }

        if(_enemyState == state.combat)
        {
            //Activate colliders
            _leftHand.enabled = true;
            _rightHand.enabled = true;
        }
        else
        {
            //Deactivate colliders
            _leftHand.enabled = false;
            _rightHand.enabled = false;
        }
    }


    /// <summary>
    /// Enemy is in this state during the day if not engaged in combat. Enemy will meander.
    /// </summary>
    private IEnumerator IdleStateMovement()
    {
        if (_idleCanMove)
        {
            _idleCanMove = false;

            // Create a new point and position it
            Random.InitState(System.DateTime.Now.Millisecond);
            _idleTargetPoint.position = new Vector3(Random.Range(3, 7) + _idleTargetPoint.position.x, transform.position.y, Random.Range(3, 7) + transform.position.z);

            _target = _idleTargetPoint;

            yield return new WaitForSeconds(Random.Range(3, 7));
            _idleCanMove = true;
        }

        Vector3 direction = (_target.position - transform.position).normalized; //Calculate direction to move.
        RotateTowardsTarget(); //Rotate towards target.

        if (Vector3.Distance(transform.position, _target.position) > .5)
        {
            transform.Translate(direction * (_movementSpeed * .25f) * Time.deltaTime, Space.World); //Move this Zombie.
        }
    }



    /// <summary>
    /// Enemy is in this state when the Player or an NPC enters their "Intuition Bubble". Enemy will look for them.
    /// </summary>
    private IEnumerator AlertStateMovement()
    {
        if (_alertCanMove)
        {
            _alertCanMove = false;

            Random.InitState(System.DateTime.Now.Millisecond);
            int timeTillChoice = Random.Range(3, 5);
            yield return new WaitForSeconds(timeTillChoice);
            int chanceToTurn = Random.Range(1, 10);

            if (chanceToTurn >= 4)
            {
                RotateTowardsTarget(); //Rotate towards target.
            }

            _alertCanMove = true;
        }

        Vector3 direction = (_target.position - transform.position).normalized; //Calculate direction to move.
        transform.Translate(direction * _movementSpeed * Time.deltaTime, Space.World); //Move this Zombie.
    }


    /// <summary>
    /// Enemy is in this state when the Player or an NPC enters their "Line of Sight". Enemies will attack in this state.
    /// </summary>
    private void CombatState() 
    {
        Vector3 direction = (_target.position - transform.position).normalized; //Calculate direction to move.
        RotateTowardsTarget(); //Rotate towards target.
        float distance = Vector3.Distance(transform.position, _target.position); //Calculate distance between this Zombie and its Target.

        // If the target is out of attack range, move towards it
        if (distance > _attackRange)
        {
            transform.Translate(direction * _movementSpeed *Time.deltaTime, Space.World); //Move this Zombie.
        }
        // If the target is within attack range, attack it
        else
        {
            if (Time.time > _nextAttackTime) //Attack Cooldown.
            {
                _animator.SetTrigger("BasicAttack");
                _nextAttackTime = Time.time + 2f; // Adds 2-second delay between attacks.
            }
        }
    }


    /// <summary>
    /// //At night the enemy waves will be "Aggressive" making them drawn to the player.
    /// </summary>
    private void AggressiveState()
    {
        _target = UIManager.Instance.playerObject; //Finds and targets the player.

        Vector3 direction = (_target.position - transform.position).normalized; //Calculate direction to move.

        RaycastHit hit;

        // Cast a ray forward from the GameObject. 
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                RaycastHit hitLeft;
                RaycastHit hitRight;

                // Cast rays to the left and right.
                bool isHitLeft = Physics.Raycast(transform.position, -transform.right, out hitLeft, 5f);
                bool isHitRight = Physics.Raycast(transform.position, transform.right, out hitRight, 5f);

                //If there is no obstacle to the left, move left.
                if (!isHitLeft)
                {
                    direction = -transform.right;
                }
                //If there is no obstacle to the right, move right.
                else if (!isHitRight)
                {
                    direction = transform.right;
                }
                else
                {
                    //If there's an obstacle in both directions, move in the direction of the least resistance.
                    //Compare the distances to the obstacles to the left and right and move towards the farthest one.
                    direction = (hitLeft.distance < hitRight.distance) ? transform.right : -transform.right;
                }
            }
        }

        RotateTowardsTarget(); //Rotate towards target.
        transform.Translate(direction * _movementSpeed * Time.deltaTime, Space.World); //Move this Zombie.
    }


    public void SetEnemyState(state newState, Collider target)
    {
        _enemyState = newState;
        _target = target.transform;
    }


    private void RotateTowardsTarget()
    {
        // Calculate rotation towards target.
        Vector3 direction = (_target.position - transform.position).normalized;
        direction.y = 0; // Forces the direction vector's Y value to be 0.

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Lerp rotation at a speed of rotationSpeed.
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Bullet")
        {
            Destroy(other.gameObject);  
            TakeDamage(1);
        }
    }
    private void TakeDamage(int damage)
    {
        _enemyHealth -= damage;

        if (_enemyHealth <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}