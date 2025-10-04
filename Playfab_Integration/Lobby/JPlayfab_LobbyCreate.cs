#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Lobby
{
    public class JPlayfab_LobbyCreate : J_PlayfabRequest<CreateLobbyRequest, CreateLobbyResult>
    {
        public readonly string playerType;
        public readonly EntityKey ownerKey;
        public readonly uint maxPlayers;
        public readonly AccessPolicy accessPolicy;
        public readonly OwnerMigrationPolicy ownerMigrationPolicy;
        public readonly bool useConnections;
        private readonly Dictionary<string, string> searchData;

        public JPlayfab_LobbyCreate(EntityKey            owner, string playerType, List<string> data, uint maxPlayers = 10,
                                    AccessPolicy         accessPolicy         = AccessPolicy.Public,
                                    //if we set an owner migration we need use connections as true, and we need at least one player connected with PubSub to allow connection
                                    OwnerMigrationPolicy ownerMigrationPolicy = OwnerMigrationPolicy.None,
                                    bool                 useConnections       = false)
        {
            ownerKey        = owner;
            this.playerType = playerType;
            searchData      = new Dictionary<string, string>();

            for (int i = 0; i < data.Count; i++) { searchData[$"string_key{i + 1}"] = data[i]; }

            this.maxPlayers           = maxPlayers;
            this.accessPolicy         = accessPolicy;
            this.ownerMigrationPolicy = ownerMigrationPolicy;
            this.useConnections       = useConnections;
            if(useConnections) {JLog.Warning($"We need at least one player connected with PubSub to allow connection using {nameof(SubscribeToLobbyResourceRequest)}", JLogTags.Playfab);}
        }

        protected override void ResetRequest(CreateLobbyRequest request) { request.Members?.Clear(); }

        protected override CreateLobbyRequest UpdateRequest(CreateLobbyRequest request)
        {
            Member self = GenerateSelf();
            request.Members              = new List<Member>() { self };
            request.Owner                = self.MemberEntity;
            request.MaxPlayers           = maxPlayers;
            request.AccessPolicy         = accessPolicy;
            request.OwnerMigrationPolicy = ownerMigrationPolicy;
            request.SearchData           = searchData;
            request.UseConnections       = useConnections;
            return request;
        }

        protected override void SendRequest(CreateLobbyRequest   request, Action<CreateLobbyResult> successCallback,
                                            Action<PlayFabError> errorCallback)
        {
            PlayFabMultiplayerAPI.CreateLobby(request, successCallback, errorCallback);
        }

        private Member GenerateSelf()
        {
            var self = new Member();
            self.MemberData         = new Dictionary<string, string>();
            self.MemberData["Type"] = playerType;
            self.MemberEntity       = ownerKey;
            return self;
        }
    }
}
#endif
