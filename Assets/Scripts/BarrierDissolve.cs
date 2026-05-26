using UnityEngine;

public class BarrierDissolve : MonoBehaviour
{
    [SerializeField] private MeshRenderer form;
    [SerializeField] private MeshRenderer dissolve;
    private Shield shield;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shield = GetComponent<Shield>(); 
        ChangeBarrierValues();
    }

    // Update is called once per frame
    public void ChangeBarrierValues()
    {
        if (shield != null)
        {
            if (shield.ShieldValue == shield.MaxShieldHealth)
            {
                form.enabled = true;
                dissolve.enabled = true;
                // 1. Vai buscar a cor atual que já está no material
                Color currentColor = form.material.GetColor("_ShieldColor");

                // 3. Aplica a nova cor com a intensidade de volta ao material
                form.material.SetColor("_ShieldColor", currentColor * 2f);
                dissolve.material.SetFloat("_Erosion", 0.25f);
            }
            else if (shield.ShieldValue > 50)
            {
                form.enabled = true;
                dissolve.enabled = true;
                // 1. Vai buscar a cor atual que já está no material
                Color currentColor = form.material.GetColor("_ShieldColor");

                // 3. Aplica a nova cor com a intensidade de volta ao material
                form.material.SetColor("_ShieldColor", currentColor * 0.5f);
                dissolve.material.SetFloat("_Erosion", 0.4f);
            }     
            else if (shield.ShieldValue > 10)
            {
                form.enabled = true;
                dissolve.enabled = true;
                // 1. Vai buscar a cor atual que já está no material
                Color currentColor = form.material.GetColor("_ShieldColor");

                // 3. Aplica a nova cor com a intensidade de volta ao material
                form.material.SetColor("_ShieldColor", currentColor * 0f);
                dissolve.material.SetFloat("_Erosion", 0.6f);
            }
            else
            {
                form.enabled = false;
                dissolve.enabled = false;
            }
        }

    }
}
