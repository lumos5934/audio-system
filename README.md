# Audio System

audio mixer 를 기반으로 audio source 를 풀링하여 채널별로 재생, 정지를 하고, <br>
볼륨 관리를 할 수 있습니다.

* [ Usage ](#usage)
* [ API ](#api)

<br>
<br>
<br>


## 🔧Usage
Audio Data를 생성해 AudioManager 에 등록하고 AudioManager 를 통해 전체적인 오디오의 그룹별 출력을 관리합니다.<br>

<br>

### Audio Data 생성
Asset / Create / AudioData <br>
<img width="521" height="254" alt="image" src="https://github.com/user-attachments/assets/93618996-a0c5-48c8-b37c-3bee3f28b276" /> <br>

<br>
<br>

### Audio Manager 생성
GameObject / Aduio / AudioManager<br> 
<img width="439" height="247" alt="image" src="https://github.com/user-attachments/assets/08d8d979-8bf5-42a1-b4d2-d3d6517f459e" /> <br>

<br>
<br>

### Audio Data 설정
<img width="288" height="308" alt="image" src="https://github.com/user-attachments/assets/e386b0db-2758-46a4-a329-20ee77f0df81" /> <br>

<br>
<br>

### Audio Data 할당
원하는 그룹에 Audio Data를 할당 하면 되고, 그룹을 통틀어서 중복된 id 가 있다면 무시 될 수 있습니다. Audio Mixer 에 있는 Audio Mixer Group 을 자동으로 탐색하여 해당 그룹 만큼 인스펙터에 표시됩니다. <br>
<img width="286" height="191" alt="image" src="https://github.com/user-attachments/assets/dd1265ca-9872-4d72-abc6-cc6251e1a43a" />
<img width="287" height="166" alt="image" src="https://github.com/user-attachments/assets/c6d3f755-e63d-48f9-9a41-c768ec13f864" /> 
<img width="288" height="92" alt="image" src="https://github.com/user-attachments/assets/ecdef582-04b4-4905-a255-3151a0aac109" /><br>



* #### 동적 할당
```cs
public AudioData data;

AduioManager.Instance.Register("BGM", data);

//

public AudioMixerGroup mixerGroup;

AudioManager.Instance.Register(mixerGroup, data);

```

<br>
<br>

### Audio 관리
```cs

// 개별 관리
var handle = AduioManager.Instance.Play("fire");
handle.Stop();
handle.Pause();
handle.UnPause();


// 그룹별 관리
AduioManager.Instance.Stop("BGM");
AduioManager.Instance.Pause("BGM");
AduioManager.Instance.UnPause("BGM");

```

<br>
<br>

### 볼륨 관리
```cs

//normalized = 0~1
AduioManager.Instance.SetVolume("BGM", 0.4);
AduioManager.Instance.GetVolume("BGM");

```


<br>
<br>
<br>


## 📖API

* ### Audio Manager
**`Register(groupName, data)`** : Audio Mixer 에 등록된 MixerGorup의 이름과 일치하다면 해당 데이터를 등록합니다. <br>
**`Register(group, data)`** : 해당 MixerGroup 으로 데이터를 할당합니다.<br>
**`Play(id, fadeIn)`** : id를 통해 오디오를 재생합니다. fadeIn 에 따라 fade 효과를 부여 할 수 있습니다.<br>
**`PlayAt(id, pos, fadeInDuration)`** : 특정 위치에 오디오를 재생합니다.<br>
**`Stop(groupName, fade)`** : 해당 그룹의 모든 핸들을 정지 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다. fadeTime 에 따라 fade 효과를 부여 할 수 있습니다. <br>
**`Pause(groupName)`** : 해당 그룹의 모든 핸들을 일시 정지 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다.<br>
**`UnPause(groupName)`** : 해당 그룹의 모든 핸들의 일시 정지를 해제 합니다. 그룹 이름이 "" 일 경우 모든 그룹에 반영합니다.<br>
**`FadeTo(handle, target, duration, omComplete)`** : 특정 핸들의 볼륨에 fade 효과를 부여 합니다.<br>
**`SetVolume(string groupName, linear)`** : Audio Mixer 의 볼륨을 조정합니다.<br>
**`GetVolume(string groupName)`** : Audio Mixer 의 볼륨을 얻습니다.<br>

<br>

 * ### Audio Handle
**`IsValid`** : 현재 재생, 일시정지중이라면 true 를 반환합니다. <br>
**`IsPlaying`** : 현재 재생중 이라면 true 를 반환합니다.<br>
**`IsPaused`** : 현재 일시정지중 이라면 true 를 반환합니다.<br>
**`Stop(fadeTime)`** : 해당 핸들을 정지 합니다. fadeTime 에 따라 fade out 하게 됩니다.<br>
**`Pause()`** : 해당 핸들을 일시 정지 합니다.<br>
**`UnPause()`** : 해당 핸들의 일시 정지를 해제 합니다.<br>
**`Fade(targetVolume, duration)`** : 해당 핸들의 duration 동안 targetVolume 으로 fade 합니다.<br>

<br>

* ### Audio Data
**`Id`** : 재생에 사용되는 키 입니다. <br>
**`Loop`** : 해당 오디오의 반복 여부를 나타냅니다.<br>
**`Priority`** : 오디오의 우선순위를 나타냅니다. <br>
**`Audio Clip Data[]`** : 재생될 클립, Volume, Pitch를 설정 할 수 있습니다. 데이터가 여러개 일 경우 랜덤한 클립을 반환합니다. <br>
**`Source Prefab`** : 실제 사용될 Audio Source, 프리팹 별로 풀링됩니다. <br>
