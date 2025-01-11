using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Upravljanje posameznega polja v mreži nonograma

public class GridSquare : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int state; // Stanje polja: 0 = belo, 1 = črno, 2 = "X"
    public int x, y; // Koordinate polja v mreži
    public Image squareImage; // Slika polja
    public Text stateText; // Besedilo za prikaz stanja polja
    public Vector2Int pos; // Dvodimenzionalni vektor za shranjevanje pozicije
    
    // Inicializacija ob prebujanju komponente
    private void Awake()
    {
        squareImage = GetComponent<Image>(); // Pridobi komponento slike
        stateText = GetComponentInChildren<Text>(); // Pridobi komponento besedila
    }

    // Nastavi stanje polja
    public void SetState(int newState)
    {
        state = newState; // Posodobi trenutno stanje

        if (state == 0) // Belo polje
        {
            squareImage.color = Color.white;
            stateText.text = "";
        }
        else if (state == 1) // Črno polje
        {
            squareImage.color = Color.black;
            stateText.text = "";
        }
        else if (state == 2) // Označeno z "X" in belo polje
        {
            squareImage.color = Color.white;
            stateText.text = "X";
            stateText.color = Color.black;
        }
    }

    // Nastavi položaj polja v mreži glede na vrstice in stolpce
    public void SetPosition(int row, int col)
    {
        x = row;
        y = col;
        pos = new Vector2Int(x, y);
    }

    // Nastavitev položaja z vektorjem
    public void SetPosition(Vector2Int p)
    {
        SetPosition(p.x, p.y);
    }

    // Nastavi velikost polja in prilagodi velikost pisave glede na velikost polja
    public void SetSize(int size)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(size * 0.9f, size * 0.9f);
        stateText.fontSize = size / 2;
    }

    // Upravljanje ob premiku miške nad poljem
    public void OnPointerEnter(PointerEventData eventData)
    {
        PuzzleGrid.selectedSquare = this; // Nastavi izbrano polje ob vstopu kazalca
    }

    // Upravljanje ob umiku miške iz polja
    public void OnPointerExit(PointerEventData eventData)
    {
        PuzzleGrid.selectedSquare = null; // Odstrani izbrano polje ob izstopu kazalca
    }
}