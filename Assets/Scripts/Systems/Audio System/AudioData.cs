using UnityEngine;
[CreateAssetMenu(fileName = "NewAudioData", menuName = "Audio/Audio Data")]
public class AudioData : ScriptableObject {
    [SerializeField] private AudioClip[] audioClips;
    public AudioType audioType = AudioType.SFX;
    public bool loop = false;

    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0f, 3f)] private float pitch = 1f;
    [Range(0f, 1f)] public float volumeVariance = 0f;
    [Range(0f, 1f)] public float pitchVariance = 0f;

    [Header("3D Sound Settings")]
    [Tooltip("0 = 2D sound, 1 = 3D sound")]
    [Range(0f, 1f)] public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
    [Range(0f, 100f)] public float minDistance = 1f;
    [Range(0f, 500f)] public float maxDistance = 500f;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;



    private AudioClip GetRandomClip() {
        if (audioClips == null || audioClips.Length == 0) {
            Debug.LogWarning($"AudioData '{name}' has no clips assigned!");
            return null;
        }
        int index = Random.Range(0, audioClips.Length);
        var clip = audioClips[index];
        return clip;
    }
    private float GetVolumeWithVariance() {
        if (volumeVariance <= 0f)
            return volume;
        float variance = Random.Range(-volumeVariance, volumeVariance);
        return Mathf.Clamp01(volume + variance);
    }
    private float GetPitchWithVariance() {
        if (pitchVariance <= 0f)
            return pitch;
        float variance = Random.Range(-pitchVariance, pitchVariance);
        return Mathf.Clamp(pitch + variance, 0.1f, 3f);
    }
    public AudioSource ApplyDataToSource(AudioSource source, out float length) {
        if (source == null) {
            Debug.LogError("AudioSource is null!");
            length = 0f;    
            return null;
        }
        source.clip = GetRandomClip();
        if (source.clip == null) {
            Debug.LogError("No AudioClip assigned in AudioData: " + name);
            length = 0f;
            return source;
        }
        source.loop = loop;
        source.volume = GetVolumeWithVariance();
        source.pitch = GetPitchWithVariance();
        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = rolloffMode;
        length = source.clip.length / source.pitch; // Adjust length based on pitch
        return source;  
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (maxDistance < minDistance)
        {
            maxDistance = minDistance;
        }
    }
#endif

}