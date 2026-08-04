using UnityEngine;

public class SoundsModule : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource audioSrc => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 1f, float p2 = 1f)
    {
        if (clip == null) return; 
        audioSrc.pitch = UnityEngine.Random.Range(p1, p2);
        audioSrc.PlayOneShot(clip, volume);
    }

    public void PlayLoopSound(AudioClip clip, float volume = 1f, float p1 = 1f, float p2 = 1f)
    {
        if (clip == null) return;
        
        if (audioSrc.isPlaying && audioSrc.clip == clip && audioSrc.loop) return;

        audioSrc.pitch = UnityEngine.Random.Range(p1, p2);
        audioSrc.clip = clip;
        audioSrc.loop = true;
        audioSrc.volume = volume;
        audioSrc.Play();
    }

    public void StopSound()
    {
        if (audioSrc != null)
        {
            audioSrc.Stop();
            audioSrc.loop = false;
            audioSrc.clip = null;
        }
    }
}