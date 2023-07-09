using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private BasicZombie _basicZombie;
    [SerializeField] private LayerMask _obstacleMask; // Mask to determine what objects count as obstacles

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Survivor" || other.transform.tag == "Scientist")
        {
            if (_basicZombie != null)
            {
                // Perform the raycast
                RaycastHit hit;
                Vector3 direction = other.transform.position - _basicZombie.transform.position;
                if (Physics.Raycast(_basicZombie.transform.position, direction, out hit, direction.magnitude, _obstacleMask))
                {
                    // If the first thing hit is not the object that entered the trigger, then there is something in between
                    if (hit.transform != other.transform)
                    {
                        return;
                    }
                }

                // If we got here, either the raycast did not hit anything or the first thing hit was the object that entered the trigger
                _basicZombie.SetEnemyState(BasicZombie.state.combat, other);
            }
        }
    }
}
