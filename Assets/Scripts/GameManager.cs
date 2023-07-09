using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Includes the "Awake" method.
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public List<BasicZombie> zombieList = new List<BasicZombie>();

    public void AddZombieToHostilesList(BasicZombie zombie)
    {
        zombieList.Add(zombie);
    }

    public void RemoveZombieFromHostilesList(BasicZombie zombie)
    {
        zombieList.Remove(zombie);
    }
}
