#if PLAYFAB_INTEGRATION
using System;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Matchmaking
{
    public class JGetPlayfabMatch : J_PlayfabRequest<GetMatchRequest, GetMatchResult>
    {
        public string MatchId { get; private set; }
        public string QueueName { get; private set; }

        public JGetPlayfabMatch SetMatchId(string matchId)
        {
            MatchId = matchId;
            return this;
        }

        public JGetPlayfabMatch SetQueue(string queueName)
        {
            QueueName = queueName;
            return this;
        }

        protected override void ResetRequest(GetMatchRequest request)
        {
            request.MatchId   = string.Empty;
            request.QueueName = string.Empty;
        }

        protected override GetMatchRequest UpdateRequest(GetMatchRequest request)
        {
            request.MatchId   = MatchId;
            request.QueueName = QueueName;
            return request;
        }

        protected override void SendRequest(GetMatchRequest      request, Action<GetMatchResult> successCallback,
                                            Action<PlayFabError> errorCallback)
        {
            PlayFabMultiplayerAPI.GetMatch(request, successCallback, errorCallback);
        }
    }
}
#endif
