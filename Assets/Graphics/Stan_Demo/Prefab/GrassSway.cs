using UnityEngine;

public class GrassSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAngleZ = 10f;       // Max side-to-side tilt (Z-axis)
    public float swaySpeed = 1f;         // Speed of the swaying motion
    public float scaleVariation = 0.02f; // Optional vertical "breathing" effect

    private float initialZ;
    private float initialScaleY;
    private float randomOffset;

    private void Start()
    {
        initialZ = transform.localEulerAngles.z;
        initialScaleY = transform.localScale.y;

        randomOffset = Random.Range(0f, Mathf.PI * 2f); // random phase so each blade moves uniquely
        swaySpeed *= Random.Range(0.8f, 1.2f);          // small natural variation
        swayAngleZ *= Random.Range(0.8f, 1.2f);
    }

    private void Update()
    {
        float time = Time.time * swaySpeed + randomOffset;

        // Simple side-to-side sway
        float swayZ = Mathf.Sin(time) * swayAngleZ;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, initialZ + swayZ);

        // Optional subtle vertical scale variation (breathing)
        if (scaleVariation > 0)
        {
            float scaleOffset = Mathf.Sin(time) * scaleVariation;
            transform.localScale = new Vector3(transform.localScale.x, initialScaleY + scaleOffset, transform.localScale.z);
        }
    }
}
