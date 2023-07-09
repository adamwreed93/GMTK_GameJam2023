using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntuitionBubble : MonoBehaviour
{
    [SerializeField] private BasicZombie _basicZombie;

    private bool _isLooping;
    private bool _isTargetInRange;
    private int _countdownTime;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Survivor" || other.transform.tag == "Scientist")
        {
            _isTargetInRange = true;

            if (_basicZombie != null)
            {
                _basicZombie.SetEnemyState(BasicZombie.state.alert, other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Survivor" || other.transform.tag == "Scientist")
        {
            if (_isLooping)
            {
                _countdownTime = 0;
            }
            else
            {
                StartCoroutine(IsTargetInRange(other));
            }
        }
    }

    private IEnumerator IsTargetInRange(Collider other)
    {
        _countdownTime = 0;

        while(_isTargetInRange)
        {
            if(_countdownTime >= 5)
            {
                _countdownTime = 0;
                _isLooping = false;
                _isTargetInRange = false;

                if (_basicZombie != null)
                {
                    _basicZombie.SetEnemyState(BasicZombie.state.idle, other);
                }

                break;
            }

            _countdownTime++;
            yield return new WaitForSeconds(1);
        }
    }
}
