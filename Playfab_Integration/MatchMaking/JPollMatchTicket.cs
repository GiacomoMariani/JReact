#if PLAYFAB_INTEGRATION
using System;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Matchmaking
{
    public class JPollMatchTicket : J_PlayfabRequest<GetMatchmakingTicketRequest, GetMatchmakingTicketResult>
    {
        public string TicketId { get; private set; }
        public string QueueName { get; private set; }

        public JPollMatchTicket SetTicketId(string ticketId)
        {
            TicketId = ticketId;
            return this;
        }

        public JPollMatchTicket SetQueue(string queueName)
        {
            QueueName = queueName;
            return this;
        }

        protected override void ResetRequest(GetMatchmakingTicketRequest request)
        {
            request.TicketId  = string.Empty;
            request.QueueName = string.Empty;
        }

        protected override GetMatchmakingTicketRequest UpdateRequest(GetMatchmakingTicketRequest request)
        {
            request.TicketId  = TicketId;
            request.QueueName = QueueName;
            return request;
        }

        protected override void SendRequest(GetMatchmakingTicketRequest request, Action<GetMatchmakingTicketResult> successCallback,
                                            Action<PlayFabError>        errorCallback)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket(request, successCallback, errorCallback);
        }
    }
}
#endif
