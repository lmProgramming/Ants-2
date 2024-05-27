using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public Sound[] sounds;
    private static Dictionary<string, float> soundTimerDictionary;

    public static SoundManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        soundTimerDictionary = new Dictionary<string, float>();

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLoop;

            if (sound.hasCooldown)
            {
                Debug.Log(sound.name);
                soundTimerDictionary[sound.name] = 0f;
            }
        }
    }

    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("soundVolume", 1f));

        Play(sounds[0].name);
    }

    public static void Play(string name)
    {
        Sound sound = Array.Find(Instance.sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        if (!CanPlaySound(sound)) return;

        sound.source.Play();
    }

    public static void Stop(string name)
    {
        Sound sound = Array.Find(Instance.sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        sound.source.Stop();
    }

    public static void Pause(string name)
    {
        Sound sound = Array.Find(Instance.sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        sound.source.Pause();
    }

    public static void UnPause(string name)
    {
        Sound sound = Array.Find(Instance.sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        sound.source.UnPause();
    }

    public static bool PlayingSound(string name)
    {
        Sound sound = Array.Find(Instance.sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return false;
        }

        return sound.source.isPlaying;
    }

    private static bool CanPlaySound(Sound sound)
    {
        if (soundTimerDictionary.ContainsKey(sound.name))
        {
            float lastTimePlayed = soundTimerDictionary[sound.name];

            if (lastTimePlayed + sound.clip.length < Time.time)
            {
                // soundTimerDictionary[sound.name] = Time.time;
                return false;
            }

            return false;
        }

        return true;
    }

    public void SetVolume(float val)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.volume = val * sounds[i].volume;
        }
    }

    public void SetVolumeOfSoundEffects(float val)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].type == Sound.Type.Effect)
            {
                sounds[i].source.volume = val * sounds[i].volume;
            }
        }
    }

    public void SetVolumeOfMusicTracks(float val)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].type == Sound.Type.Music)
            {
                sounds[i].source.volume = val * sounds[i].volume;
            }
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool isLoop;
    public bool hasCooldown;
    public AudioSource source;

    public enum Type
    {
        Effect,
        Music
    }

    public Type type;
}