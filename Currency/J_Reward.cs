﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Currency
{
    /// <summary>
    /// the basic reward giver
    /// </summary>
    public abstract class J_Reward<T> : ScriptableObject, iObservable<J_Reward<T>>
    {
        private event JGenericDelegate<J_Reward<T>> OnRewardFail;
        private event JGenericDelegate<J_Reward<T>> OnRewardGranted;

        /// <summary>
        /// sends a reward
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool GrantReward(int amount, Transform sender = null)
        {
            var rewardSuccess = TrySendReward(amount, sender);
            
            if (!rewardSuccess) OnRewardFail?.Invoke(this);
            else OnRewardGranted?.Invoke(this);

            return rewardSuccess;
        }

        protected abstract bool TrySendReward(int amount, Transform sender);

        #region SUBSCRIBERS
        public void SubscribeToWindChange(JGenericDelegate<J_Reward<T>> actionToAdd) { OnRewardGranted += actionToAdd; }
        public void UnSubscribeToWindChange(JGenericDelegate<J_Reward<T>> actionToRemove) { OnRewardGranted -= actionToRemove; }

        public void SubscribeToFail(JGenericDelegate<J_Reward<T>> actionToAdd) { OnRewardFail += actionToAdd; }
        public void UnSubscribeToFail(JGenericDelegate<J_Reward<T>> actionToRemove) { OnRewardFail -= actionToRemove; }
        #endregion
    }
}
