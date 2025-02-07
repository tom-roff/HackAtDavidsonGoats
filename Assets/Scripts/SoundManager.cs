using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string soundName;         // Unique identifier
        public AudioClip clip;           // The AudioClip
        [Range(0f, 1f)]
        public float volume = 1f;        // Volume scaling
        public AudioMixerGroup mixerGroup; // Optional: for routing through an Audio Mixer
    }

    // Array to populate in the Inspector with all sound effects and music
    public Sound[] sounds;

    // Serialized prefab for pooled AudioSources
    public AudioSource audioSourcePrefab;

    // Private mapping for sound lookup
    private Dictionary<string, Sound> soundDict;

    // Pool of AudioSources to handle overlapping playbacks
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    
    void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize the sound dictionary for quick lookup.
        soundDict = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.soundName))
            {
                soundDict.Add(s.soundName, s);
            }
        }
    }

    // Plays sound at a given position, start time, and end time
    public void PlaySound(string name, Vector3 position, float startNormalized = 0f, float endNormalized = 1f)
    {
        if (!soundDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.transform.position = position;
        source.clip = s.clip;
        source.volume = s.volume;
        if (s.mixerGroup != null)
        {
            source.outputAudioMixerGroup = s.mixerGroup;
        }

        // Ensures values are between 0 and 1
        startNormalized = Mathf.Clamp01(startNormalized);
        endNormalized = Mathf.Clamp01(endNormalized);

        // Ensures end is greater than start
        if (endNormalized <= startNormalized)
        {
            Debug.LogWarning("endNormalized was not greater than startNormalized, playing full clip instead.");
            startNormalized = 0f;
            endNormalized = 1f;
        }

        // Calculates actual start and end time
        float startTime = s.clip.length * startNormalized;
        float segmentDuration = s.clip.length * (endNormalized - startNormalized);

        source.time = startTime;
        source.Play();

        source.SetScheduledEndTime(AudioSettings.dspTime + segmentDuration);
    }



    public void PlaySound(string name)
    {
        PlaySound(name, Vector3.zero);
    }

    // Checks for an idle AudioSource, otherwise creates one
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource src in audioSourcePool)
        {
            if (!src.isPlaying)
            {
                return src;
            }
        }
        
        // No available source found; instantiate a new one.
        AudioSource newSource = Instantiate(audioSourcePrefab, transform);
        audioSourcePool.Add(newSource);
        return newSource;
    }
}
