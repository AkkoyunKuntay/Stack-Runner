using UnityEngine;

public class PlatformView : MonoBehaviour
{
    [SerializeField] Renderer rend;

    public bool isFinal;
    [SerializeField] private Material debrisMat;
    public float Width => transform.localScale.x; 
    public float Depth => transform.localScale.z;


    public void Init(float width, float depth, Vector3 pos, bool fromRight)
    {
        rend = GetComponentInChildren<Renderer>();
        transform.position = pos;
        transform.localScale = new Vector3(width, transform.localScale.y, depth);
    }

    public void Resize(float newWidth)
    {
        Vector3 s = transform.localScale;
        s.x = newWidth;
        transform.localScale = s;
    }

    public void SpawnDebris(float width, int dirSign)
    {
        GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debris.transform.localScale = new Vector3(width, transform.localScale.y, Depth);

        float half = Width / 2f + width / 2f;
        Vector3 pos = transform.position + Vector3.right * dirSign * half;
        debris.transform.position = pos;
        debris.GetComponent<MeshRenderer>().material = debrisMat;
        debris.GetComponent<Collider>().enabled = false;
        var rb = debris.AddComponent<Rigidbody>();
        rb.mass = 2f;
        Destroy(debris, 3f); 
    }

    public void ApplyMaterial(Material mat)    
    {
        if (rend != null && mat != null)
        {
            rend.material = mat;
            debrisMat = mat;
        }         
    }
}
