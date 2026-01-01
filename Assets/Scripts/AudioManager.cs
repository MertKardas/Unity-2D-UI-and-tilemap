using MyUtility;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager> {

    // --- STRUCT (ZERO-GARBAGE) ---
    // Class yerine Struct kullanýyoruz.
    // 'endTime' yerine 'elapsed' sayacý kullanýyoruz ki Pause yapýnca süreyi dondurabilelim.
    private struct ActiveSound {
        public AudioSource source;
        public AudioData data; 
        public float duration;      // Sesin toplam süresi
        public float elapsed;       // Ne kadar süredir çalýyor?
        public bool isUnscaled;     // UI sesi mi?
        public bool isPaused;       // Þu an duraklatýldý mý?
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

    // Kapasiteyi baþtan veriyoruz (Allocation önlemek için)
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
        // Tersten döngü (Silme iþlemi için güvenli)
        for (int i = activeSounds.Count - 1; i >= 0; i--) {
            // Struct'ýn kopyasýný alýyoruz
            ActiveSound activeSound = activeSounds[i];

            // 1. Güvenlik Kontrolü: Source yok olduysa listeden sil
            if (activeSound.source == null || !activeSound.source.gameObject.activeSelf) {
                activeSounds.RemoveAt(i);
                continue;
            }

            // 2. Loop sesleri süre takibine girmez (Manuel durdurulur)
            if (activeSound.source.loop) continue;

            // 3. Pause Kontrolü: Eðer ses duraklatýldýysa süreyi (elapsed) arttýrma!
            if (activeSound.isPaused) continue;

            // 4. Süre Takibi
            float dt = activeSound.isUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            activeSound.elapsed += dt;

            // Struct deðer tipidir, kopyayý deðiþtirdik. Listeyi güncellememiz lazým.
            // Bu iþlem Stack üzerinde olduðu için çok hýzlýdýr.
            activeSounds[i] = activeSound;

            // 5. Süre Doldu mu?
            if (activeSound.elapsed >= activeSound.duration) {
                ReturnToPool(activeSound.source);
                // ReturnToPool listeyi güncelleyeceði için burada RemoveAt çaðýrmýyoruz.
            }
        }
    }

    private void InitPool(int defaultCapacity, int maxCapacity) {
        audioPool = new ObjectPool<AudioSource>(
            createFunc: () => { 
                var _audioGO = new GameObject("PooledAudioSource");
                _audioGO.AddComponent<AudioSource>();
                return _audioGO.GetComponent<AudioSource>();
            },
            actionOnGet: source => source.gameObject.SetActive(true),
            actionOnRelease: source => {
                source.Stop();
                source.clip = null;
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
        var _audioGO = new GameObject("MusicSource_Template");
        _audioGO.AddComponent<AudioSource>();
        GameObject musicGO = Instantiate(_audioGO, transform);
        musicGO.name = "MusicSource_Main";
        musicSource = musicGO.GetComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
    }

    #region Play Sound Methods

    public AudioSource PlaySound(AudioData data, Vector3 position = default, Transform parent = null) {
        if (data == null) return null;

        AudioSource source = audioPool.Get();

        // Data uygula ve süreyi al
        data.ApplyDataToSource(source, out float length);

        source.outputAudioMixerGroup = GetMixerGroupByType(data.audioType);

        if (parent != null) {
            source.transform.SetParent(parent);
            source.transform.localPosition = Vector3.zero;
        } else {
            source.transform.position = (position == default) ? transform.position : position;
        }

        source.Play();

        // --- TRACKING ---
        // Yeni struct oluþturup listeye atýyoruz. (Garbage Free)
        activeSounds.Add(new ActiveSound {
            source = source,
            duration = length,
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

    #region Pause & Resume Logic (YENÝ)

    /// <summary>
    /// Belirli bir türdeki (Örn: SFX) tüm aktif sesleri duraklatýr.
    /// </summary>
    public void PauseAudioByType(AudioType type) {
        if (type == AudioType.Music) { PauseMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        for (int i = 0; i < activeSounds.Count; i++) {
            // Struct kopyasýný al
            ActiveSound sound = activeSounds[i];

            // Eðer o gruba aitse duraklat
            if (sound.source.outputAudioMixerGroup == targetGroup && !sound.isPaused) {
                sound.source.Pause();
                sound.isPaused = true;

                // Struct'ý listeye geri yaz (Güncelleme)
                activeSounds[i] = sound;
            }
        }
    }

    /// <summary>
    /// Belirli bir türdeki duraklatýlmýþ sesleri devam ettirir.
    /// </summary>
    public void ResumeAudioByType(AudioType type) {
        if (type == AudioType.Music) { ResumeMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        for (int i = 0; i < activeSounds.Count; i++) {
            ActiveSound sound = activeSounds[i];

            if (sound.source.outputAudioMixerGroup == targetGroup && sound.isPaused) {
                sound.source.UnPause();
                sound.isPaused = false;

                // Struct'ý listeye geri yaz
                activeSounds[i] = sound;
            }
        }
    }

    /// <summary>
    /// UI hariç her þeyi duraklatýr (Genelde oyun içi Pause menüsü için)
    /// </summary>
    public void PauseAllGameSounds() {
        PauseAudioByType(AudioType.SFX);
        PauseAudioByType(AudioType.Ambience);
        // Müzik genelde devam eder ama istersen: PauseMusic();
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
        // Manuel for döngüsü (Lambda allocation'dan kaçmak için)
        int index = -1;
        for (int i = 0; i < activeSounds.Count; i++) {
            if (activeSounds[i].source == source) {
                index = i;
                break;
            }
        }

        if (index >= 0) {
            // Swap Removal: Listenin ortasýndan silmek yerine sonuncuyu buraya kopyalayýp sonuncuyu sil.
            // Bu iþlem CPU dostudur (Kaydýrma yapmaz).
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

        // Listeyi boþaltýrken tersten gidip havuza atýyoruz
        while (activeSounds.Count > 0) {
            // Son elemanýn kaynaðýný al
            var source = activeSounds[activeSounds.Count - 1].source;
            // Havuza iade et (ReturnToPool metodu listeyi de temizleyecektir)
            ReturnToPool(source);
        }
    }

    public void StopAudioByType(AudioType type) {
        if (type == AudioType.Music) { StopMusic(); return; }

        var targetGroup = GetMixerGroupByType(type);

        // Tersten dönüyoruz çünkü silme iþlemi yapacaðýz
        for (int i = activeSounds.Count - 1; i >= 0; i--) {
            if (activeSounds[i].source.outputAudioMixerGroup == targetGroup) {
                ReturnToPool(activeSounds[i].source);
            }
        }
    }

    #endregion

    #region Music & Volume Control
    public void PlayMusic(AudioClip musicClip, bool loop = true) {
        if (musicSource.clip == musicClip) return;
        musicSource.Stop();
        musicSource.clip = musicClip;
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