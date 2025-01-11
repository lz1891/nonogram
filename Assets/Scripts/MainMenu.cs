using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Upravljanje glavnega menija za prilagoditev nastavitev in začetek igre

public class MainMenu : MonoBehaviour
{
    public Slider difficultySlider; // Drsnik za izbiro težavnosti
    public Text difficultyText; // Prikaz izbrane težavnosti

    // Dovoljene vrednosti za drsnik
    private readonly int[] allowedValues = {1, 2, 3, 4, 5};

    private void Start()
    {
        // Nastavi vrednost drsnika iz PlayerPrefs ali privzeto na 1, če ni nastavljena
        int savedValue = PlayerPrefs.GetInt("Difficulty", allowedValues[0]);

        // Nastavi drsnik na najbližjo dovoljeno vrednost
        difficultySlider.value = GetClosestAllowedValue(savedValue);

        // AddListener za spremembe drsnika, da vedno skoči na dovoljene vrednosti
        difficultySlider.onValueChanged.AddListener(SnapToAllowedValue);

        // Posodobi besedilo
        UpdateDifficultyText(difficultySlider.value);
    }

    // Nastavi vrednost drsnika na najbližjo dovoljeno vrednost
    private void SnapToAllowedValue(float value)
    {
        // Poišči najbližjo dovoljeno vrednost trenutni vrednosti drsnika
        difficultySlider.value = GetClosestAllowedValue((int)value);

        // Posodobi besedilo
        UpdateDifficultyText(difficultySlider.value);
    }

    // Najdi najbližjo dovoljeno vrednost glede na trenutni vnos
    private int GetClosestAllowedValue(int value)
    {
        int closest = allowedValues[0]; // Privzeto nastavi na prvo dovoljeno vrednost
        foreach (int v in allowedValues)
        {
            // Preveri razdaljo do trenutne vrednosti in poišči najbližjo
            if (Mathf.Abs(value - v) < Mathf.Abs(value - closest))
            {
                closest = v;
            }
        }
        return closest;
    }

    // Posodobi besedilo, ki prikazuje izbrano stopnjo težavnosti
    private void UpdateDifficultyText(float value)
    {
        int difficulty = (int)value;
        difficultyText.text = $"{difficulty}";
    }

    // Začni igro in shrani stopnjo težavnosti
    public void StartGame()
    {
        int difficulty = (int)difficultySlider.value; // Pridobi trenutno vrednost drsnika
        PlayerPrefs.SetInt("Difficulty", difficulty); // Shrani težavnost
        
        // Naloži igro
        SceneManager.LoadScene("Game");
    }

    // Zapri igro
    public void QuitGame()
    {
        Application.Quit(); // Zapri igro
    }
}