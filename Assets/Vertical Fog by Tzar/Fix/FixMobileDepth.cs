using UnityEngine;

[ExecuteInEditMode]
public class FixMobileDepth : MonoBehaviour {
    [SerializeField] Transform fog;
    float yPos;
    private void Awake() {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        yPos = fog.position.y;
    }

    private void Update() {
        fog.transform.position = new Vector3(fog.transform.position.x, yPos, fog.transform.position.z);
    }
}
