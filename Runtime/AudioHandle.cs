using LLib;
using UnityEngine;

public class AudioHandle
{
    public static readonly AudioHandle Invalid = new();

    public bool  IsValid    => AudioSource != null && AudioSource.isPlaying || _isPaused;
    public bool  IsPlaying  => AudioSource != null && AudioSource.isPlaying;
    public bool  IsPaused   => _isPaused;
    public float Volume
    {
        get => AudioSource?.volume ?? 0;
        set
        {
            if (AudioSource)
            {
                AudioSource.volume = value;
            }
        }
    }
    
    internal AudioSource AudioSource;
    internal AudioData Data;
    internal float BaseVolume;
    
    private bool _isPaused;
    private AudioGroup _group;
    private AudioHandlePool _pool;
    

    AudioHandle() { }
    internal AudioHandle(AudioHandlePool pool)
    {
        _pool = pool;
    }
    
    
    public void Stop(float fadeTime = 0f)
    {
        if (AudioSource == null) return;
        if (fadeTime > 0f)
        {
            AudioManager.Instance.FadeTo(this, 0f, fadeTime, () => _pool.Release(this));
        }
        else
        {
            _pool.Release(this);
        }
    }

    
    public void Pause()
    {
        if (AudioSource == null || _isPaused) 
            return;
        
        AudioSource.Pause();
        _isPaused = true;
    }

    
    public void UnPause()
    {
        if (AudioSource == null || !_isPaused) 
            return;
        
        AudioSource.UnPause();
        _isPaused = false;
    }


    public void Fade(float targetVolume, float duration)
    {
        AudioManager.Instance.FadeTo(this, targetVolume, duration);
    }
    
    
    internal void Bind(AudioGroup group, AudioSource source, AudioData data)
    {
        Data = data;
        _group = group;
        
        AudioSource = source;
        AudioSource.playOnAwake = false;
        AudioSource.outputAudioMixerGroup = group.MixerGroup;
        var clipData = Data.GetClipData();
        AudioSource.clip = clipData.Clip;
        AudioSource.volume = clipData.Volume;
        AudioSource.pitch = clipData.Pitch;
        AudioSource.priority = Data.priority;
        AudioSource.loop = Data.loop;
        
        BaseVolume = source.volume;
        
        _isPaused = false;
    }


    internal void Unbind()
    {
        if (AudioSource == null) 
            return;

        _group.ActiveHandles.Remove(this);
        AudioSource.Stop();
        AudioSource.loop = false;
        AudioSource   = null;
        Data     = null;
        _isPaused = false;
    }
}