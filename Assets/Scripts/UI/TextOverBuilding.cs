using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOverBuilding : MonoBehaviour
{
    public static TextOverBuilding instance;

    private readonly List<TextMeshPro> activeTexts = new List<TextMeshPro>();
    private Camera cam;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        cam = Camera.main;
    }

    public void DisplayText(GameObject target, string text)
    {
        // CREATE text, but DO NOT clear old ones
        GameObject go = new GameObject("WorldText");

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 4f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        // Find model height
        Renderer r = target.GetComponentInChildren<Renderer>();
        float height = r != null ? r.bounds.size.y : 1f;

        // Position ABOVE the model
        Vector3 pos = target.transform.position + Vector3.up * (height + 0.5f);
        go.transform.position = pos;

        // Face camera
        go.transform.rotation = Quaternion.LookRotation(go.transform.position - cam.transform.position);

        activeTexts.Add(tmp);
    }

    public void DisableAllText()
    {
        foreach (TextMeshPro t in activeTexts)
        {
            if (t != null)
                Destroy(t.gameObject);
        }
        activeTexts.Clear();
    }
}