using System.Collections.Generic;
using JReact.JuiceMenuComposer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jreact.UiViewMono.UiAnimations
{
    public class JUI_AnimationControls : JUI_Item
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private JUI_AnimationControls _waitExternalDelay;
        [BoxGroup("Setup", true, true, 0), SerializeField] private JUI_AnimationControls _waitExternalPlay;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_SO_AnimationTime _delay;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_AnimationTime _playTime;

        public void SetDelayTime(J_SO_AnimationTime delay)    { _delay    = delay; }
        public void SetPlayTime(J_SO_AnimationTime  playTime) { _playTime = playTime; }

        // --------------- JUI CONTROLS --------------- //
        public override void OnInit(JUI_Screen parentScreen) {}

        public override IEnumerator<float> OnBeforeShow(JUI_Screen parentScreen) { yield break; }

        public override void OnStopShowing(JUI_Screen parentScreen) {}

        public override void OnCompleteShow(JUI_Screen parentScreen) {}

        public override IEnumerator<float> OnBeforeHide(JUI_Screen parentScreen) { yield break; }

        public override void OnStopHiding(JUI_Screen parentScreen) {}

        public override void OnCompleteHide(JUI_Screen parentScreen) {}
    }
}
