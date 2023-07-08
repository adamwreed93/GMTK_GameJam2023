using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentDayText;
    [SerializeField] private TextMeshProUGUI _countdownTimerText;
    [SerializeField] private Animator _nightTextAnimator;
    [SerializeField] private bool _isDaytime = true; //Game starts at daytime.
    [SerializeField] private Slider _sunTimer; //The Day Timer which is also a slider
    [SerializeField] private GameObject _sunImage;
    [SerializeField] private GameObject _clockBubbles;
    [SerializeField] private GameObject _dayTitleText;
    [SerializeField] private GameObject _moon;
    [SerializeField] private Light _directionalSunLight;

    private float _increment; //The value by which _sunTimer increases every second
    private bool _isCountdownStarted = false;
    private float _countdownDuration = 11f; //Duration of the countdown in seconds
    private float _countdownTimer = 0f; //Current countdown timer value


    void Start()
    {
        _increment = 12.0f / (5.0f * 60.0f); //Calculate the increment to reach 12 in 5 minutes
    }

    void Update()
    {
        RunClock();
    }


    private void RunClock()
    {
        if (_isDaytime)
        {
            _sunTimer.value += _increment * Time.deltaTime;

            if (_sunTimer.value >= 9.5f)
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

        //Play "Night Approaches..." text animation here! (AR)

        _isDaytime = false;
        _nightTextAnimator.SetTrigger("FadeInNightText");
        _directionalSunLight.color = new Color32(70, 54, 215, 255); //Night
        _countdownTimerText.gameObject.SetActive(false);
        _moon.SetActive(true);
        _sunTimer.value = 0;
    }
}