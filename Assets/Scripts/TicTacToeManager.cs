using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeManager : MonoBehaviour
{
    
    public Button[] buttons = new Button[9];
    public TextMeshProUGUI statusText;
    public GameObject gameOverPanel;
    public GameObject minigameCanvas;

    private char[] board = new char[9];
    private bool isPlayerTurn = true;
    private bool gameOver = false;
    private int movesMade = 0;

    private bool isAwaitingAIMove = false;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (isAwaitingAIMove)
        {
            isAwaitingAIMove = false;
            AITurn();
        }
    }
    private void InitializeGame()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = ' ';
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            buttons[i].interactable = true;

            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
        isPlayerTurn = true;
        gameOver = false;
        movesMade = 0;
        isAwaitingAIMove = false;
        gameOverPanel.SetActive(false);
        UpdateStatusText("Your turn (X)");
    }
    private void UpdateStatusText(string message)
    {
        statusText.text = message;
    }

    public void OnButtonClick(int index)
    {
        if (board[index] != ' ' || gameOver || !isPlayerTurn)
        {
            return;
        }

        MakeMove(index, 'X');
        isPlayerTurn = false;
        if (!gameOver)
        {
            UpdateStatusText("AI's turn (O)...");
            isAwaitingAIMove = true;
        }
    }

    private void MakeMove(int index, char player)
    {
        board[index] = player;
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = player.ToString();
        buttons[index].interactable = false;
        movesMade++;

        if (CheckWin(player))
        {
            GameOver(player.ToString() + " wins!");
        }
        else if (movesMade == 9)
        {
            GameOver("It's a draw!");
        }
    }

    private void AITurn()
    {
        if (gameOver) return;

        int bestMove = GetBestMove();

        if (bestMove != -1)
        {
            MakeMove(bestMove, 'O');
        }

        if (!gameOver)
        {
            isPlayerTurn = true;
            UpdateStatusText("Your turn (X)");
        }
    }

    private int GetBestMove()
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                board[i] = 'O';
                if (CheckWin('O'))
                {
                    board[i] = ' '; return i;
                }
                board[i] = ' ';
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                board[i] = 'X';
                if (CheckWin('X'))
                {
                    board[i] = ' '; return i;
                }
                board[i] = ' ';
            }
        }

        if (board[4] == ' ') return 4;
        int[] corners = { 0, 2, 6, 8 };
        foreach (int corner in corners)
        {
            if (board[corner] == ' ') return corner;
        }
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                return i;
            }
        }
        return -1;
    }
    private bool CheckWin(char player)
    {
        int[,] winningLines = new int[,]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < 8; i++)
        {
            if (board[winningLines[i, 0]] == player &&
                board[winningLines[i, 1]] == player &&
                board[winningLines[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    private void GameOver(string message)
    {
        gameOver = true;
        UpdateStatusText("Game Over: " + message);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        gameOverPanel.SetActive(true);
    }
    public void PlayAgain()
    {
        InitializeGame();
    }
    public void CloseGame()
    {
        minigameCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}