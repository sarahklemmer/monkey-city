using UnityEngine;

public class BananaTesterDELETE : MonoBehaviour
{
    [SerializeField] private int bananasToAdd = 10;
    [SerializeField] private bool autoIncrement = false;
    [SerializeField] private float autoIncrementInterval = 2f;
    
    private float timer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (BananaManager.instance != null)
            {
                BananaManager.instance.AddBananas(bananasToAdd);
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (BananaManager.instance != null)
            {
                BananaManager.instance.AddBananas(100);
            }
        }

        if (autoIncrement)
        {
            timer += Time.deltaTime;
            if (timer >= autoIncrementInterval)
            {
                timer = 0f;
                if (BananaManager.instance != null)
                {
                    BananaManager.instance.AddBananas(bananasToAdd);
                }
            }
        }
    }
}