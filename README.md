# Audio System

audio mixer 를 기반으로 오디오를 관리합니다. audio source를 풀링하여 Mixer Group 별로 재생, 정지를 하고, 볼륨 관리를 할 수 있습니다. 

* [ Usage ](#usage)
* [ API ](#api)

<br>
<br>
<br>


## 🔧Usage

<br>

#### Audio Data 생성
`Asset / Create / AudioData` <br>
<img width="521" height="254" alt="image" src="https://github.com/user-attachments/assets/93618996-a0c5-48c8-b37c-3bee3f28b276" /> <br>

<br>
<br>

#### Audio Manager 생성
`GameObject / Aduio / AudioManager`<br> 
<img width="439" height="247" alt="image" src="https://github.com/user-attachments/assets/08d8d979-8bf5-42a1-b4d2-d3d6517f459e" /> <br>

<br>
<br>

#### Audio Data 설정
<img width="320" height="312" alt="image" src="https://github.com/user-attachments/assets/4f456754-19ad-46d3-8fe8-45babba3d199" /> <br>

<br>
<br>

#### Audio 관리
```cs

// 개별 관리
var handle = AduioManager.Instance.Play(AudioData);
handle.Stop();
handle.Pause();
handle.UnPause();


// 그룹별 관리
AduioManager.Instance.Stop(MixerGroup);
AduioManager.Instance.Pause(MixerGroup);
AduioManager.Instance.UnPause(MixerGroup);

```

<br>
<br>


#### 볼륨 관리
```cs

//normalized = 0~1
AduioManager.Instance.SetVolume(MixerGroup, 0.4);
AduioManager.Instance.GetVolume(MixerGroup);

```

<br>
<br>


#### Audio Player
컴포넌트 형태로 사용 가능한 Audio Player 를 제공합니다. 사용방법은 동일하며 `Is Source Root` 체크를 통해 오디오 소스를 자식으로 지정 할 수 있습니다. <br>

`GameObject / Aduio / AudioManager`<br> 
<img width="484" height="244" alt="image" src="https://github.com/user-attachments/assets/beb7d7ae-f738-44ad-9a14-941d468106cd" />
<img width="322" height="209" alt="image" src="https://github.com/user-attachments/assets/27b6da69-4e3e-43ac-9f5b-6a559b50e324" />



<br>
<br>
<br>


## 📖API

#### Audio Manager
**`Play(data, fadeIn)`** : id를 통해 오디오를 재생합니다. fadeIn 에 따라 fade 효과를 부여 할 수 있습니다.<br>
**`Play(data, pos, fadeInDuration)`** : 특정 위치에 오디오를 재생합니다.<br>
**`Play(data, root, fadeInDuration)`** : 특정 부모 자식으로 오디오를 재생합니다.<br>
**`Stop(mixerGroup, fade)`** : 해당 그룹의 모든 핸들을 정지 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다. fadeTime 에 따라 fade 효과를 부여 할 수 있습니다. <br>
**`Pause(mixerGroup)`** : 해당 그룹의 모든 핸들을 일시 정지 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다.<br>
**`UnPause(mixerGroup)`** : 해당 그룹의 모든 핸들의 일시 정지를 해제 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다.<br>
**`FadeTo(handle, target, duration, omComplete)`** : 특정 핸들의 볼륨에 fade 효과를 부여 합니다.<br>
**`SetVolume(mixerGroup, normalizedVolume)`** : Audio Mixer Group 의 볼륨을 0~1로 조정합니다.<br>
**`GetVolume(mixerGroup)`** : Audio Mixer Group 의 볼륨을 얻습니다.<br>

<br>

#### Audio Handle
**`IsValid`** : 현재 재생, 일시정지중이라면 true 를 반환합니다. <br>
**`IsPlaying`** : 현재 재생중 이라면 true 를 반환합니다.<br>
**`IsPaused`** : 현재 일시정지중 이라면 true 를 반환합니다.<br>
**`Stop(fadeTime)`** : 해당 핸들을 정지 합니다. fadeTime 에 따라 fade out 하게 됩니다.<br>
**`Pause()`** : 해당 핸들을 일시 정지 합니다.<br>
**`UnPause()`** : 해당 핸들의 일시 정지를 해제 합니다.<br>
**`Fade(targetVolume, duration)`** : 해당 핸들의 duration 동안 targetVolume 으로 fade 합니다.<br>

<br>

#### Audio Data
**`Loop`** : 해당 오디오의 반복 여부를 나타냅니다.<br>
**`MixerGroup`** : 사용 할 Audio Mixer Group 입니다. <br>
**`Audio Clip`** : 재생될 클립 입니다. <br>
**`Volume`** : 오디오의 볼륨을 0~1로 설정합니다. <br>
**`Pitch`** : 오디오의 피치를 0~1로 설정합니다. <br>
**`Priority`** : 오디오의 우선순위를 설정합니다. <br>
**`Source Prefab`** : 실제 사용될 Audio Source, 프리팹 별로 풀링됩니다. <br>


<br>

#### Audio Player
**`Data`** : 재생 할 Audio Data 입니다. <br>
**`FadeInDuration`** : Play시 fade 효과의 지속시간입니다. <br>
**`FadeOutDuration`** : Stop시 fade 효과의 지속시간입니다. <br>
**`PlayOnEnable`** : OnEnable시 재생하는지의 여부입니다. <br>
**`FollowTreansform`** : 해당 AudioSource가 Player를 따라다니는지의 여부입니다. <br>
**`Play()`** : 오디오를 재생합니다. <br>
**`Stop()`** : 오디오를 정지합니다. <br>
**`Pause()`** : 오디오를 일시정지합니다. <br>
**`UnPause()`** : 오디오의 일시정지를 해제합니다. <br>
**`SetVolume()`** : 오디오의 볼륨을 조절합니다. <br>
