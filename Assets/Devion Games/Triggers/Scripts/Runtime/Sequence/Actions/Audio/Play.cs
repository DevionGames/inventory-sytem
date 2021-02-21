using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Audio/Play")]
    public class Play : Action
    {
        [SerializeField]
        private AudioClip m_Clip = null;
        [SerializeField]
        private AudioMixerGroup m_AudioMixerGroup = null;
        [SerializeField]
        private float m_Volume = 0.4f;

        public override ActionStatus OnUpdate()
        {
            UnityTools.PlaySound(this.m_Clip, this.m_Volume,this.m_AudioMixerGroup);
            return ActionStatus.Success;
        }
    }
}
