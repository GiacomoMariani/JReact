#if PLAYFAB_INTEGRATION
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace JReact.Playfab_Integration
{
    public abstract class JAbs_PlayfabData<T> : IJPlayfabData
    {
        public string Key { get; }
        public UserDataRecord LastRecord { get; private set; }
        public JAbs_PlayfabData(string key) { Key = key; }

        // --------------- LOAD --------------- //
        void IJPlayfabData.ReceiveData(UserDataRecord value)
        {
            LastRecord = value;
            HandleDataReceived(value);
        }

        /// <summary>
        /// Processes the data received from the PlayFab service.
        /// This method is invoked when user data is retrieved and updates the last recorded data.
        /// Override this method to implement custom logic for handling the received data.
        /// </summary>
        /// <param name="record">The user data record received from the PlayFab service.</param>
        protected virtual void HandleDataReceived(UserDataRecord record) {}

        void IJPlayfabData.EmptyDataRetrieved() { HandleEmptyDataRetrieved(); }

        /// <summary>
        /// happens if the player has no saved data, usually at the first log in
        /// </summary>
        protected virtual void HandleEmptyDataRetrieved() {}

        void IJPlayfabData.LoadError(PlayFabError error) { HandleLoadError(error); }

        /// <summary>
        /// Handles errors that occur during the data loading process from the PlayFab service.
        /// This method allows for custom handling of PlayFab errors encountered during data retrieval.
        /// </summary>
        /// <param name="error">The error details received from the PlayFab service.</param>
        protected virtual void HandleLoadError(PlayFabError error) {}

        // --------------- SAVE --------------- //
        /// <summary>
        /// Retrieves the current data associated with the PlayFab implementation.
        /// The method is responsible for returning the specific data type used in the implementation.
        /// Override this method to define the logic for fetching and returning the data.
        /// </summary>
        /// <returns>The data of type T associated with the current instance.</returns>
        protected abstract T GetData();

        string IJPlayfabData.ConvertToString()
        {
            T data = GetData();
            return DataToString(data);
        }

        // --------------- MAIN QUEUES --------------- //
        public async UniTask SaveData(bool forced = true)
        {
            J_PlayfabDataSetter.Add(this);
            if (forced) await J_PlayfabDataSetter.SaveAllStatic();
        }

        public async UniTask LoadData(bool forced = true)
        {
            J_PlayfabDataGetter.Add(this);
            if (forced) await J_PlayfabDataGetter.LoadAllStatic();
        }

        // --------------- BASE CONVERTERS --------------- //
        protected virtual string DataToString(T      item) { return JsonUtility.ToJson(item); }
        protected virtual T      StringToData(string json) { return JsonUtility.FromJson<T>(json); }
    }

    public interface IJPlayfabData
    {
        string Key { get; }
        internal string ConvertToString();
        internal void   ReceiveData(UserDataRecord value);
        internal void   EmptyDataRetrieved();
        internal void   LoadError(PlayFabError error);
    }
}
#endif
