using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    [SerializeField] private AudioClip soundtrack;
    [SerializeField] private bool playOnStart = true;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource source;
    public static Soundtrack instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate Soundtrack on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Soundtrack: No MainCamera found (tag a camera as MainCamera).");
            return;
        }

        // Reuse existing AudioSource on camera if present, otherwise add one.
        source = cam.GetComponent<AudioSource>();
        if (source == null) source = cam.gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = true;
        source.volume = volume;
        source.clip = soundtrack;

    }

    void Start()
    {
        if (playOnStart) PlaySoundtrack();
    }

    public void PlaySoundtrack()
    {
        if (source == null) return;
        if (source.clip == null)
        {
            Debug.LogWarning("Soundtrack: No AudioClip assigned.");
            return;
        }
        if (!source.isPlaying) source.Play();
    }

    public void StopSoundtrack()
    {
        if (source == null) return;
        if (source.isPlaying) source.Stop();
    }

    // Optional helper if you want to swap tracks later
    public void SetSoundtrack(AudioClip clip, bool playImmediately = true)
    {
        soundtrack = clip;
        if (source == null) return;
        source.clip = clip;
        if (playImmediately) PlaySoundtrack();
    }

    public void ToggleSoundtrack()
    {
        if (source.isPlaying) source.Stop();
        else source.Play();
    }

    public bool Playing() => source.isPlaying;
}