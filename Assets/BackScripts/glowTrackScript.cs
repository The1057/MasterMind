using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class glowTrackScript : MonoBehaviour
{
    public Camera cam;
    public GameObject glow;
    public float distance,glowHeight;
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        Vector3 direction = cam.transform.position - glow.transform.parent.position;
        direction = direction.normalized;
        direction *= distance;
        direction.y = glowHeight;
        glow.transform.position = glow.transform.parent.position - direction;
        glow.transform.LookAt(cam.transform.position);
        glow.transform.Rotate(0,0,90);
        
    }
}
