using UnityEngine;

public class RainController : MonoBehaviour
{
    [Header("Rain Settings")]
    public ParticleSystem rainParticles;
    public Light directionalLight;
    public Material[] groundMaterials;    // List of ground materials
    public string maskProperty = "RainMask";  // Shader property name (exposed slider)
    public Color rainLightColor = new Color(0.5f, 0.5f, 0.6f);
    public float sunnyIntensity = 2f;
    public float rainIntensity = 1f;
    public float maxRainEmission = 100f;
    public float transitionDuration = 2f;

    [Header("Props Settings")]
    public Transform propsContainer;      // Parent of all props
    public Color rainPropsColor = new Color(0.5f, 0.5f, 0.6f);

    private Color originalLightColor;
    private float originalLightIntensity;
    private ParticleSystem.EmissionModule rainEmission;
    private bool isRaining = false;
    private float currentMask = 0f;
    private Color[] originalPropsColors;
    private Renderer[] propRenderers;

    void Start()
    {
        // Store original light settings
        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
            originalLightIntensity = directionalLight.intensity;
        }

        // Prepare rain particle system
        if (rainParticles != null)
        {
            rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            rainEmission = rainParticles.emission;
            rainEmission.rateOverTime = 0f; // start with no emission
        }

        // Store original colors of props
        if (propsContainer != null)
        {
            propRenderers = propsContainer.GetComponentsInChildren<Renderer>();
            originalPropsColors = new Color[propRenderers.Length];
            for (int i = 0; i < propRenderers.Length; i++)
            {
                originalPropsColors[i] = propRenderers[i].material.color;
            }
        }
    }

    void Update()
    {
        // Toggle rain with "R" key
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRaining = !isRaining;
        }

        float delta = Time.deltaTime / transitionDuration;

        // Smoothly transition light
        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(directionalLight.color, isRaining ? rainLightColor : originalLightColor, delta);
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, isRaining ? rainIntensity : sunnyIntensity, delta);
        }

        // Smoothly transition rain particles
        if (rainParticles != null)
        {
            float targetRate = isRaining ? maxRainEmission : 0f;
            float currentRate = Mathf.Lerp(rainEmission.rateOverTime.constant, targetRate, delta);
            rainEmission.rateOverTime = currentRate;

            if (isRaining && !rainParticles.isPlaying)
                rainParticles.Play();
        }

        // Smoothly transition wetness mask for all materials
        float targetMask = isRaining ? 1f : 0f;
        currentMask = Mathf.Lerp(currentMask, targetMask, delta);

        foreach (var mat in groundMaterials)
        {
            if (mat != null)
            {
                mat.SetFloat(maskProperty, currentMask);
            }
        }

        // Smoothly transition props colors
        if (propRenderers != null)
        {
            for (int i = 0; i < propRenderers.Length; i++)
            {
                Color targetColor = isRaining ? rainPropsColor : originalPropsColors[i];
                propRenderers[i].material.color = Color.Lerp(propRenderers[i].material.color, targetColor, delta);
            }
        }
    }

    [ContextMenu("Activate Rain")]
    public void ActivateRain() => isRaining = true;

    [ContextMenu("Deactivate Rain")]
    public void DeactivateRain() => isRaining = false;
}
