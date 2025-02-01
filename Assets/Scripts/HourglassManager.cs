using UnityEngine;

public class HourglassManager : MonoBehaviour
{

    // Note: The material names are flipped in the editor so the material named bottom is actually the top material.
    public Material topMaterial;
    public Material bottomMaterial;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateSoulMat(int currentSouls, int maxSouls)
    {
        //Set Top Accordingly, .5 - 1 range

        float currentSoulFloat = (float)currentSouls;
        float maxSoulFloat = (float)maxSouls;

        float topConversion = ((currentSoulFloat/maxSoulFloat)/2f)+.5f;
        topMaterial.SetFloat("soulEmptyAmount", topConversion);

        //Set Bottom Accordingly 0 - .5
        float bottomConversion = .5f - ((currentSoulFloat/maxSoulFloat)/2f);
        bottomMaterial.SetFloat("soulEmptyAmount",bottomConversion);

    }



}
