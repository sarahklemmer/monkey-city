using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    [SerializeField] Scene gameplayScene;

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
        Assert.IsTrue(gameplayScene.IsValid(), "gameplayScene invalild!");
        SceneManager.LoadScene(gameplayScene.name);
    }
}