using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArcherTowerUpgradeEffects : MonoBehaviour
{
    [SerializeField] GameObject sniperMetalParent;
    [SerializeField] GameObject tackMetalParent;
    [SerializeField] List<Texture> upgradeTextures = new List<Texture>();
    [SerializeField] Texture originalTexture;
    [SerializeField] List<GameObject> bananaModels;

    List<GameObject> sniperMetalRenderers;
    List<GameObject> tackMetalRenderers;

    private readonly List<Vector3> sniperBaseScales = new List<Vector3>();
    private readonly List<Vector3> tackBaseScales = new List<Vector3>();

    void Awake()
    {
        Assert.IsNotNull(sniperMetalParent, "no sniper metal parent in archertower prefab assigned to upgrade effect");
        Assert.IsNotNull(tackMetalParent, "no sniper metal parent in archertower prefab assigned to upgrade effect");
        sniperMetalRenderers = new List<GameObject>();
        foreach (Transform child in sniperMetalParent.transform)
        {
            sniperMetalRenderers.Add(child.gameObject);
        }
        tackMetalRenderers = new List<GameObject>();
        foreach (Transform child in tackMetalParent.transform)
        {
            tackMetalRenderers.Add(child.gameObject);
        }
        // validate lists and cache base scales
        ValidateMetalList(sniperMetalRenderers, sniperBaseScales, "sniperMetalRenderers");
        ValidateMetalList(tackMetalRenderers, tackBaseScales, "tackMetalRenderers");

        Assert.IsTrue(
            upgradeTextures.Count >= ArcherTower.MAX_LEVEL - ArcherTower.CHOOSE_PATH_LEVEL,
            "upgradeTextures should be larger than the available upgrade levels"
        );

        if(originalTexture == null) originalTexture = upgradeTextures[0];
        SetTextureOnAllMetalRenderers(originalTexture);
    }

    private void ValidateMetalList(List<GameObject> renderers, List<Vector3> baseScales, string listName)
    {
        Assert.IsNotNull(renderers, $"{listName} not assigned");
        Assert.IsTrue(renderers.Count > 0, $"{listName} list is empty");

        foreach (var go in renderers)
        {
            Assert.IsNotNull(go, $"{listName} contains a null GameObject");

            var r = go.GetComponent<Renderer>();
            Assert.IsNotNull(r, $"{listName} renderer '{go.name}' missing Renderer");

            baseScales.Add(go.transform.localScale);
            go.SetActive(false);
        }
    }

    public void Upgrade(ArcherTowerType type, int level)
    {
        Assert.IsNotNull(sniperMetalRenderers, "sniperMetalRenderers not assigned");
        Assert.IsNotNull(tackMetalRenderers, "tackMetalRenderers not assigned");

        if (level < ArcherTower.CHOOSE_PATH_LEVEL)
        {
            if (bananaModels != null && bananaModels.Count > 0)
            {
                int halfCount = Math.Max(1, (bananaModels.Count + 1) / 2);
                int i = level == 1 ? 0 : halfCount;
                int cap = level == 1 ? halfCount : bananaModels.Count;
                
                while(i < cap)
                {
                    bananaModels[i].SetActive(true);
                    ++i;
                }
            }
            Assert.AreEqual(type, ArcherTowerType.Base, "upgrading with non base type too early");
            return;
        }
        else Assert.AreNotEqual(type, ArcherTowerType.Base, "upgrading with base type too late");

        Vector3 pos = transform.position;
        if (type == ArcherTowerType.SniperMonkey)
        {
            pos.y = 1.5f;
            transform.position = pos;
            float scale = 1f + 0.08f * (level - ArcherTower.CHOOSE_PATH_LEVEL);
            ApplyUpgradeToList(sniperMetalRenderers, sniperBaseScales, scale, "sniperMetalRenderers");
        }
        else if (type == ArcherTowerType.TackSprayer)
        {
            pos.y = 0;
            transform.position = pos;
            ApplyUpgradeToList(tackMetalRenderers, tackBaseScales, 1f, "tackMetalRenderers");
        }

        int idx = level - ArcherTower.CHOOSE_PATH_LEVEL;
        Debug.Log(idx);
        var texture = upgradeTextures[idx];
        Assert.IsNotNull(texture, "null texture in the upgrade textures list");
        SetTextureOnAllMetalRenderers(texture);
    }

    private void ApplyUpgradeToList(
        List<GameObject> renderers,
        List<Vector3> baseScales,
        float scaleFactor,
        string listName)
    {
        Assert.IsTrue(renderers.Count == baseScales.Count,
            $"{listName} and baseScales out of sync; did you modify the list at runtime?");

        for (int i = 0; i < renderers.Count; i++)
        {
            var go = renderers[i];
            Assert.IsNotNull(go, $"{listName} contains a null GameObject");

            go.SetActive(true);
            go.transform.localScale = baseScales[i] * scaleFactor;
        }
    }

    private void SetTextureOnAllMetalRenderers(Texture texture)
    {
        foreach (var go in sniperMetalRenderers)
        {
            var r = go.GetComponent<Renderer>();
            var mat = r.material;
            mat.mainTexture = texture;
            mat.color = Color.white * 1.6f;
        }

        foreach (var go in tackMetalRenderers)
        {
            var r = go.GetComponent<Renderer>();
            var mat = r.material;
            mat.mainTexture = texture;
            mat.color = Color.white * 1.6f;
        }
    }
}
