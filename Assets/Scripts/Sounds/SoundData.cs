using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public AudioClip[] clips; // можно несколько, чтобы играть случайный
    public float volume = 1f;
    public bool loop = false;

    public AudioCategory category;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;
        return clips[Random.Range(0, clips.Length)];
    }
}

public enum AudioCategory
{
    SFX,
    Music,
    UI,
    Voice,
    Ambient
}
