#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration.Lobby
{
    public enum LobbyMemberType { NotMember, Owner, Member }

    public class JPlayfab_LobbySearch : J_PlayfabRequest<FindLobbiesRequest, FindLobbiesResult>
    {
        public bool Containsfilter => !string.IsNullOrEmpty(Filter);
        public string Filter { get; private set; }
        public string Sort { get; private set; }

        public JPlayfab_LobbySearch(List<string> equals, LobbyMemberType memberType, string sort = "lobby/memberCount desc")
        {
            if (equals != default)
                for (int i = 0; i < equals.Count; i++) { AddToFilter($"string_key{i + 1} eq '{equals[i]}'"); }

            if (memberType      == LobbyMemberType.Owner) { AddToFilter("lobby/amOwner eq 'true'"); }
            else if (memberType == LobbyMemberType.Member) { AddToFilter("lobby/amMember eq 'true'"); }

            Sort = sort;
        }

        private void AddToFilter(string s)
        {
            if (!Containsfilter) { Filter =  s; }
            else { Filter                 += $" and {s}"; }
        }

        protected override void ResetRequest(FindLobbiesRequest request) {}

        protected override FindLobbiesRequest UpdateRequest(FindLobbiesRequest request)
        {
            request.Filter  = Filter;
            request.OrderBy = Sort;
            return request;
        }

        protected override void SendRequest(FindLobbiesRequest   request, Action<FindLobbiesResult> successCallback,
                                            Action<PlayFabError> errorCallback)
        {
            PlayFabMultiplayerAPI.FindLobbies(request, successCallback, errorCallback);
        }
    }
}
#endif
