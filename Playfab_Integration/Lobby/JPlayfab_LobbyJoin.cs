#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Lobby
{
    public class JPlayfab_LobbyJoin : J_PlayfabRequest<JoinLobbyRequest, JoinLobbyResult>
    {
        private readonly EntityKey _entity;
        private readonly string _connectionId;
        private readonly string _playerType;

        public static async UniTask<JoinLobbyResult> JoinFirstLobby(EntityKey entity, string playerType, List<string> equals = default)
        {
            J_PlayfabResult<FindLobbiesResult> searchResult =
                await new JPlayfab_LobbySearch(equals, LobbyMemberType.NotMember).Process();

            List<LobbySummary> lobbiesFound = searchResult.Result.Lobbies;
            if (lobbiesFound       == default ||
                lobbiesFound.Count == 0)
            {
                JLog.Warning($"No lobbies found to join.", JLogTags.Playfab);
                return default;
            }

            string                           connectionId       = lobbiesFound[0].ConnectionString;
            var                              joinFirstFreeLobby = new JPlayfab_LobbyJoin(entity, connectionId, playerType);
            J_PlayfabResult<JoinLobbyResult> result             = await joinFirstFreeLobby.Process();
            return result.Result;
        }

        public JPlayfab_LobbyJoin(EntityKey entity, string connectionId, string playerType)
        {
            _entity       = entity;
            _connectionId = connectionId;
            _playerType   = playerType;
        }

        protected override void ResetRequest(JoinLobbyRequest request) {}

        protected override JoinLobbyRequest UpdateRequest(JoinLobbyRequest request)
        {
            request.MemberEntity     = _entity;
            request.MemberData       = new Dictionary<string, string>() { { "Type", _playerType } };
            request.ConnectionString = _connectionId;
            return request;
        }

        protected override void SendRequest(JoinLobbyRequest     request, Action<JoinLobbyResult> successCallback,
                                            Action<PlayFabError> errorCallback)
        {
            PlayFabMultiplayerAPI.JoinLobby(request, successCallback, errorCallback);
        }
    }
}
#endif
