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
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        _currentWaveNumber++;
        _numberOfZombiesToSpawn *= _currentWaveNumber;

        for (int i = 0; i < _numberOfZombiesToSpawn; i++)
        {
            GameObject zombie = Instantiate(_zombiePrefab, _enemyContainer.transform);
            AddZombieToWaveList(zombie);

            // move zombie to a random position between -150 and 150 X, 200 and -250 Z. Y stays the same.
            float randomX = Random.Range(-150, 150);
            float randomZ = Random.Range(-250, 200);
            float currentY = zombie.transform.position.y;
            zombie.transform.position = new Vector3(randomX, currentY, randomZ);

            yield return new WaitForSeconds(1);
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
