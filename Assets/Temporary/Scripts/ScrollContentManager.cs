using UnityEngine;
using UnityEngine.UI;

public class ScrollContentManager : MonoBehaviour
{
    // Reference to the GridLayoutGroup on this object
    private GridLayoutGroup grid;

    // Reference to this object's RectTransform
    private RectTransform rectTransform;

    int childrenNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // Get components
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        childrenNumber = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        //Definir o tamanho do objeto
        float howManyRows;
        howManyRows = (Mathf.Round(childrenNumber / grid.constraintCount) * 5f) / 5f;

        Vector2 size = rectTransform.sizeDelta;
        size.y =(
                    (howManyRows * grid.cellSize.y) //Contando quantos itens tem na horizontal, com o seu tamanho
                    +
                    ((howManyRows - 1) * grid.spacing.y) //Contando o espaçamento entre os itens
                    +
                    grid.padding.top //Adicionando o padding superior
                    +
                    grid.padding.bottom //Adicionando o padding inferior
                );
        rectTransform.sizeDelta = size;
    }
}
