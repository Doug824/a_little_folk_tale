using UnityEngine;
using System.Collections.Generic;

namespace ALittleFolkTale.Core
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<AudioManager>();
                return instance;
            }
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;

        [Header("Audio Collections")]
        [SerializeField] private AudioClip[] musicTracks;
        [SerializeField] private SoundEffect[] soundEffects;
        [SerializeField] private AudioClip[] ambientSounds;

        [Header("Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float ambientVolume = 0.5f;

        private Dictionary<string, AudioClip> soundDictionary;
        private int currentMusicIndex = -1;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSoundDictionary();
        }

        private void InitializeSoundDictionary()
        {
            soundDictionary = new Dictionary<string, AudioClip>();
            
            foreach (var sound in soundEffects)
            {
                if (!soundDictionary.ContainsKey(sound.name))
                {
                    soundDictionary.Add(sound.name, sound.clip);
                }
            }
        }

        public void PlayMusic(int trackIndex)
        {
            if (trackIndex < 0 || trackIndex >= musicTracks.Length)
                return;

            currentMusicIndex = trackIndex;
            musicSource.clip = musicTracks[trackIndex];
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        public void PlayMusic(string trackName)
        {
            for (int i = 0; i < musicTracks.Length; i++)
            {
                if (musicTracks[i].name == trackName)
                {
                    PlayMusic(i);
                    return;
                }
            }
        }

        public void PlaySound(string soundName)
        {
            if (soundDictionary.ContainsKey(soundName))
            {
                sfxSource.PlayOneShot(soundDictionary[soundName], sfxVolume * masterVolume);
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found in AudioManager");
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }

        public void PlayAmbient(int ambientIndex)
        {
            if (ambientIndex < 0 || ambientIndex >= ambientSounds.Length)
                return;

            ambientSource.clip = ambientSounds[ambientIndex];
            ambientSource.volume = ambientVolume * masterVolume;
            ambientSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void StopAmbient()
        {
            ambientSource.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            ambientSource.volume = ambientVolume * masterVolume;
        }

        private void UpdateAllVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
            ambientSource.volume = ambientVolume * masterVolume;
        }

        public void FadeMusic(float duration, float targetVolume)
        {
            StartCoroutine(FadeAudioSource(musicSource, duration, targetVolume * masterVolume));
        }

        private System.Collections.IEnumerator FadeAudioSource(AudioSource source, float duration, float targetVolume)
        {
            float startVolume = source.volume;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }
    }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }
}