using UnityEngine;

public class GameObjectDestruction : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;

    public void SafeDestroy()
    {
        if (targetObj == null)
        {
            return;
        }

        Object.Destroy(targetObj);
    }
}