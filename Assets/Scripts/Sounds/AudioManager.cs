using UnityEngine;

public class SOAudioManager : MonoBehaviour
{
    public static SOAudioManager Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySound(SoundData soundData)
    {
        if (soundData == null) return;

        var clip = soundData.GetRandomClip();
        if (clip == null) return;

        switch (soundData.category)
        {
            case AudioCategory.SFX:
                sfxSource.PlayOneShot(clip, soundData.volume);
                break;
            case AudioCategory.Music:
                musicSource.clip = clip;
                musicSource.volume = soundData.volume;
                musicSource.loop = soundData.loop;
                musicSource.Play();
                break;
            case AudioCategory.UI:
                sfxSource.PlayOneShot(clip, soundData.volume);
                break;
            default:
                sfxSource.PlayOneShot(clip, soundData.volume);
                break;
        }
    }
}
