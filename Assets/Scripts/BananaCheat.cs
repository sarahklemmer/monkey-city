using UnityEngine;

public class BananaCheat : MonoBehaviour
{
    void Update()
    {
        //cheat for bananas :D
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            BananaManager.instance.AddBananas(999);
        }
    }
}
