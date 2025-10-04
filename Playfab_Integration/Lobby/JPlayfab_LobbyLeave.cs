#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Lobby
{
    public class JPlayfab_LobbyLeave : J_PlayfabRequest<LeaveLobbyRequest, LobbyEmptyResult>
    {
    private readonly EntityKey _leaver;
    private readonly string _lobbyId;

    public static async UniTask<List<LobbySummary>> LeaveAllLobby(EntityKey self)
    {
        J_PlayfabResult<FindLobbiesResult> ownLobbies = await new JPlayfab_LobbySearch(default, LobbyMemberType.Member).Process();
        List<LobbySummary>                 lobbyFound = ownLobbies.Result.Lobbies;
        for (int i = 0; i < lobbyFound.Count; i++)
        {
            var lobby  = lobbyFound[i];
            var result = await new JPlayfab_LobbyLeave(self, lobby.LobbyId).Process();

            JLog.Log($"Leaving Lobby {lobby.LobbyId}. Success: {result.IsSuccessfull}", JLogTags.Playfab);
        }

        return lobbyFound;
    }

    public JPlayfab_LobbyLeave(EntityKey leaver, string lobbyId)
    {
        _leaver  = leaver;
        _lobbyId = lobbyId;
    }

    protected override void ResetRequest(LeaveLobbyRequest request) {}

    protected override LeaveLobbyRequest UpdateRequest(LeaveLobbyRequest request)
    {
        request.MemberEntity = _leaver;
        request.LobbyId      = _lobbyId;
        return request;
    }

    protected override void SendRequest(LeaveLobbyRequest    request, Action<LobbyEmptyResult> successCallback,
                                        Action<PlayFabError> errorCallback)
    {
        PlayFabMultiplayerAPI.LeaveLobby(request, successCallback, errorCallback);
    }
    }
}
#endif
