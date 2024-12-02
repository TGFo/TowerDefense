using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWeatherSystem : MonoBehaviour
{
    [Range(0, 1)]
    public float weatherIntensity;

    public Light directionalLight;
    public Material skyboxMaterial;

    private Color sunnyLightColor = new Color(.95f, 1f, 0.8f);
    private Color gloomyLightColor = new Color(0.5f, 0.5f, 0.6f);

    private Color sunnySkyColor = new Color(0.81f, 0.53f, 0.98f);
    private Color gloomySkyColor = new Color(0.2f, 0.2f, 0.3f);

    public GameObject cloudPlane;
    private Material cloudMaterial;

    public Material renderMat;

    private void Update()
    {
        weatherIntensity = ResourceManager.instance.GetCurrentTowerPercentage();
        UpdateLighting();
        UpdateSkybox();
        UpdateFog();
        UpdateClouds();
    }

    private void UpdateLighting()
    {
        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(gloomyLightColor, sunnyLightColor, weatherIntensity);
            directionalLight.intensity = Mathf.Lerp(0.3f, 1.5f, weatherIntensity);
        }
    }

    private void UpdateSkybox()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_SkyTint", Color.Lerp(gloomySkyColor, sunnySkyColor, weatherIntensity));
        }
    }

    private void UpdateFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.Lerp(gloomySkyColor, sunnySkyColor, weatherIntensity);
        RenderSettings.fogDensity = Mathf.Lerp(0.02f, 0.001f, weatherIntensity);
    }

    private void UpdateClouds()
    {
        if(cloudMaterial == null)
        {
            cloudMaterial = cloudPlane.GetComponent<Renderer>().material;
        }
        cloudMaterial.SetFloat("_Opacity", weatherIntensity / .1f);
        cloudMaterial.SetFloat("_CloudDensity", ResourceManager.instance.currentWave / .1f + 0.001f);
        cloudMaterial.SetColor("_SkyColour", Color.Lerp(sunnySkyColor, gloomySkyColor, weatherIntensity));
        cloudMaterial.SetFloat("_DisplacementStrength", weatherIntensity * 2);
        cloudMaterial.SetColor("_AmbientColour", Color.Lerp(sunnySkyColor, gloomySkyColor, weatherIntensity));
        renderMat.SetFloat("_ColourResolution", Mathf.Clamp(weatherIntensity * 8, 3, 256));
    }
    private void OnDisable()
    {
        renderMat.SetFloat("_ColourResolution",8);
        skyboxMaterial.SetColor("_SkyTint", gloomySkyColor);
    }
}