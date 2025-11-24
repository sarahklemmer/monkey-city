using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleFlasher : MonoBehaviour
{
    private Image flashingImage;
    public bool active {get; private set;} = false;
    public static ToggleFlasher instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate ToggleFlasher on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {   
        flashingImage = GetComponentInChildren<Image>();
        Assert.IsNotNull(flashingImage, "no image on the toggle");
        StartFlash();
    }

    public void StartFlash()
    {
        if (active) return;
        StartCoroutine(Flash());
    }

    public void StopFlash()
    {
        active = false;
    }

    IEnumerator Flash()
    {
        active = true;

        Assert.IsNotNull(flashingImage, "toggle graphic is null");
        Color baseColor = flashingImage.color;

        float speed = 1.5f;

        // precompute fully visible and fully invisible versions
        Color visible = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        Color invisible = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        while (active)
        {
            // Fade in (0 → 1 alpha)
            float t = 0f;
            while (t < 1f && active)
            {
                flashingImage.color = Color.Lerp(invisible, visible, t);
                t += Time.deltaTime * speed;
                yield return null;
            }

            // Fade out (1 → 0 alpha)
            t = 0f;
            while (t < 1f && active)
            {
                flashingImage.color = Color.Lerp(visible, invisible, t);
                t += Time.deltaTime * speed;
                yield return null;
            }
        }

        // restore the original color when finished flashing
        flashingImage.color = baseColor;
    }
}