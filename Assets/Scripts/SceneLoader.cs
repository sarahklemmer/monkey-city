using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class SceneLoader : MonoBehaviour
{
    static Scene scene;

    void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }
    
    public static void ReloadScene()
    {
        Assert.IsTrue(scene.IsValid(), "Couldn't grab scene properly!");
        SceneManager.LoadScene(scene.name);
    }
}