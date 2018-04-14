using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public List<string> Scenes = new List<string>();
    public List<AudioSource> TrackList = new List<AudioSource>();

    private string _currentScene;
    private string _currentTrack;

    private IEnumerator _currentCoroutine;

    void Start()
    {
        _currentScene = SceneController.Instance.GetCurrentSceneName();
        if (Scenes.Contains(_currentScene))
        {
            _currentTrack = TrackList[Scenes.IndexOf(_currentScene)].name;
            _currentCoroutine = FadeInMusic(TrackList[Scenes.IndexOf(_currentScene)]);
            StartCoroutine(_currentCoroutine);
        }
    }

    void Update()
    {
        string currentScene = SceneController.Instance.GetCurrentSceneName();

        if (Scenes.Contains(currentScene) && _currentScene != currentScene && _currentTrack != TrackList[Scenes.IndexOf(currentScene)].name)
        {   
            // Stop fading in; Begin fading out
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = FadeOutMusic(TrackList[Scenes.IndexOf(_currentScene)]);
            StartCoroutine(_currentCoroutine);

            // Start fading in for next track
            _currentScene = currentScene;
            _currentTrack = TrackList[Scenes.IndexOf(_currentScene)].name;
            _currentCoroutine = FadeInMusic(TrackList[Scenes.IndexOf(_currentScene)]);
            StartCoroutine(_currentCoroutine);
        }
    }

    IEnumerator FadeInMusic(AudioSource track)
    {
        float startVolume = track.volume;
        float FadeInTime = 10.0f;

        track.Play();
        while (track.volume < 0.05f)
        {
            track.volume += startVolume * Time.deltaTime / FadeInTime;
            yield return null;
        }
    }

    IEnumerator FadeOutMusic(AudioSource track)
    {
        float startVolume = track.volume;
        float FadeOutTime = 20.0f;

        while (track.volume > 0.001f)
        {
                track.volume -= startVolume * Time.deltaTime / FadeOutTime;

                yield return null;
        }
        track.Stop();
        track.volume = 0.01f;
    }
}
