using System;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySound : StateMachineBehaviour
{

    public Condition condition;

    public AudioClip sound;
    public AudioMixerGroup audioMixerGroup;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(0f,1f)]
    public float normalizedPlayTime;

    private bool restart = true;
    private AudioSource audioSource;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (this.audioSource == null)
        {
            this.audioSource = animator.GetComponent<AudioSource>();
            if (this.audioSource == null)
            {
                this.audioSource = animator.gameObject.AddComponent<AudioSource>();
                this.audioSource.outputAudioMixerGroup = audioMixerGroup;
            }

        }

        if (condition == Condition.OnEnter)
            Play(sound, volume);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (condition != Condition.OnUpdate)
            return;

        var x = stateInfo.normalizedTime - Math.Truncate(stateInfo.normalizedTime);
        if (x < normalizedPlayTime) {
            restart = true;
        }

        if (restart && x > normalizedPlayTime)
        {
           Play(sound, volume);
           restart = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (condition == Condition.OnExit)
            Play(sound, volume);
    }

    private void Play(AudioClip clip, float volume)
    {
        if (clip == null){
            return;
        }

        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);
        }
    }

    public enum Condition { 
        OnEnter,
        OnUpdate,
        OnExit

    }
}
