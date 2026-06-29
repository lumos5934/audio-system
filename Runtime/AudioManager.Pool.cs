using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LLib
{
    public partial class AudioManager
    {
        internal class Pool
        {
            private readonly HashSet<AudioHandle> _active = new();
            
            private readonly Dictionary<AudioSource, IObjectPool<AudioSource>> _sourcePools = new();
            private readonly IObjectPool<AudioHandle> _handlePool;
            
            public Pool()
            {
                _handlePool = new ObjectPool<AudioHandle>(
                    createFunc: () => new AudioHandle(),
                    actionOnGet: handle => {},
                    actionOnRelease: handle => handle.Unbind(),
                    actionOnDestroy: null
                );
            }

            public AudioHandle Get(AudioData data)
            {
                var source = GetOrCreateSource(data.SourcePrefab);
                
                var handle = _handlePool.Get();
                handle.Bind(source, data);

                _active.Add(handle);
                
                return handle;
            }

            public void Release(AudioHandle handle)
            {
                if (_active.Contains(handle))
                {
                    _active.Remove(handle);

                    if (_sourcePools.TryGetValue(handle.Data.SourcePrefab, out var pool))
                    {
                        pool.Release(handle.AudioSource);
                    }

                    _handlePool.Release(handle);
                }
            }
            
            private AudioSource GetOrCreateSource(AudioSource prefab)
            {
                if (!_sourcePools.TryGetValue(prefab, out var pool))
                {
                    pool = new ObjectPool<AudioSource>(
                        createFunc: () => Instantiate(prefab),
                        actionOnGet: source => source.gameObject.SetActive(true),
                        actionOnRelease: source =>
                        {
                            source.transform.SetParent(null);
                            source.gameObject.SetActive(false);
                        },
                        actionOnDestroy: source => Destroy(source.gameObject)
                    );
                    
                    _sourcePools[prefab] = pool;
                }
              
                return pool.Get();
            }
        }
    }
}


