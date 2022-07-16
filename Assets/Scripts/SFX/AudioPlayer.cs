using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [System.Serializable]
    struct Audio
    {
        public string clipName;
        public AudioClip audioClip;
        [Range(0f, 1f)] public float playVolume;
    }

    [Header("Global Settings")]
    [SerializeField][Range(0,1)] float globalSFXVolume = 1f;
    [SerializeField][Range(0,1)] float globalMusicVolume = 1f;

    [Header("Sound Effects")]
    [SerializeField] List<Audio> audioClips = new List<Audio>();
    Dictionary<string, Audio> clipDictionary = new Dictionary<string, Audio>();

    [Header("Music")]
    [SerializeField] float fadeTime = 2f;
    [SerializeField] float defaultBGMTimePause = 3f;
    [SerializeField] string startingBGMName = "level";
    [SerializeField] List<Audio> bgmList = new List<Audio>();
    Dictionary<string, Audio> bgmDictionary = new Dictionary<string, Audio>();
    string currentBGM;
    float currentVolume;
    float volumeProportion;

    Coroutine volumeCoroutine;

    AudioSource audioSource;
    PlayerPrefsHandler prefsHandler;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        prefsHandler = FindObjectOfType<PlayerPrefsHandler>();
    }

    private void OnEnable()
    {
        FindObjectOfType<PauseMenuUI>().OnGamePaused += CalculateCoroutineProgress;
    }

    private void OnDisable()
    {
        var pauseMenu = FindObjectOfType<PauseMenuUI>();
        if (!pauseMenu) return;
        pauseMenu.OnGamePaused -= CalculateCoroutineProgress;
    }

    private void Start()
    {
        currentVolume = 0;
        audioSource.volume = 0;

        PopulateLookupDictionaries();
        UpdateStartingVolumes();
        StartCoroutine(StartNewBGM(startingBGMName, 0));
    }

    public Coroutine FadeOutMusic()
    {
        return FadeOutVolume();
    }

    public void SetGlobalSFXVolume(float amount)
    {
        globalSFXVolume = amount / 10f;
        prefsHandler.SetSFXVolume(amount);
    }

    public void SetGlobalMusicVolume(float amount)
    {
        globalMusicVolume = amount / 10f;
        HandleMusicVolumeUpdate();
        prefsHandler.SetMusicVolume(amount);
    }

    public void PlayAudioClip(string name)
    {
        if (clipDictionary.ContainsKey(name))
        {
            var audio = clipDictionary[name];
            if (audio.audioClip == null) return;
            AudioSource.PlayClipAtPoint(audio.audioClip, Camera.main.transform.position, audio.playVolume * globalSFXVolume);
        }
    }

    public IEnumerator StartNewBGM(string name)
    {
        yield return FadeOutVolume();

        SetBGM(name);

        yield return new WaitForSeconds(defaultBGMTimePause);
        yield return FadeInVolume(bgmDictionary[name].playVolume);
    }

    public IEnumerator StartNewBGM(string name, float delay)
    {
        yield return FadeOutVolume();

        SetBGM(name);

        yield return new WaitForSeconds(delay);
        yield return FadeInVolume(bgmDictionary[name].playVolume);
    }

    private Coroutine FadeOutVolume()
    {
        return UpdateVolume(0, fadeTime);
    }

    private Coroutine FadeInVolume(float volume)
    {
        return UpdateVolume(volume, fadeTime);
    }

    private Coroutine UpdateVolume(float target, float time)
    {
        if (volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }

        volumeCoroutine = StartCoroutine(UpdateVolumeCoroutine(target, time));
        return volumeCoroutine;
    }

    private IEnumerator UpdateVolumeCoroutine(float target, float time)
    {
        while (!Mathf.Approximately(currentVolume, target))
        {
            float setAmount = Mathf.MoveTowards(currentVolume, target, Time.deltaTime / time);
            currentVolume = setAmount;
            audioSource.volume = currentVolume * globalMusicVolume;

            while (Mathf.Approximately(Time.timeScale, Mathf.Epsilon))
            {
                yield return null;
            }

            yield return null;
        }
    }

    void CalculateCoroutineProgress(bool state)
    {
        if (!state) return;

        volumeProportion = currentVolume / bgmDictionary[currentBGM].playVolume;
    }

    private void HandleMusicVolumeUpdate()
    {
        if (volumeCoroutine == null)
        {
            audioSource.volume = currentVolume * globalMusicVolume;
            return;
        }

        audioSource.volume = currentVolume * globalMusicVolume * volumeProportion;
    }


    private void PopulateLookupDictionaries()
    {
        foreach (Audio audio in audioClips)
        {
            clipDictionary[audio.clipName] = audio;
        }

        foreach (Audio audio in bgmList)
        {
            bgmDictionary[audio.clipName] = audio;
        }
    }

    private void UpdateStartingVolumes()
    {
        globalSFXVolume = prefsHandler.GetSFXVolume() / 10f;
        globalMusicVolume = prefsHandler.GetMusicVolume() / 10f;
    }

    private void SetBGM(string name)
    {
        if (!bgmDictionary.ContainsKey(name)) return;
        audioSource.Stop();
        currentBGM = name;
        audioSource.clip = bgmDictionary[name].audioClip;
        audioSource.Play();
    }
}
