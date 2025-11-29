using UnityEngine;
using System.Collections;
using TMPro;

public class BananaVisualization : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float fadeStartTime = 1f;
    [SerializeField] Sprite bananaIcon;
    [SerializeField] Sprite upgradeIcon;

    static BananaVisualization instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BananaVisualization on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static void SpawnAtPosition(Transform pos, int amount)
    {
        GameObject graphic = Instantiate(instance.prefab, pos);
        instance.StartCoroutine(instance.DestroyPopup(graphic, $"+{amount}", amount < 0 ? Color.red : Color.yellow, instance.bananaIcon));
    }

    public static void SpawnAtPosition(Transform pos, string text, Color color, Sprite sprite = null)
    {
        GameObject graphic = Instantiate(instance.prefab, pos);
        if(sprite == null) graphic.transform.position = new Vector3 (graphic.transform.position.x, graphic.transform.position.y + 1f, graphic.transform.position.z);
        instance.StartCoroutine(instance.DestroyPopup(graphic, text, color, sprite == null ? instance.upgradeIcon : sprite));
    }

    IEnumerator DestroyPopup(GameObject obj, string text, Color color, Sprite sprite)
    {
        TMP_Text tmp_text = obj.GetComponentInChildren<TMP_Text>();
        SpriteRenderer icon = obj.GetComponentInChildren<SpriteRenderer>();

        tmp_text.text = text;
        tmp_text.color = color;

        float timer = 0f;

        while(timer < lifetime)
        {     
            timer += Time.deltaTime;
            
            obj.transform.LookAt(Camera.main.transform);
            obj.transform.Rotate(0, 180, 0);
            
            obj.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            
            // Fade out near the end
            if (timer > fadeStartTime)
            {
                float alpha = 1f - ((timer - fadeStartTime) / (lifetime - fadeStartTime));
                
                // Fade text
                Color c = tmp_text.color;
                c.a = alpha;
                tmp_text.color = c;
                
                if(icon != null)
                {
                    Color iconColor = icon.color;
                    iconColor.a = alpha;
                    icon.color = iconColor;   
                    if(sprite != null) icon.sprite = sprite;                 
                }
            }
            
            yield return null;
        }
        Destroy(obj); 
    }
}