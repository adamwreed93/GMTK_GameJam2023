using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Includes the "Awake" method.
    #region Singleton
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager is null!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public Transform playerObject; //Usable as a reference to the Player object.

    [SerializeField] private TextMeshProUGUI _currentDayText;
    [SerializeField] private TextMeshProUGUI _countdownTimerText;
    [SerializeField] private TextMeshProUGUI _waveEnemiesRemainingText;
    [SerializeField] private Animator _nightTextAnimator;
    [SerializeField] private Animator _fadeInAnimator;
    [SerializeField] private Animator _injuredBloodyScreenAnimator;
    [SerializeField] private Slider _sunTimer; //The Day Timer which is also a slider
    [SerializeField] private GameObject _sunImage;
    [SerializeField] private GameObject _clockBubbles;
    [SerializeField] private GameObject _dayTitleText;
    [SerializeField] private GameObject _moon;
    [SerializeField] private Light _directionalSunLight;
    [SerializeField] private GameObject _injuredBloodyScreen;

    public Slider staminaSlider; // The Stamina Meter which is also a slider
    public Image staminaFill; // The fill component of the slider

    private float _increment; //The value by which _sunTimer increases every second
    private bool _isCountdownStarted = false;
    private float _countdownDuration = 11f; //Duration of the countdown in seconds
    private float _countdownTimer = 0f; //Current countdown timer value
    private int _currentDay = 1;

    public bool isDaytime = true; //Game starts at daytime.


    void Start()
    {
        _increment = 12.0f / (5.0f * 60.0f); //Calculate the increment to reach 12 in 5 minutes
        _fadeInAnimator.SetTrigger("FadeIn");
    }

    void Update()
    {
        RunClock();
    }


    private void RunClock()
    {
        if (isDaytime)
        {
            _sunTimer.value += _increment * Time.deltaTime;

            if (_sunTimer.value <= 3 || _sunTimer.value >= 9.5f)
            {
                _directionalSunLight.color = new Color32(253, 163, 170, 255); //Evening & Dawn
            }
            else
            {
                _directionalSunLight.color = new Color32(255, 218, 179, 255); //Daylight 
            }

            // Switch to night time.
            if (_isCountdownStarted == false && _sunTimer.value >= 11.5f)
            {
                _sunImage.SetActive(false);
                _clockBubbles.SetActive(false);
                _dayTitleText.SetActive(false);
                _countdownTimerText.gameObject.SetActive(true);

                _isCountdownStarted = true;
                StartCoroutine(CountdownTimer());
            }
        }
    }

    private IEnumerator CountdownTimer()
    {
        _countdownTimer = _countdownDuration; //Set initial countdown timer value

        while (_countdownTimer > 1f)
        {
            _countdownTimer -= 1f; //Decrease the countdown timer by 1 second

            // Update the countdown timer text object
            _countdownTimerText.text = _countdownTimer.ToString();

            yield return new WaitForSeconds(1f); //Wait for 1 second before the next iteration
        }

        isDaytime = false;
        _fadeInAnimator.SetTrigger("FadeIn");
        _nightTextAnimator.SetTrigger("FadeInNightText");
        _directionalSunLight.color = new Color32(70, 54, 215, 255); //Night
        _countdownTimerText.gameObject.SetActive(false);
        _moon.SetActive(true);
        _sunTimer.value = 0;
        _waveEnemiesRemainingText.gameObject.SetActive(true);

        GameManager.Instance.SpawnWave();
    }

    public void InjuredBloodyScreen(bool isActive)
    {
        _injuredBloodyScreenAnimator.SetBool("IsInjured", isActive);
    }

    public void BeginNewDay()
    {
        _directionalSunLight.color = new Color32(253, 163, 170, 255); //Evening & Dawn
        _moon.SetActive(false);
        _sunImage.SetActive(true);
        _clockBubbles.SetActive(true);
        _dayTitleText.SetActive(true);
        _currentDay++;
        _currentDayText.text = _currentDay.ToString();
        isDaytime = true;
    }

    public void UpdateWaveEnemiesRemainingText()
    {
        _waveEnemiesRemainingText.text = GameManager.Instance.zombieWaveList.Count.ToString();
    }
}