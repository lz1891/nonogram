using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Upravljanje mreže nonograma in logike igre

public class PuzzleGrid : MonoBehaviour
{
    private int rows, columns; // Število vrstic in stolpcev v mreži
    public int[,] currentGrid; // Trenutno stanje mreže
    public int[,] solutionGrid; // Rešitev nonograma
    public static GridSquare selectedSquare = null; // Izbrano polje

    private int mouseAction = -1; // Trenutno dejanje miške (-1 pomeni brez dejanja)
    private int actionFlag = 0; // Flag za spremljanje dejanj

    public bool isPlaying = false; // Indikator, ali je igra aktivna
    private GridSquare[,] gridSquares; // Mreža polj (2D tabela)
    public Transform gridContainer; // Parent vseh polj v mreži
    public GameObject gridSquarePrefab; // Prefab za posamezna polja
    public GameObject hintTextPrefab; // Prefab za namige

    [HideInInspector]
    public int squareSize = 100; // Velikost polja
    [HideInInspector]
    public int fontSize = 50; // Velikost pisave za namige

    public GameObject pausePanel, winPanel; // Okno za pavzo in okno za zmago
    public Text timerText; // Prikaz časa igre
    public Text timeTakenText; // Prikaz porabljenega časa ob zmagi

    private List<Text> rowHints, columnHints; // Seznam namigov za vrstice in stolpce
    private float elapsedTime = 0; // Čas, ki je minil med igranjem

    // Inicializacija mreže glede na podane vrstice, stolpce in rešitev
    public void InitializeGrid(int rowCount, int colCount, int[,] puzzle)
    {
        rows = rowCount;
        columns = colCount;
        solutionGrid = puzzle; // Dodeli rešitev nonograma

        // Preračunaj velikost polj in pisave glede na število vrstic
        squareSize = (int)(1000 / rowCount / 1.6f);
        fontSize = squareSize / 2;
        if (fontSize < 25) fontSize = 25;

        currentGrid = new int[columns, rows]; // Inicializacija mreže

        GenerateGridSquares(); // Ustvari mrežo polj
        GenerateHints(); // Ustvari namige za vrstice in stolpce

        // Postavi mrežo na sredino zaslona
        gridContainer.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-squareSize * rows / 2, -squareSize * columns / 1.4f);
    }

    // Generiranje posameznih polj v mreži
    private void GenerateGridSquares()
    {
        gridSquares = new GridSquare[columns, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Ustvari novo polje v mreži in nastavi pozicijo v mreži
                GameObject squareObj = Instantiate(gridSquarePrefab, gridContainer);
                squareObj.name = $"Square ({i}, {j})";
                
                GridSquare square = squareObj.GetComponent<GridSquare>();
                square.SetPosition(i, j);
                Vector2 position = new Vector2(squareSize / 2 + squareSize * j, (rows - i) * squareSize - squareSize / 2);
                square.GetComponent<RectTransform>().anchoredPosition = position;
                gridSquares[i, j] = square;

                SetSquare(square, 0); // Inicializacija belega polja
                square.SetSize(squareSize);
            }
        }
    }

    //Generiranje namigov za vrstice in stolpce
    private void GenerateHints()
    {
        // Generiranje namigov za stolpce (glede na rešitev)
        columnHints = new List<Text>();
        for (int i = 0; i < columns; i++)
        {
            List<int> result = ReadPuzzle(solutionGrid, true, i);
            for(int a = 0; a < result.Count; a++)
            {
                GameObject squareObj = Instantiate(hintTextPrefab, gridContainer); // Ustvari novo polje za prikaz namiga
                squareObj.GetComponent<RectTransform>().sizeDelta = new Vector2(squareSize, squareSize);
                Vector2 position = new Vector2(i * squareSize + squareSize / 2, (rows) * squareSize + (result.Count - a) * squareSize - squareSize / 2);
                squareObj.GetComponent<RectTransform>().anchoredPosition = position;

                Text t = squareObj.GetComponentInChildren<Text>();
                t.text = result[a] + "";
                t.fontSize = fontSize;
                columnHints.Add(t);
            }
        }

        // Generiranje namigov za vrstice (glede na rešitev)
        rowHints = new List<Text>();
        for (int i = 0; i < rows; i++)
        {
            List<int> result = ReadPuzzle(solutionGrid, false, i);
            for (int a = 0; a < result.Count; a++)
            {
                GameObject squareObj = Instantiate(hintTextPrefab, gridContainer); // Ustvari novo polje za prikaz namiga
                squareObj.GetComponent<RectTransform>().sizeDelta = new Vector2(squareSize, squareSize);
                Vector2 position = new Vector2(-(result.Count - a) * squareSize + squareSize / 2, (rows - i) * squareSize - squareSize / 2);
                squareObj.GetComponent<RectTransform>().anchoredPosition = position;

                Text t = squareObj.GetComponentInChildren<Text>();
                t.text = result[a] + "";
                t.fontSize = fontSize;
                rowHints.Add(t);
            }
        }
    }

    // Posodobitev časa in obdelava vnosa med igranjem
    private void Update()
    {
        if (isPlaying)
        {
            // Posodobi čas in prikaži v obliki minute:sekunde
            elapsedTime += Time.deltaTime; // Posodobi čas
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            HandleInput(); // Obdelaj vnos 
        }
    }

    // Obdelava vnosa igralca (kliki)
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Levi klik
        {
            mouseAction = 0; // Nastavi dejanje na levi klik
            SetActionFlag(); // Nastavi stanje glede na trenutno izbrano polje
        }
        else if (Input.GetMouseButtonDown(1)) // Desni klik
        {
            mouseAction = 1; // Nastavi dejanje na desni klik
            SetActionFlag(); // Nastavi stanje glede na trenutno izbrano polje
        }

        if (selectedSquare != null)
        {
            if (Input.GetMouseButton(0) && mouseAction == 0) // Levi klik
            {
                ApplyAction(selectedSquare);
            }
            else if (Input.GetMouseButton(1) && mouseAction == 1) // Desni klik
            {
                ApplyAction(selectedSquare);
            }
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            mouseAction = -1; // Ponastavi dejanje miške
            actionFlag = 0; // Ponastavi flag dejanja
        }
    }

    // Nastavi dejanje glede na trenutno stanje polja
    private void SetActionFlag()
    {
        if (selectedSquare == null) return;

        if (mouseAction == 0) // Levi klik za barvanje polja
        {
            if (selectedSquare.state == 0)
            {
                actionFlag = 1; // Nastavi na črno
            }
            else if (selectedSquare.state == 1)
            {
                actionFlag = 0; // Ponastavi na belo
            }
        }
        else if (mouseAction == 1) // Desni klik za označitev "X"
        {
            if (selectedSquare.state == 0)
            {
                actionFlag = 2; // Označi z "X"
            }
            else if (selectedSquare.state == 2)
            {
                actionFlag = 0; // Ponastavi na belo
            }
        }
    }

    // Uporabi dejanje na izbrano polje
    private void ApplyAction(GridSquare square)
    {
        if (actionFlag == 1)
        {
            SetSquare(square, 1); // Napolni s črno
        }
        else if (actionFlag == 2)
        {
            SetSquare(square, 2); // Označi z "X"
        }
        else if (actionFlag == 0) 
        {
            SetSquare(square, 0); // Ponastavi na belo
        }
    }

    // Nastavi stanje izbranega polja
    private void SetSquare(GridSquare square, int state)
    {
        square.SetState(state); // Nastavi stanje polja
        currentGrid[square.x, square.y] = state; // Posodobi stanje mreže

        if (CheckWinCondition()) // Preveri, če je igralec zmagal
        {
            isPlaying = false;
            winPanel.SetActive(true);
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            timeTakenText.text = $"Nonogram si rešil v {minutes:D2}:{seconds:D2}.";
        }
    }

    // Preveri zmago z ujemanjem mreže z rešitvijo
    private bool CheckWinCondition()
    {
        for (int i = 0; i < rows; i++)
        {
            if (!IntListCompare(ReadPuzzle(solutionGrid, false, i), ReadPuzzle(currentGrid, false, i))) return false;
        }
        for (int i = 0; i < columns; i++)
        {
            if (!IntListCompare(ReadPuzzle(solutionGrid, true, i), ReadPuzzle(currentGrid, true, i))) return false;

        }
        return true; // Igralec je pravilno rešil nonogram
    }

    // Primerjava seznamov za preverjanje ujemanja
    public static bool IntListCompare(List<int> a, List<int> b)
    {
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    // Zmaga: končaj igro in prikaži okno za zmago
    public void Win()
    {
        isPlaying = false;

        winPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    // Premor: ustavi čas in prikaži okno za premor
    public void Pause()
    {
        isPlaying = false;
        pausePanel.SetActive(true);
    }

    // Nadaljuj igro: skrij okno za premor in nadaljuj čas
    public void Resume()
    {
        isPlaying = true;
        pausePanel.SetActive(false);
    }
    
    // Ponastavi igro: ponastavi čas, mrežo in odstrani okni za premor in zmago
    public void Restart()
    {
        elapsedTime = 0f; // Ponastavi čas igre
        isPlaying = true;

        // Ponastavi mrežo z začetnimi vrednostmi
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                SetSquare(gridSquares[i, j], 0); // Ponastavi vsako polje na začetno stanje
            }
        }

        pausePanel.SetActive(false); // Skrij okno za premor
        winPanel.SetActive(false); // Skrij zmagovalno okno
    }

    // Izhod iz igre na glavni meni
    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Preberi namige za vrstico ali stolpec glede na rešitev
    public List<int> ReadPuzzle(int[,] solutionGrid, bool readColumn, int index)
    {
        List<int> result = new List<int>();
        if (readColumn)
        {
            bool flag = false;
            int count = 0;
            for (int r = 0; r < rows; r++)
            {
                if (flag)
                {
                    if (solutionGrid[r, index] == 1)
                    {
                        count++;
                    }
                    else
                    {
                        result.Add(count);
                        flag = false;
                        count = 0;
                    }
                }
                else
                {
                    if (solutionGrid[r, index] == 1)
                    {
                        count = 1;
                        flag = true;
                    }
                }
            }
            if (flag == true)
            {
                result.Add(count);
            }
        }
        else
        {
            bool flag = false;
            int count = 0;
            for (int c = 0; c < columns; c++)
            {
                if (flag)
                {
                    if (solutionGrid[index, c] == 1)
                    {
                        count++;
                    }
                    else
                    {
                        result.Add(count);
                        flag = false;
                        count = 0;
                    }
                }
                else
                {
                    if (solutionGrid[index, c] == 1)
                    {
                        count = 1;
                        flag = true;
                    }
                }
            }
            if (flag == true)
            {
                result.Add(count);
            }
        }

        if (result.Count == 0) result.Add(0); // Če ni najdenih namigov, dodaj 0
        return result;
    }
}
