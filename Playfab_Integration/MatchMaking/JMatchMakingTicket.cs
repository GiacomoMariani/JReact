#if PLAYFAB_INTEGRATION
using System;
using PlayFab;
using PlayFab.MultiplayerModels;
using EntityKey = PlayFab.ClientModels.EntityKey;

namespace JReact.Playfab_Integration.Matchmaking
{
    public class JMatchMakingTicket : J_PlayfabRequest<CreateMatchmakingTicketRequest, CreateMatchmakingTicketResult>
    {
        // --------------- READONLY --------------- //
        public readonly EntityKey playerEntity;
        public readonly int giveUpAfterSeconds;

        public string QueueName { get; private set; }

        public JMatchMakingTicket(EntityKey playerEntity, int giveUpAfterSeconds)
        {
            this.playerEntity       = playerEntity;
            this.giveUpAfterSeconds = giveUpAfterSeconds;
        }

        public JMatchMakingTicket SetQueue(string queueName)
        {
            QueueName = queueName;
            return this;
        }

        protected override void ResetRequest(CreateMatchmakingTicketRequest request)
        {
            request.Creator            = null;
            request.GiveUpAfterSeconds = 0;
            request.QueueName          = string.Empty;
        }

        protected override CreateMatchmakingTicketRequest UpdateRequest(CreateMatchmakingTicketRequest request)
        {
            request.Creator = new MatchmakingPlayer
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type, },
                //sample we can change
                Attributes = new MatchmakingPlayerAttributes { DataObject = new { Skill = 24.4 }, },
            };

            request.GiveUpAfterSeconds = giveUpAfterSeconds;
            request.QueueName          = QueueName;
            return request;
        }

        protected override void SendRequest(CreateMatchmakingTicketRequest        request,
                                            Action<CreateMatchmakingTicketResult> successCallback, Action<PlayFabError> errorCallback)
        {
            PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, successCallback, errorCallback);
        }
    }
}
#endif
