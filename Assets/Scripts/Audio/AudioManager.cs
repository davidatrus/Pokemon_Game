using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;


    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;

    [SerializeField] float fadeDuration=0.75f;

    AudioClip currentMusic;

    float originalVolume;

    Dictionary<AudioID, AudioData> sfxLookup;

    //making it into singleton so its accessable everywhere
    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this; 
    }

    private void Start()
    {
        originalVolume = musicPlayer.volume;

       sfxLookup = sfxList.ToDictionary(x => x.id);
    }

    public void PlaySFX(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;

        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        //oneshot plays our sfx w/o canceling other audio 
        sfxPlayer.PlayOneShot(clip);
    }


    public void PlaySFX(AudioID audioID, bool pauseMusic=false)
    {
        if (!sfxLookup.ContainsKey(audioID)) return;


        var audioData = sfxLookup[audioID];
        PlaySFX(audioData.clip, pauseMusic);
    }


    //function to play music loop var to see if you want to loop music or not
    public void PlayMusic(AudioClip clip, bool loop=true, bool fade= false)
    {
        if (clip == null || clip == currentMusic) return;

        currentMusic = clip;

        StartCoroutine(PlayMusicAsync(clip, loop, fade));

    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        if (fade)
          yield return musicPlayer.DOFade(0,fadeDuration).WaitForCompletion();

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
            yield return musicPlayer.DOFade(originalVolume,fadeDuration).WaitForCompletion();
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalVolume,fadeDuration);
    }
}

public enum AudioID { UISelect, Hit, Faint, ExpGain, SuperHit, WeakHit, MissHit,PokemonObtained,ItemObtained}

[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
}
