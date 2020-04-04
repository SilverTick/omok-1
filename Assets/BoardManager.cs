using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;    // tools to manage the scene, for restarting the game
using BoardGame;                      // namespace from Board.cs
using TMPro;                          // for modifying text on Canvas


public class BoardManager : MonoBehaviour
{
    // Instance variables
    public const bool      p1 = true;
    public const bool      p2 = false;
    public       bool      current;
    public GameObject      p1_piece;
    public GameObject      p2_piece;
    public GameObject      piece_parent;
    public TextMeshProUGUI text;

    // Assume board is 15x15
    private const int M = 15;
    private const int N = 15;

    private Board  board;
    private Camera cam;

    /* Scene 0 */
    public void Menu() {
        SceneManager.LoadScene(0);
    }

    /* Scene 1 */
    // Mark a position on the board provided that a player hasn't already marked it.
    public void PlacePiece(bool player, int x, int y) {
        /* offset between internal and external boards is 7
           BoardGame Board origin (0, 0) begins at top-left
           Unity board origin (0, 0) begins at middle center */

        Debug.Log("Whose turn is it: " + (current ? 1 : 2));

        if (board.Mark(player, x + 7, y + 7)) {
            switch (player) {
                // Human
                case p1:
                    Debug.Log("Player 1: " + (x, y));
                    /* set the position of p1_piece to be at position (x, y)
                        var keyword => not strict typing, like Python
                        right now, var => value of a GameObject
                        .transform => class that manipulates position, scale for parents, children
                        Instantiate() => second argument will become the parent of the first argument */
                    var g1 = Instantiate(p1_piece, piece_parent.transform);
                    g1.transform.position = new Vector2(x, y);
                    break;

                // AI
                case p2:
                    Debug.Log("Player 2: " + (x, y));
                    var g2 = Instantiate(p2_piece, piece_parent.transform);
                    g2.transform.position = new Vector2(x, y);
                    break;
            }

            // Swap players
            current = !current;

        }

        else {
            Debug.LogError("Cannot mark there, player " + (Convert.ToInt32(player) + 1) + ".");
        }

    }

    // For AI to pick move
    private int Minimax(int depth, int alpha, int beta, bool isMaximizing) {
        // First check, did anyone win?
        if (depth == 100 || board.GameOver()) { 
            if (board.P1Win()) { return +10; }
            else if (board.P2Win()) { return -10; }
            else { return 0; }
        }

        // Player 2's turn
        if (isMaximizing) {
            int maxEval = -10000;
            // Check all possible spots again
            for (int i = 0; i < M; i++) {
                for (int j = 0; j < N; j++) {
                    // Is the spot available?
                    if (board.IsMarked(i, j)) {
                        // Simulate player 2's marking
                        board.Mark(p2, i, j);

                        // Get score, is MAX's turn next
                        int eval = this.Minimax(depth + 1, alpha, beta, false);

                        // Unmark
                        board.Unmark(i, j);

                        // Alpha-beta pruning
                        maxEval = Math.Max(eval, maxEval);
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha) { break; }
                    }
                }
            }
            return maxEval;
        }
        // Player 1's turn
        else {
            int minEval = 10000;
            // Check all possible spots again
            for (int i = 0; i < M; i++) {
                for (int j = 0; j < N; j++) {
                    // Is the spot available?
                    if (board.IsMarked(i, j)) {
                        // Simulate player 1's marking
                        board.Mark(p1, i, j);

                        // Get score, is MIN's turn next
                        int eval = this.Minimax(depth + 1, alpha, beta, true);

                        // Unmark
                        board.Unmark(i, j);

                        // Alpha-beta pruning
                        minEval = Math.Min(eval, minEval);
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha) { break; }
                    }
                }
            }
            return minEval;
        }
    }

    // AI's turn
    public void BestMove() {
        // Represents negative infinity
        int bestScore = -10000;
        Move bestMove = new Move(0, 0);

        for (int i = 0; i < M; i++) {
            for (int j = 0; j < N; j++) {
                // Is the spot available? Then have the AI mark it.
                if (!board.IsMarked(i, j)) {
                    board.Mark(p2, i, j);

                    // Get the score for this spot, then simulate Player 1
                    // int score = this.Minimax(0, 10000, -10000, false);
                    int score = this.Minimax(0, -10000, 10000, false);

                    // Unmark the board
                    board.Unmark(i, j);

                    // Update score and best move
                    if (score > bestScore) {
                        bestScore = score;
                        bestMove = new Move(i, j);
                    }
                }
            }
        }

        // actually mark the board at the end
        // PlacePiece(p2, bestMove.x, bestMove.y);
        // to account for offset on Unity Board
        PlacePiece(p2, bestMove.x - 7, bestMove.y - 7);
        // board.Mark(p2, bestMove.x, bestMove.y);
    }

    public void RestartGame() {
        SceneManager.LoadScene(1);
    }

    /* Mono-behaviors, event functions used by Unity for first frame and ongoing frames. */
    // Start is called before the first frame update
    void Start()
    {
        // start with Player 1
        current = p1;

        // see that selection from BoardSelector is accessible in BoardManager
        Debug.Log(PlayerPrefs.GetString("selection"));

        // make this more elegant by doing regex later
        switch (PlayerPrefs.GetString("selection"))
        {
            case "slime_and_pig":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Slime");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Pig");
                break;
            case "slime_and_octopus":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Slime");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Octopus");
                break;
            case "slime_and_mushroom":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Slime");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Mushroom");
                break;
            case "teddy_and_trixter":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Pink Teddy");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Trixter");
                break;
            case "pig_and_octopus":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Pig");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Octopus");
                break;
            case "pig_and_mushroom":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Pig");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Mushroom");
                break;
            case "panda_and_teddy":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Panda Teddy");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Pink Teddy");
                break;
            case "panda_and_trixter":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Panda Teddy");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Trixter");
                break;
            case "panda_and_blocktopus":
                p1_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Panda Teddy");
                p2_piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Blocktopus");
                break;
        }

        // create something to place things on a board
        // board will always be the same size
        cam = Camera.main;
        board = new Board(15, 15, 5);

    }

    // Update is called once per frame, 1/60th of a second
    void Update()
    {
        // Change player turn text
        switch (current)
        {
            case p1:
                text.text = "It is player 1's turn.";
                break;
            case p2:
                text.text = "It is player 2's turn.";
                break;
        }

        // check if the game is over
        if (board.GameOver())
        {
            if (board.P1Win())
                text.text = "Player 1 won!";
            else if (board.P2Win())
                text.text = "Player 2 won!";
            else
                text.text = "Game is tied";

            // prevents the game from further updating, thus rejecting mouse input
            return;
        }

        // get the left mouse click and its position
        if (Input.GetMouseButtonDown(0)) {

            // Check if the current player is human
            if (current == p1) {
                // world position of the cursor, round coords to ints
                Vector2 v = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int v_int = Vector2Int.RoundToInt(v);

                // get the .x and .y attributes 
                PlacePiece(current, v_int.x, v_int.y);
                text.text = "It is player " + (Convert.ToInt32(current) + 1) + "'s turn.";
            }

            // Delay for 2 seconds
            // Invoke("BestMove", 2);
            BestMove();


        }
    }

    // For AI to pick moves
    public class Move {
        public int x;
        public int y;
        public Move(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}
