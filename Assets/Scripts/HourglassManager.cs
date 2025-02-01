using UnityEngine;

public class HourglassManager : MonoBehaviour
{

    // Note: The material names are flipped in the editor so the material named bottom is actually the top material.
    public Material topMaterial;
    public Material bottomMaterial;

    public GameObject hourglass;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Material test;

    public void UpdateSoulMat(int currentSouls, int maxSouls)
    {
        //Set Top Accordingly, .5 - 1 range

        float currentSoulFloat = (float)currentSouls;
        float maxSoulFloat = (float)maxSouls;

        float topConversion = ((currentSoulFloat/maxSoulFloat)/2f)+.5f;
        topMaterial.SetFloat("soulEmptyAmount", topConversion);
        hourglass.GetComponent<Renderer>().materials[3] = topMaterial;

        

        //Set Bottom Accordingly 0 - .5 range
        float bottomConversion = .5f - ((currentSoulFloat/maxSoulFloat)/2f);
        bottomMaterial.SetFloat("soulEmptyAmount", bottomConversion);
        hourglass.GetComponent<Renderer>().materials[2] = test;
        Debug.Log("Material's soulEmptyAmount: " + bottomMaterial.GetFloat("soulEmptyAmount"));

    }



}
