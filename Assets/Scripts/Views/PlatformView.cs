using UnityEngine;

public class PlatformView : MonoBehaviour
{
    public void Init(float width, float depth, Vector3 pos, bool fromRight)
    {
        transform.position = pos;
        transform.localScale = new Vector3(width, transform.localScale.y, depth);

        // TODO: forwarding color or direction adjustments
    }
    
}
