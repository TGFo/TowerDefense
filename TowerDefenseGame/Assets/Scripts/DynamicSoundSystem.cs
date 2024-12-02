using UnityEngine;

public class DynamicSoundSystem : MonoBehaviour
{
    [Range(0, 1)]
    public float soundIntensity; // 0 = calm, 1 = intense

    public AudioSource musicSource;
    public AudioSource ambientSource;

    public AudioClip calmMusic;
    public AudioClip intenseMusic;
    public AudioClip calmAmbient;
    public AudioClip intenseAmbient;

    public float transitionSpeed = 1.0f;

    private void Update()
    {
        SetIntensity();
        UpdateMusic();
        UpdateAmbient();
    }

    private void UpdateMusic()
    {
        if (musicSource == null) return;

        AudioClip targetMusic = soundIntensity < 0.5f ? calmMusic : intenseMusic;

        if (musicSource.clip != targetMusic)
        {
            CrossfadeMusic(targetMusic);
        }

        musicSource.volume = Mathf.Lerp(0.3f, 1.0f, soundIntensity);
    }

    private void CrossfadeMusic(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndInMusic(newClip));
    }

    private System.Collections.IEnumerator FadeOutAndInMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / transitionSpeed;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        while (musicSource.volume < Mathf.Lerp(0.3f, 1.0f, soundIntensity))
        {
            musicSource.volume += Time.deltaTime / transitionSpeed;
            yield return null;
        }
    }

    private void UpdateAmbient()
    {
        if (ambientSource == null) return;

        AudioClip targetAmbient = soundIntensity < 0.5f ? calmAmbient : intenseAmbient;

        if (ambientSource.clip != targetAmbient)
        {
            CrossfadeAmbient(targetAmbient);
        }

        ambientSource.volume = Mathf.Lerp(0.2f, 0.8f, soundIntensity);
    }

    private void CrossfadeAmbient(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndInAmbient(newClip));
    }

    private System.Collections.IEnumerator FadeOutAndInAmbient(AudioClip newClip)
    {
        float startVolume = ambientSource.volume;
        while (ambientSource.volume > 0)
        {
            ambientSource.volume -= startVolume * Time.deltaTime / transitionSpeed;
            yield return null;
        }

        ambientSource.Stop();
        ambientSource.clip = newClip;
        ambientSource.Play();

        while (ambientSource.volume < Mathf.Lerp(0.2f, 0.8f, soundIntensity))
        {
            ambientSource.volume += Time.deltaTime / transitionSpeed;
            yield return null;
        }
    }
    public void SetIntensity()
    {
        soundIntensity = ResourceManager.instance.currentWave * 0.1f;
    }
}
