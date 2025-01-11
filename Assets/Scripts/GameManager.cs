using UnityEngine;
using UnityEngine.SceneManagement;

// Upravljanje nastavitev in inicializacija nonograma

public class GameManager : MonoBehaviour
{
    public int rows; // Število vrstic (glede na izbiro)
    public int columns; // Število stolpcev (glede na izbiro)
    public PuzzleGrid puzzleGrid; // Skripta za upravljanje mreže

    public float fillRate; // Stopnja zapolnitve polj s črnimi polji

    private void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);

        // Nastavi parametre glede na težavnost
        (int rows, int columns, float fillRate) = GetDifficultySettings(difficulty);

        // Ustvari nov nonogram in ga naloži v mrežo
        int[,] generatedPuzzle = GeneratePuzzle(rows, columns, fillRate);
        puzzleGrid.InitializeGrid(rows, columns, generatedPuzzle);
        puzzleGrid.isPlaying = true; // Začni igro
    }

    // Nastavitve za določeno težavnostno stopnjo
    private (int, int, float) GetDifficultySettings(int difficulty)
    {
        switch (difficulty)
        {
            case 1:
                return (5, 5, 0.7f); // Težavnost 1: mreža 5x5, gostota polj 70 %
            case 2:
                return (10, 10, 0.6f); // Težavnost 2: mreža 10x10, gostota polj 60 %
            case 3:
                return (15, 15, 0.5f); // Težavnost 3: mreža 15x15, gostota polj 50 %
            case 4:
                return (20, 20, 0.5f); // Težavnost 4: mreža 20x20, gostota polj 50 %
            case 5:
                return (25, 25, 0.4f); // Težavnost 5: mreža 25x25, gostota polj 40 %
            default:
                return (5, 5, 0.7f); // Privzeto
        }
    }

    // Ustvari naključen nonogram glede na izbrano velikost mreže
    private int[,] GeneratePuzzle(int rows, int columns, float fillRate)
    {
        int[,] puzzle = new int[columns, rows]; // Inicializacija prazne mreže
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                // Zapolni polje glede na verjetnost (fillRate) - če je manjša od fillRate, postavi črno (1), drugače belo (0)
                puzzle[col, row] = Random.Range(0f, 1f) < fillRate ? 1 : 0;
            }
        }
        return puzzle;
    }
}