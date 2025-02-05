using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager: MonoBehaviour
    {
        [SerializeField] AudioClip _click, _deselect, _match, _noMatch, _woosh, _pop;
        AudioSource _audioSource;
        public void PlayClick() => _audioSource.PlayOneShot(_click);
        public void PlayDeselect() => _audioSource.PlayOneShot(_deselect);
        public void PlayMatch() => _audioSource.PlayOneShot(_match);
        public void PlayNomatch() => _audioSource.PlayOneShot(_noMatch);
        public void PlayWoosh() => _audioSource.PlayOneShot(_woosh);
        public void PlayPop() => PlayPitch(_pop);

        void Awake() => _audioSource = GetComponent<AudioSource>();

        public void PlayPitch(AudioClip audioClip)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(audioClip);
            _audioSource.pitch = 1;
        }
    }
}
