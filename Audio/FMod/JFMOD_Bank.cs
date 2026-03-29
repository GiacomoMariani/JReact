#if FJMOD_HELPER
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    public enum JBankState { Invalid, Unloaded, Loading, Loaded, LoadedWithSample }

    public sealed class JFMOD_Bank : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _path;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _loadWithSampleData = true;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Bank Bank { get; private set; }

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasBankHandle => Bank.isValid();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public JBankState BankState { get; private set; }

        private void Awake() { BankState = JBankState.Unloaded; }

        public async UniTask LoadBank()
        {
            JLog.Log($"Loading bank {_path}", JLogTags.Audio, this);
            if (Bank.isValid())
            {
                JLog.Warning($"Bank already loaded or loading. Skipping", JLogTags.Audio, this);
                return;
            }

            if (BankState == JBankState.Loading)
            {
                if (_loadWithSampleData) { await WaitForSampleData(); }
                else { await WaitForBankLoading(); }

                return;
            }

            RESULT result = RuntimeManager.StudioSystem.loadBankFile(_path, LOAD_BANK_FLAGS.NONBLOCKING, out Bank loadedBank);

            if (result != RESULT.OK)
            {
                JLog.Error($"FMOD loadBankFile failed: {result} ({_path})", JLogTags.Audio, this);
                BankState = JBankState.Invalid;
                return;
            }

            BankState = JBankState.Loading;
            Bank      = loadedBank;

            bool bankLoadedSuccessfully = await WaitForBankLoading();
            if (!bankLoadedSuccessfully)
            {
                BankState = JBankState.Invalid;
                return;
            }

            BankState = JBankState.Loaded;
            if (!_loadWithSampleData)
            {
                JLog.Log($"Bank {_path} loaded without samples", JLogTags.Audio, this);
                return;
            }

            JLog.Log($"Bank {_path} loading samples", JLogTags.Audio, this);

            bool sampleDataLoadedSuccessfully = await WaitForSampleData();
            if (!sampleDataLoadedSuccessfully)
            {
                BankState = JBankState.Invalid;
                return;
            }

            BankState = JBankState.LoadedWithSample;
            JLog.Log($"Bank {_path} loaded with samples", JLogTags.Audio, this);
        }

        public async UniTask<bool> WaitForBankLoading()
        {
            RESULT        result;
            LOADING_STATE loadingState;
            do
            {
                result = Bank.getLoadingState(out loadingState);
                if (result != RESULT.OK)
                {
                    JLog.Error($"FMOD getLoadingState failed: {result}", JLogTags.Audio, this);
                    return false;
                }

                await UniTask.Yield();
            }
            while (loadingState == LOADING_STATE.LOADING);

            if (loadingState == LOADING_STATE.LOADED) { return true; }

            JLog.Error($"Bank failed to load. State: {loadingState}", JLogTags.Audio, this);
            return false;
        }

        public async UniTask<bool> WaitForSampleData()
        {
            RESULT result = Bank.loadSampleData();
            if (result != RESULT.OK)
            {
                JLog.Error($"FMOD loadSampleData failed: {result}", JLogTags.Audio, this);
                return false;
            }

            LOADING_STATE loadingState;
            do
            {
                result = Bank.getSampleLoadingState(out loadingState);
                if (result != RESULT.OK)
                {
                    JLog.Error($"FMOD getSampleLoadingState failed: {result}", JLogTags.Audio, this);
                    return false;
                }

                await UniTask.Yield();
            }
            while (loadingState == LOADING_STATE.LOADING);

            if (loadingState == LOADING_STATE.LOADED) { return true; }

            JLog.Error($"Sample data failed to load. State: {loadingState}", JLogTags.Audio, this);
            return false;
        }

        public void UnloadBank()
        {
            if (!Bank.isValid()) { return; }

            JLog.Log($"Unloading bank {_path}", JLogTags.Audio, this);
            RESULT result = Bank.unloadSampleData();
            if (result != RESULT.OK) { JLog.Warning($"FMOD unloadSampleData returned: {result}", JLogTags.Audio, this); }

            result = Bank.unload();
            if (result != RESULT.OK) { JLog.Warning($"FMOD unload returned: {result}", JLogTags.Audio, this); }

            Bank.clearHandle();
            Bank      = default;
            BankState = JBankState.Unloaded;
            JLog.Log($"Bank {_path} unloaded", JLogTags.Audio, this);
        }
    }
}
#endif
