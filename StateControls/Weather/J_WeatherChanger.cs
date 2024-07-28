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
    public sealed class J_WeatherChanger : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_WeatherType[] _allWeathers;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_WeatherStates _weatherStateControl;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _allWeatherWeights;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;

        // --------------- COMMANDS --------------- //
        public void ActivateThis()
        {
            Assert.IsNotNull(_weatherStateControl, $"{name} requires a _weatherStateControl");
            CalculateWeights();
            _weatherStateControl.Activate();
            WaitWeather(_weatherStateControl.CurrentState);
        }

        private void CalculateWeights()
        {
            _allWeatherWeights = 0;
            for (int i = 0; i < _allWeathers.Length; i++) _allWeatherWeights += _allWeathers[i].Weight;
        }

        public void EndThis()
        {
            Timing.KillCoroutines(_coroutine);
            _weatherStateControl.CurrentState.End();
            _weatherStateControl.ResetThis();
        }

        // --------------- WEATHER CHANGES --------------- //
        private void WaitWeather(J_WeatherType weather)
        {
            _coroutine = Timing.RunCoroutine(WaitBeforeChange(weather), Segment.SlowUpdate);
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
