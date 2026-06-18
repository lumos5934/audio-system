using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


namespace LLib
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private List<AudioSetup> _setupList = new();

        private Dictionary<string, AudioGroup> _groupByName = new();
        private Dictionary<string, (AudioGroup, AudioData)> _groupDataById = new();
        private AudioHandlePool _handlePool;

       
        [Serializable]
        private struct AudioSetup
        {
            public string groupName;
            public List<AudioData> audioDataList;
        }
        
        
        private void Awake()    
        {
            if (_mixer == null)
            {
                enabled = false;
                return;
            }
            
            if (Instance != null)
            {
                Destroy(gameObject); 
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            
            _handlePool = new AudioHandlePool(transform);

            var mixerGroups = _mixer.FindMatchingGroups("");
            foreach (var mixerGroup in mixerGroups)
            {
                _groupByName[mixerGroup.name] = new AudioGroup()
                {
                    MixerGroup = mixerGroup,
                };
            }

            foreach (var setup in _setupList)
            {
                foreach (var audioData in setup.audioDataList)
                {
                    Register(setup.groupName,  audioData);
                }
            }
        }


        public void Register(string groupName, AudioData data)
        {
            if (_groupByName.TryGetValue(groupName, out var group))
            {
                if (!_groupDataById.TryAdd(data.id, (group, data)))
                {
                    Debug.LogWarning($"already registered: {data.id}");
                }
            }
            else
            {
                Debug.LogError($"not found Group :'{groupName}' , Data :'{data.id}'");
            }
        }


        public void Register(AudioMixerGroup group, AudioData data)
        {
            Register(group.name, data);
        }


        public AudioHandle Play(string id, float fadeIn = 0f)
        {
            return PlayAt(id, Vector3.zero, fadeIn);
        }

        
        public AudioHandle PlayAt(string id, Vector3 pos, float fadeDuration = 0f)
        {
            if(!_groupDataById.TryGetValue(id, out var value))
                return AudioHandle.Invalid;
            
            AudioGroup group = value.Item1;
            AudioData data = value.Item2;
            
            var handle = _handlePool.Get(group, data);
            if(handle == null)
                return AudioHandle.Invalid;
            
            var audioSource = handle.AudioSource;
            audioSource.transform.position = pos;
            audioSource.Play();
            
            if (fadeDuration > 0f)
            {
                audioSource.volume = 0f;
                FadeTo(handle, handle.BaseVolume, fadeDuration);
            }

            if (!data.loop)
            {
                StartCoroutine(AutoReleaseRoutine(handle));
            }

            group.ActiveHandles.Add(handle);
            
            return handle;
        }


        public void Stop(string groupName = "", float fade = 0f)
        {
            ForeachGroup(groupName, handle => handle.Stop(fade));
        }

        
        public void Pause(string groupName = "")
        {
            ForeachGroup(groupName, handle => handle.Pause());
        }

        
        public void UnPause(string groupName = "")
        {
            ForeachGroup(groupName, handle => handle.UnPause());
        }


        private void ForeachGroup(string groupName, Action<AudioHandle> handleAction)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                foreach (var nameGroupPair in _groupByName)
                {
                    foreach (var handle in nameGroupPair.Value.ActiveHandles.ToList())
                    {
                        handleAction?.Invoke(handle);
                    }
                }

                return;
            }
            
            
            if (_groupByName.TryGetValue(groupName, out var group))
            {
                foreach (var handle in group.ActiveHandles.ToList())
                {
                    handleAction?.Invoke(handle);
                }
            }
        }


        public void FadeTo(AudioHandle handle, float target, float duration, Action onComplete = null)
        {
            StartCoroutine(FadeRoutine(handle, target, duration, onComplete));
        }

        
        public void SetVolume(string groupName, float normalizedVolume)
        {
            if (_groupByName.TryGetValue(groupName, out var group))
            {
                float value = normalizedVolume > 0.0001f ? Mathf.Log10(normalizedVolume) * 20f : -80f;
                group.MixerGroup.audioMixer.SetFloat(groupName, value);
            }
        }

        
        public float GetVolume(string groupName)
        {
            if (_groupByName.TryGetValue(groupName, out var group))
            {
                group.MixerGroup.audioMixer.GetFloat(groupName, out var dB);
                return Mathf.Pow(10, dB / 20f);
            }

            return 0f;
        }
        
        
        private IEnumerator FadeRoutine(AudioHandle handle, float targetVolume, float duration, Action onComplete)
        {
            float start = handle.Volume;
            for (float t = 0; t < duration && handle.IsValid; t += Time.unscaledDeltaTime)
            {
                handle.Volume = Mathf.Lerp(start, targetVolume, t / duration);
                yield return null;
            }

            if (handle.IsValid)
            {
                handle.Volume = targetVolume;
            }
            
            onComplete?.Invoke();
        }
        

        private IEnumerator AutoReleaseRoutine(AudioHandle h)
        {
            while (h.IsValid)
            {
                yield return null;
            }
        
            _handlePool.Release(h);
        }
    }
}
