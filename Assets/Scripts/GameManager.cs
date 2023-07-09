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


    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private int _numberOfZombiesToSpawn = 10;
    private int _currentWaveNumber = 0;

    public bool isZombified;

    public List<BasicZombie> zombieList = new List<BasicZombie>();

    public List<GameObject> zombieWaveList = new List<GameObject>();

    public void AddZombieToHostilesList(BasicZombie zombie)
    {
        zombieList.Add(zombie);
    }

    public void RemoveZombieFromHostilesList(BasicZombie zombie)
    {
        zombieList.Remove(zombie);
    }

    public void SpawnWave()
    {
        _currentWaveNumber++;
        _numberOfZombiesToSpawn *= _currentWaveNumber;

        for (int i = 0; i < _numberOfZombiesToSpawn; i++)
        {
            GameObject zombie = Instantiate(_zombiePrefab, _enemyContainer.transform);
            AddZombieToWaveList(zombie);
        }
        UIManager.Instance.UpdateWaveEnemiesRemainingText();
    }

    public void AddZombieToWaveList(GameObject zombie)
    {
        zombieWaveList.Add(zombie);
    }

    public void RemoveZombieFromWaveList(GameObject zombie)
    {
        zombieWaveList.Remove(zombie);
        UIManager.Instance.UpdateWaveEnemiesRemainingText();
    }

    public void CheckIfWaveIsOver()
    {
        if (zombieWaveList.Count == 0 && UIManager.Instance.isDaytime == false)
        {
            UIManager.Instance.BeginNewDay();
        }
    }
}
