#if PLAYFAB_INTEGRATION
using System;
using Cysharp.Threading.Tasks;
using JReact.TimeProgress;
using PlayFab.MultiplayerModels;
using Sirenix.OdinInspector;
using EntityKey = PlayFab.ClientModels.EntityKey;
using Object = UnityEngine.Object;

namespace JReact.Playfab_Integration.Matchmaking
{
    public class JMain_PlayfabMatchMaking
    {
        // --------------- CONST --------------- //
        public const string MatcheFound = "Matched";

        // --------------- FIELDS AND PROPERTIES --------------- //
        protected JMatchMakingTicket _createtTicketRequest;
        protected JPollMatchTicket _pollTicketRequest;
        protected JGetPlayfabMatch _getMatchRequest;

        protected readonly int _pollTimingMs = 6500;
        protected readonly int _giveUpAfterSeconds = 10;
        protected readonly string _invalidResult = string.Empty;

        public float StartTime { get; private set; }
        public float TimeLeft => _giveUpAfterSeconds - (JTime.UnscaledTime - StartTime);
        public bool IsSearching { get; private set; }
        public bool IsCanceled { get; private set; }
        public bool CanContinue => TimeLeft > 0 && !IsCanceled;

        // --------------- CONSTRUCTOR --------------- //
        public JMain_PlayfabMatchMaking(EntityKey entityKey, int giveUpAfterSeconds, int pollTimingMs = 6500)
        {
            _giveUpAfterSeconds = giveUpAfterSeconds;
            _pollTimingMs       = pollTimingMs;
            StartTime           = JTime.UnscaledTime;

            _createtTicketRequest = new JMatchMakingTicket(entityKey, giveUpAfterSeconds);
            _pollTicketRequest    = new JPollMatchTicket();
            _getMatchRequest      = new JGetPlayfabMatch();
        }

        [Button]
        public virtual async UniTask<string> FindMatch(string queueName, Object context = default)
        {
            IsSearching = true;
            IsCanceled  = false;
            try
            {
                // --------------- TICKET --------------- //
                _createtTicketRequest.SetQueue(queueName);
                J_PlayfabResult<CreateMatchmakingTicketResult> createTicketResult = await _createtTicketRequest.Process();

                if (createTicketResult.Error != null) { return _invalidResult; }

                JLog.Log($"Success - MatchMaking Ticket Created: {createTicketResult.Result.TicketId}", JLogTags.Playfab, context);

                // --------------- POLL RESULT --------------- //
                _pollTicketRequest.SetTicketId(createTicketResult.Result.TicketId).SetQueue(queueName);

                GetMatchmakingTicketResult pollResult = null;
                while (pollResult == null)
                {
                    if (!CanContinue) { return _invalidResult; }

                    J_PlayfabResult<GetMatchmakingTicketResult> pollTaskResult = await _pollTicketRequest.Process();
                    if (pollTaskResult.Error != null) { return _invalidResult; }

                    JLog.Log($"Polled Match: {pollTaskResult.Result.TicketId}: {pollTaskResult.Result.Status}", JLogTags.Playfab,
                             context);

                    if (pollTaskResult.Result.Status == MatcheFound) { pollResult = pollTaskResult.Result; }
                    else { await UniTask.Delay(_pollTimingMs); }
                }

                JLog.Log($"Poll Ticket Confirmed. Retrieving Match: {pollResult.MatchId}", JLogTags.Playfab);

                // --------------- GET MATCH --------------- //
                if (!CanContinue) { return _invalidResult; }

                _getMatchRequest.SetMatchId(pollResult.MatchId).SetQueue(queueName);

                J_PlayfabResult<GetMatchResult> getMatch = await _getMatchRequest.Process();
                if (getMatch.Error != null) { return _invalidResult; }

                JLog.Log($"Match retrieved: {getMatch.Result.MatchId}", JLogTags.Playfab);
                return getMatch.Result.MatchId;
            }
            catch (Exception e)
            {
                JLog.Error($"{e.Message}\n{e.StackTrace}", JLogTags.Playfab, context);
                return _invalidResult;
            }
            finally { IsSearching = false; }
        }
    }
}
#endif
