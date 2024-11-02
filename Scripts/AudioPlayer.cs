using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Indie
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        [Header("Audio Tracks")]
        [SerializeField] private List<AudioClip> audioTracks = new List<AudioClip>();
        [SerializeField] private float fadeDuration = 1.5f; // Duration of crossfade between tracks
        [SerializeField] private float transitionStartThreshold = 3f; // Time left in track before transition starts

        public AudioSource audioSource;
        [SerializeField] private int currentTrackIndex;
        [SerializeField] private bool isFading;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false; // Do not loop to allow transitions
            audioSource.volume = 0; // Start with volume at 0 for fade-in effect
        }

        private void Start()
        {
            Play();
        }

        private void Update()
        {
            // If nearing the end of the current track, start fading to the next
            if (!isFading && audioSource.isPlaying && audioSource.time >= audioSource.clip.length - transitionStartThreshold)
            {
                StartCoroutine(FadeToNextTrack());
            }
        }

        private void PlayTrack(int index)
        {
            if (index < 0 || index >= audioTracks.Count) return;

            audioSource.clip = audioTracks[index];
            audioSource.Play();
            currentTrackIndex = index;

            // Fade in the track
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeToNextTrack()
        {
            isFading = true;

            // Fade out current track
            yield return StartCoroutine(FadeOut());

            // Move to the next track and start playing
            int nextTrackIndex = (currentTrackIndex + 1) % audioTracks.Count;
            PlayTrack(nextTrackIndex);

            isFading = false;
        }

        private IEnumerator FadeIn()
        {
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 1;
        }

        private IEnumerator FadeOut()
        {
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0;
        }

        public void SetTrackList(List<AudioClip> tracks)
        {
            audioTracks = tracks;
            currentTrackIndex = 0;
            PlayTrack(currentTrackIndex);
        }

        public void Stop()
        {
            StartCoroutine(FadeOutAndStop());
        }

        public void Pause()
        {
            StartCoroutine(FadeOutAndPause());
        }

        public void UnPause()
        {
            audioSource.UnPause();
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeOutAndStop()
        {
            yield return StartCoroutine(FadeOut());
            audioSource.Stop();
        }

        private IEnumerator FadeOutAndPause()
        {
            yield return StartCoroutine(FadeOut());
            audioSource.Pause();
        }

        public void Play()
        {
            PlayTrack(0);
        }

        public void Next()
        {
            StartCoroutine(FadeToNextTrack());
        }
    }
}
