#if PLAYFAB_INTEGRATION
using PlayFab.MultiplayerModels;

namespace JReact.Playfab_Integration
{
    public static class JPlayfabExtensions
    {
        public static EntityKey ToMultiplayerKey(this PlayFab.ClientModels.EntityKey entityKey)
            => new EntityKey { Id = entityKey.Id, Type = entityKey.Type };
    }
}
#endif
