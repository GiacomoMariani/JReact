using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.Weather
{
    /// <summary>
    /// controls the weather
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Weather/Weather Changer", fileName = "WeatherChanger")]
    public sealed class J_WeatherChanger : J_Service
    {
        // --------------- CONSTANTS --------------- //
        private const string COROUTINE_WeatherMainTag = "COROUTINE_WeatherMainTag";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_WeatherType[] _allWeathers;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_WeatherStates _weatherStateControl;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _allWeatherWeights;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;

        // --------------- COMMANDS --------------- //
        protected override void ActivateThis()
        {
            base.ActivateThis();
            Assert.IsNotNull(_weatherStateControl, $"{name} requires a _weatherStateControl");
            _instanceId = GetInstanceID();

            CalculateWeights();

            _weatherStateControl.Activate();
            WaitWeather(_weatherStateControl.CurrentState);
        }

        private void CalculateWeights()
        {
            _allWeatherWeights = 0;
            for (int i = 0; i < _allWeathers.Length; i++) _allWeatherWeights += _allWeathers[i].Weight;
        }

        protected override void EndThis()
        {
            Timing.KillCoroutines(_instanceId, COROUTINE_WeatherMainTag);
            _weatherStateControl.CurrentState.End();
            _weatherStateControl.ResetThis();
            base.EndThis();
        }

        // --------------- WEATHER CHANGES --------------- //
        private void WaitWeather(J_WeatherType weather)
        {
            Timing.RunCoroutine(WaitBeforeChange(weather), Segment.SlowUpdate, _instanceId, COROUTINE_WeatherMainTag);
        }

        private IEnumerator<float> WaitBeforeChange(J_WeatherType weather)
        {
            yield return Timing.WaitForSeconds(weather.CalculateNextInterval());
            CheckNextWeather();
        }

        //used to calculate the next state
        private void CheckNextWeather()
        {
            //if there's no weather we do nothing
            if (_allWeathers        == null ||
                _allWeathers.Length == 0)
            {
                JLog.Warning($"{name} has not available weathers, we cannot change.");
                return;
            }

            //setup a counter and the weather
            int nextWeightedIndex = UnityEngine.Random.Range(0, _allWeatherWeights);
            //counts to get the next state
            for (int i = 0; i < _allWeathers.Length; i++)
            {
                //counter => remove the weight
                nextWeightedIndex -= _allWeathers[i].Weight;
                //if counter less tha 0 we can set the new weather
                if (nextWeightedIndex <= 0)
                {
                    SetNextWeather(_allWeathers[i]);
                    return;
                }
            }

            JLog.Error($"{name} found no weather");
        }

        private void SetNextWeather(J_WeatherType nextWeather)
        {
            _weatherStateControl.SetNewState(nextWeather);
            WaitWeather(nextWeather);
        }
    }
}
