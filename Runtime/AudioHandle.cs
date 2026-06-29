using UnityEngine;

namespace LLib
{
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
                if (AudioSource != null)
                {
                    AudioSource.volume = value;
                }
            }
        }
        
        internal AudioSource AudioSource;
        internal AudioData Data;
        internal float BaseVolume;
        
        private bool _isPaused;
        
        internal AudioHandle() { }

        
        public void Stop(float fadeDuration = 0f)
        {
            if (AudioSource == null) 
                return;
            
            if (fadeDuration > 0f)
            {
                AudioManager.Instance.FadeTo(this, 0f, fadeDuration, () => AudioSource.Stop());
            }
            else
            {
                AudioSource.Stop();
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
        
        
        internal void Bind(AudioSource source, AudioData data)
        {
            Data = data;
            
            AudioSource = source;
            AudioSource.playOnAwake = false;
            AudioSource.outputAudioMixerGroup = data.MixerGroup;
            AudioSource.clip = Data.Clip;
            AudioSource.volume = Data.Volume;
            AudioSource.pitch = Data.Pitch;
            AudioSource.priority = Data.Priority;
            AudioSource.loop = Data.Loop;
            
            BaseVolume = source.volume;
            
            _isPaused = false;
        }


        internal void Unbind()
        {
            if (AudioSource == null) 
                return;

            AudioSource.Stop();
            AudioSource.loop = false;
            AudioSource = null;
            Data = null;
            
            _isPaused = false;
        }
    }
}
