using MyUtility;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager> {

    // --- STRUCT (ZERO-GARBAGE) ---
    private struct ActiveSound {
        public AudioSource source;
        public AudioData data;
        public float elapsed;       // Safety buffer for first-frame check
        public bool isUnscaled;     // UI sesi mi?
        public bool isPaused;
    }

    [Header("Settings")]


    [Header("Mixer Groups")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioMixerGroup ambienceGroup;

    // PlayerPrefs & Params
    private const string MusicVolumePrefKey = "MUSIC_VOLUME";
    private const string VolumePrefKey = "MASTER_VOLUME";
    private const string SfxVolumePrefKey = "SFX_VOLUME";
    private const string UiVolumePrefKey = "UI_VOLUME";
    private const string AmbienceVolumePrefKey = "AMBIENCE_VOLUME";
    private const string MasterVolumeParam = "MasterVolume";
    private const string MusicVolumeParam = "MusicVolume";
    private const string SfxVolumeParam = "SFXVolume";
    private const string UiVolumeParam = "UIVolume";
    private const string AmbienceVolumeParam = "AmbienceVolume";

    private ObjectPool<AudioSource> audioPool;

    // Kapasiteyi ba�tan veriyoruz (Allocation �nlemek i�in)
    private List<ActiveSound> activeSounds = new List<ActiveSound>(64);

    private AudioSource musicSource;

    protected override void Awake() {
        base.Awake();
        InitPool(20, 64);
        InitMusicSource();
        LoadVolumeSettings();
    }

    // --- MAIN LOOP ---
    private void Update() {
        for (int i = activeSounds.Count - 1; i >= 0; i--) {
            ActiveSound activeSound = activeSounds[i];

            // 1. Safety check: Source destroyed
            if (activeSound.source == null || !activeSound.source.gameObject.activeSelf) {
                activeSounds.RemoveAt(i);
                continue;
            }

            // 2. Skip looping sounds (manually stopped)
            if (activeSound.source.loop) continue;

            // 3. Skip paused sounds
            if (activeSound.isPaused) continue;

            // 4. Update elapsed time (safety buffer for first-frame)
            float dt = activeSound.isUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            activeSound.elapsed += dt;
            activeSounds[i] = activeSound;

            // 5. Check if sound finished (with safety buffer)
            if (!activeSound.source.isPlaying && activeSound.elapsed > 0.05f) {
                ReturnToPool(activeSound.source);
            }
        }
    }

    private void InitPool(int defaultCapacity, int maxCapacity) {
        audioPool = new ObjectPool<AudioSource>(
            createFunc: () => { 
                var _audioGO = new GameObject("PooledAudioSource");
                var audioSource = _audioGO.AddComponent<AudioSource>();
                return audioSource;
            },
            actionOnGet: source => source.gameObject.SetActive(true),
            actionOnRelease: source => {
                source.Stop();
                source.clip = null;
                source.volume = 1f;
                source.ignoreListenerPause = false;
                source.gameObject.SetActive(false);
                if (source.transform.parent != transform) source.transform.SetParent(transform);
            },
            actionOnDestroy: source => Destroy(source.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxCapacity
        );
    }

    private void InitMusicSource() {
        var musicGO = new GameObject("MusicSource_Main");
        musicGO.transform.SetParent(transform);
        musicSource = musicGO.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
    }

    #region Play Sound Methods

    public AudioSource PlaySound(AudioData data, Vector3 position = default, Transform parent = null) {
        if (data == null) return null;

        AudioSource source = audioPool.Get();

        data.ApplyDataToSource(source, out _);

        source.outputAudioMixerGroup = GetMixerGroupByType(data.audioType);
        source.ignoreListenerPause = (data.audioType == AudioType.UI);

        if (parent != null) {
            source.transform.SetParent(parent);
            source.transform.localPosition = Vector3.zero;
        } else {
            source.transform.position = (position == default) ? transform.position : position;
        }

        source.Play();

        activeSounds.Add(new ActiveSound {
            source = source,
            elapsed = 0f,
            isUnscaled = (data.audioType == AudioType.UI),
            isPaused = false,
            data = data
        });

        return source;
    }

    // Overloads
    public AudioSource PlaySound(AudioData data) => PlaySound(data, default, null);
    public AudioSource PlaySound3D(AudioData data, Vector3 position) => PlaySound(data, position, null);
    public AudioSource PlaySoundAttached(AudioData data, Transform parent) => PlaySound(data, default, parent);

    #endregion

    #region Pause & Resume Logic (YEN�)

    /// <summary>
    /// Belirli bir t�rdeki (�rn: SFX) t�m aktif sesleri duraklat�r.
    /// </summary>
    public void PauseAudioByType(AudioType type) {
        if (type == AudioType.Music) { PauseMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        for (int i = 0; i < activeSounds.Count; i++) {
            // Struct kopyas�n� al
            ActiveSound sound = activeSounds[i];

            // E�er o gruba aitse duraklat
            if (sound.source.outputAudioMixerGroup == targetGroup && !sound.isPaused) {
                sound.source.Pause();
                sound.isPaused = true;

                // Struct'� listeye geri yaz (G�ncelleme)
                activeSounds[i] = sound;
            }
        }
    }

    /// <summary>
    /// Belirli bir t�rdeki duraklat�lm�� sesleri devam ettirir.
    /// </summary>
    public void ResumeAudioByType(AudioType type) {
        if (type == AudioType.Music) { ResumeMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        for (int i = 0; i < activeSounds.Count; i++) {
            ActiveSound sound = activeSounds[i];

            if (sound.source.outputAudioMixerGroup == targetGroup && sound.isPaused) {
                sound.source.UnPause();
                sound.isPaused = false;

                // Struct'� listeye geri yaz
                activeSounds[i] = sound;
            }
        }
    }

    /// <summary>
    /// UI hari� her �eyi duraklat�r (Genelde oyun i�i Pause men�s� i�in)
    /// </summary>
    public void PauseAllGameSounds() {
        PauseAudioByType(AudioType.SFX);
        PauseAudioByType(AudioType.Ambience);
        // M�zik genelde devam eder ama istersen: PauseMusic();
    }

    public void ResumeAllGameSounds() {
        ResumeAudioByType(AudioType.SFX);
        ResumeAudioByType(AudioType.Ambience);
        // ResumeMusic();
    }

    #endregion

    #region Tracking & Release Logic

    public void StopSound(AudioSource source) {
        if (source != null && source.gameObject.activeSelf) {
            ReturnToPool(source);
        }
    }
    public void StopSound(AudioData data) {
        if (data == null) return;
        for (int i = 0; i < activeSounds.Count;) {


            ActiveSound sound = activeSounds[i];

            if (sound.data == data) {
                AudioSource sourceToStop = sound.source;

                int lastIndex = activeSounds.Count - 1;

                if (i < lastIndex) {

                    activeSounds[i] = activeSounds[lastIndex];
                }

                activeSounds.RemoveAt(lastIndex);

                audioPool.Release(sourceToStop);

            } else {
                i++;
            }
        }
    }

    private void ReturnToPool(AudioSource source) {
        // Manuel for d�ng�s� (Lambda allocation'dan ka�mak i�in)
        int index = -1;
        for (int i = 0; i < activeSounds.Count; i++) {
            if (activeSounds[i].source == source) {
                index = i;
                break;
            }
        }

        if (index >= 0) {
            // Swap Removal: Listenin ortas�ndan silmek yerine sonuncuyu buraya kopyalay�p sonuncuyu sil.
            // Bu i�lem CPU dostudur (Kayd�rma yapmaz).
            int lastIndex = activeSounds.Count - 1;
            if (index < lastIndex) {
                activeSounds[index] = activeSounds[lastIndex];
            }
            activeSounds.RemoveAt(lastIndex);
        }

        audioPool.Release(source);
    }

    public void StopAllAudio() {
        StopMusic();

        // Listeyi bo�alt�rken tersten gidip havuza at�yoruz
        while (activeSounds.Count > 0) {
            // Son eleman�n kayna��n� al
            var source = activeSounds[activeSounds.Count - 1].source;
            // Havuza iade et (ReturnToPool metodu listeyi de temizleyecektir)
            ReturnToPool(source);
        }
    }

    public void StopAudioByType(AudioType type) {
        if (type == AudioType.Music) { StopMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        // Tersten d�n�yoruz ��nk� silme i�lemi yapaca��z
        for (int i = activeSounds.Count - 1; i >= 0; i--) {
            if (activeSounds[i].source.outputAudioMixerGroup == targetGroup) {
                ReturnToPool(activeSounds[i].source);
            }
        }
    }

    #endregion

    #region Fade Methods

    public void FadeOutSound(AudioSource source, float duration) {
        if (source == null || !source.gameObject.activeSelf) return;

        LeanTween.value(source.gameObject, source.volume, 0f, duration)
            .setOnUpdate((float val) => source.volume = val)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => StopSound(source));
    }

    public void FadeOutSound(AudioData data, float duration) {
        if (data == null) return;

        for (int i = 0; i < activeSounds.Count; i++) {
            if (activeSounds[i].data == data) {
                FadeOutSound(activeSounds[i].source, duration);
            }
        }
    }

    public void FadeOutMusic(float duration) {
        if (musicSource == null || !musicSource.isPlaying) return;

        LeanTween.value(musicSource.gameObject, musicSource.volume, 0f, duration)
            .setOnUpdate((float val) => musicSource.volume = val)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                StopMusic();
                musicSource.volume = 1f;
            });
    }

    
    #endregion

    #region Music & Volume Control
    public void PlayMusic(AudioClip musicClip, bool loop = true) {
        if (musicSource.clip == musicClip) return;
        musicSource.Stop();
        musicSource.clip = musicClip;
        //for now, set volume to 0.5f. Later, we can adjust it based on settings.
        musicSource.volume = 0.5f;
       
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void StopMusic() => musicSource.Stop();
    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();

    private AudioMixerGroup GetMixerGroupByType(AudioType type) {
        return type switch {
            AudioType.Music => musicGroup,
            AudioType.SFX => sfxGroup,
            AudioType.UI => uiGroup,
            AudioType.Ambience => ambienceGroup,
            _ => sfxGroup
        };
    }

    public float GetVolume(AudioType type) {
        string key = type switch {
            AudioType.Master => VolumePrefKey,
            AudioType.Music => MusicVolumePrefKey,
            AudioType.SFX => SfxVolumePrefKey,
            AudioType.UI => UiVolumePrefKey,
            AudioType.Ambience => AmbienceVolumePrefKey,
            _ => VolumePrefKey
        };
        return PlayerPrefs.GetFloat(key, 1f);
    }

    public void SetVolume(AudioType type, float value) {
        value = Mathf.Clamp01(value);
        float dbValue = value > 0.0001f ? Mathf.Log10(value) * 20 : -80f;
        string param = type switch {
            AudioType.Master => MasterVolumeParam,
            AudioType.Music => MusicVolumeParam,
            AudioType.SFX => SfxVolumeParam,
            AudioType.UI => UiVolumeParam,
            AudioType.Ambience => AmbienceVolumeParam,
            _ => MasterVolumeParam
        };
        audioMixer.SetFloat(param, dbValue);

        string key = type switch {
            AudioType.Master => VolumePrefKey,
            AudioType.Music => MusicVolumePrefKey,
            AudioType.SFX => SfxVolumePrefKey,
            AudioType.UI => UiVolumePrefKey,
            AudioType.Ambience => AmbienceVolumePrefKey,
            _ => VolumePrefKey
        };
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings() {
        SetVolume(AudioType.Master, PlayerPrefs.GetFloat(VolumePrefKey, 1f));
        SetVolume(AudioType.Music, PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f));
        SetVolume(AudioType.SFX, PlayerPrefs.GetFloat(SfxVolumePrefKey, 1f));
        SetVolume(AudioType.UI, PlayerPrefs.GetFloat(UiVolumePrefKey, 1f));
        SetVolume(AudioType.Ambience, PlayerPrefs.GetFloat(AmbienceVolumePrefKey, 1f));
    }
    #endregion
}

public enum AudioType {
    Master,
    Music,
    SFX,
    UI,
    Ambience
}