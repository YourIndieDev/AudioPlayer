using System.Collections.Generic;
using UnityEngine;

namespace Indie
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Players")]
        [SerializeField] private List<AudioPlayer> audioPlayers = new List<AudioPlayer>();
        [SerializeField] private float crossfadeDuration = 2f;

        private AudioPlayer activePlayer;
        private AudioPlayer nextPlayer;
        private bool isTransitioning;

        [SerializeField] private int switchToIndex = 0;

        private void Start()
        {
            if (audioPlayers.Count > 0)
            {
                activePlayer = audioPlayers[0];
                activePlayer.gameObject.SetActive(true);
                for (int i = 1; i < audioPlayers.Count; i++)
                {
                    audioPlayers[i].gameObject.SetActive(false);
                }
            }
        }

        public void PlayPlaylist(List<AudioClip> tracks, int playerIndex)
        {
            if (isTransitioning || playerIndex < 0 || playerIndex >= audioPlayers.Count)
                return;

            nextPlayer = audioPlayers[playerIndex];
            nextPlayer.SetTrackList(tracks);
            StartCoroutine(CrossfadeToNextPlayer());
        }

        private System.Collections.IEnumerator CrossfadeToNextPlayer()
        {
            isTransitioning = true;

            // Activate the next player and fade it in
            nextPlayer.gameObject.SetActive(true);
            nextPlayer.Stop();
            AudioSource nextSource = nextPlayer.audioSource;
            AudioSource activeSource = activePlayer.audioSource;

            float startVolume = activeSource.volume;
            for (float t = 0; t < crossfadeDuration; t += Time.deltaTime)
            {
                float progress = t / crossfadeDuration;
                activeSource.volume = Mathf.Lerp(startVolume, 0, progress);
                nextSource.volume = Mathf.Lerp(0, startVolume, progress);
                yield return null;
            }

            // Finalize the transition
            activeSource.volume = 0;
            nextSource.volume = startVolume;
            activePlayer.Stop();
            activePlayer.gameObject.SetActive(false);
            activePlayer = nextPlayer;

            activePlayer.Play();

            isTransitioning = false;
        }

        public void SwtichAudioPlayer()
        {
            nextPlayer = audioPlayers[switchToIndex];
            StartCoroutine(CrossfadeToNextPlayer());
        }
    }
}

