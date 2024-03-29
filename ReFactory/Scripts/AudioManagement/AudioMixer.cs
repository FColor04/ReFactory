﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace AudioManagement;

public class AudioMixer : IDisposable
{
    public AudioMixerGroup Group;

    public float MixerVolume;
    public int AudioTrackLimit;

    private float _fadeVolumeTarget;
    private float _fadeVolumeMultiplier = 1f;
    private float _fadeDuration = 2f;

    private SoundEffectInstance _fadeInInstance;
    private readonly List<SoundEffectInstance> _instances = new();

    public AudioMixer(AudioMixerGroup group, float mixerVolume = 1f, int audioTrackLimit = -1)
    {
        MixerVolume = mixerVolume;
        Group = group;
        AudioTrackLimit = audioTrackLimit;
    }

    public void Dispose()
    {
        _fadeInInstance?.Dispose();
        var activeInstances = new Queue<SoundEffectInstance>(_instances);
        while (activeInstances.TryDequeue(out var instance))
            instance.Dispose();
    }

    public void FadeTo(SoundEffect audioTrack, float targetVolume = 1f, float pitch = 0f, float pan = 0f, float duration = 2f, bool loop = true)
    {
        _fadeInInstance = audioTrack.CreateInstance();
        _fadeInInstance.Volume = 0;
        _fadeInInstance.Pitch = pitch;
        _fadeInInstance.Pan = pan;
        _fadeInInstance.IsLooped = loop;
        _fadeInInstance.Play();

        _fadeVolumeMultiplier = 1f;
        _fadeVolumeTarget = targetVolume;
        _fadeDuration = duration;
    }

    public void Play(SoundEffect audioTrack, float volume = 1f, float pitch = 0, float pan = 0f, bool loop = true)
    {
        if (AudioTrackLimit != -1 && _instances.Count >= AudioTrackLimit) return;

        var trackInstance = audioTrack.CreateInstance();
        trackInstance.Volume = MixerVolume * volume * _fadeVolumeMultiplier;
        trackInstance.Pitch = pitch;
        trackInstance.Pan = pan;
        trackInstance.IsLooped = loop;
        trackInstance.Play();
        _instances.Add(trackInstance);
    }

    /// <summary>
    /// Manually update the mixer fade status
    /// </summary>
    /// <param name="deltaTime">Time between this and previous frame</param>
    public void Update(float deltaTime)
    {
        if (_fadeInInstance == null) return;

        if (_fadeVolumeMultiplier > 0f)
            _fadeVolumeMultiplier -= deltaTime / (Math.Max(_fadeDuration, 0.0001f));
        else
        {
            _fadeInInstance.Volume = _fadeVolumeTarget * MixerVolume;
            _instances.Add(_fadeInInstance);
            _fadeInInstance = null;

            return;
        }

        _fadeInInstance.Volume = _fadeVolumeTarget * MixerVolume * (1 - _fadeVolumeMultiplier);
    }
}