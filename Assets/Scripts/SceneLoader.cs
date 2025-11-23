using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate SceneLoader on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ReloadScene()
    {
        BuildingUnlock.Reset();
        Scene scene = SceneManager.GetActiveScene();
        Assert.IsTrue(scene.IsValid(), "gameplayScene invalild!");
        SceneManager.LoadScene(scene.name);
    }
}