#if JSPINE_SUPPORT
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JSpineSupport
{
    public sealed class J_DoodleSkinSelector : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;

        //this is the state, show what we have applied
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<J_SO_DoodleSkin> _appliedSkins = new List<J_SO_DoodleSkin>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<J_SO_DoodleSkin, Skin> _resolvedSourceSkins = new Dictionary<J_SO_DoodleSkin, Skin>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private SkeletonDataAsset _cachedSkeletonDataAsset;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _iteration;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Skin _baseSkin;

        private void Awake() { _baseSkin = _doodle.Skeleton.Data.DefaultSkin; }

        // --------------- COMMANDS - ENTRY POINT --------------- //
        public void SetSingleSkin(J_SO_DoodleSkin skinAsset)
        {
            Assert.IsNotNull(skinAsset, $"{gameObject.name} requires a {nameof(skinAsset)}");
            _appliedSkins.Clear();
            AddSkin(skinAsset);
        }

        [Button]
        public void UnEquipAllSkins()
        {
            _appliedSkins.Clear();
            RefreshSkin(_baseSkin);
        }

        [Button]
        public void AddSkin(J_SO_DoodleSkin skinAsset)
        {
            if (_appliedSkins.Contains(skinAsset))
            {
                JLog.Warning($"Skin {skinAsset.name} already applied", JLogTags.Avatar, this);
                return;
            }

            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            Assert.IsTrue(skinAsset.IsCompatible(skeletonAnimation.skeletonDataAsset),
                          $"Skin not compatible skeleton data for skin expected {skinAsset.SkeletonDataName}, got {skeletonAnimation.skeletonDataAsset.name}");

            AddSkinData(skinAsset);
            Skin skin = GetCurrentSkinFromData();
            RefreshSkin(skin);
        }

        [Button]
        public void RemoveSkin(J_SO_DoodleSkin skinAsset)
        {
            if (!_appliedSkins.Contains(skinAsset))
            {
                JLog.Warning($"Skin {skinAsset.name} not applied", JLogTags.Avatar, this);
                return;
            }

            _appliedSkins.Remove(skinAsset);
            Skin newSkin = GetCurrentSkinFromData();
            RefreshSkin(newSkin);
        }

        // --------------- COMMANDS IMPLEMENTATIONS --------------- //
        private void AddSkinData(J_SO_DoodleSkin skinAsset)
        {
            EnsureCacheValid();
            if (!_resolvedSourceSkins.ContainsKey(skinAsset))
            {
                Skeleton skeleton   = _doodle.SpineSkeleton.Skeleton;
                Skin     sourceSkin = skeleton.Data.FindSkin(skinAsset.ThisSkin);

                if (sourceSkin == null)
                {
                    JLog.Warning($"Spine skin not found: {skinAsset.ThisSkin}", JLogTags.Avatar, this);
                    return;
                }

                _resolvedSourceSkins[skinAsset] = sourceSkin;
            }

            _appliedSkins.Add(skinAsset);
        }

        private Skin GetCurrentSkinFromData()
        {
            var skin = new Skin($"{GetEntityId()}_Combined_Skin_{name}__{_iteration++}");
            for (int i = 0; i < _appliedSkins.Count; i++) { skin.AddSkin(_resolvedSourceSkins[_appliedSkins[i]]); }

            return skin;
        }

        private void RefreshSkin(Skin skin)
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            Skeleton          skeleton          = skeletonAnimation.skeleton;

            skeleton.SetSkin(skin);
            skeleton.SetupPoseSlots();
            skeletonAnimation.AnimationState.Apply(skeleton);
            skeletonAnimation.Update(0f);
        }

        // --------------- SAFECHECKS --------------- //
        private void EnsureCacheValid()
        {
            var skeletonAnimation = _doodle.SpineSkeleton;
            var currentAsset      = skeletonAnimation.skeletonDataAsset;

            if (_cachedSkeletonDataAsset == currentAsset) { return; }

            _cachedSkeletonDataAsset = currentAsset;
            _resolvedSourceSkins.Clear();
        }

        [Button]
        public void SeeAllSkins()
        {
            var skeletonData = _doodle.Skeleton.Data;
            foreach (Skin skin in skeletonData.Skins) { JLog.Log(skin.Name, JLogTags.Avatar, this); }
        }
    }
}
#endif
