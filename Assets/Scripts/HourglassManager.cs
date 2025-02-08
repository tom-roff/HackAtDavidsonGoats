using UnityEditor;
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

    Material[] materials;




    public void UpdateSoulMat(int currentSouls, int maxSouls)
    {

        Material newTopInst = new Material(topMaterial);
        Material newBottomInst = new Material(bottomMaterial);
        //Set Top Accordingly, .16 - .3 range
        ShaderUtil.allowAsyncCompilation = false;

        float currentSoulFloat = (float)currentSouls;
        float maxSoulFloat = (float)maxSouls;

        

        float topConversion = ((currentSoulFloat/maxSoulFloat)*.14f)+.16f;
        newTopInst.SetFloat("_soulEmptyAmount", topConversion);

        


        //Set Bottom Accordingly .01 - .16 range
        float bottomConversion = .16f - ((currentSoulFloat/maxSoulFloat)*.15f) + .01f;
        newBottomInst.SetFloat("_soulEmptyAmount", bottomConversion);
        // Debug.Log("Material's soulEmptyAmount: " + newBottomInst.GetFloat("_soulEmptyAmount"));

        topMaterial.SetShaderPassEnabled("ForwardLit", true);
        newBottomInst.SetShaderPassEnabled("ForwardLit", true);

        materials = hourglass.GetComponent<Renderer>().materials;
        materials[2] = newBottomInst;
        materials[3] = newTopInst;

        hourglass.GetComponent<Renderer>().materials = materials;

    }



}
