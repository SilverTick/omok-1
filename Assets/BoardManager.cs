using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;
using TMPro;

public class BoardManager : MonoBehaviour
{
    // Instance variables
    public const bool p1 = true;
    public const bool p2 = false;
    public       bool current;
    public GameObject p1_piece;
    public GameObject p2_piece;
    public GameObject piece_parent;
    public TextMeshProUGUI text;

    private Board  board;
    private Camera cam; 

    // Mark a position on the board provided that a player hasn't already marked it.
    public void PlacePiece(bool player, int x, int y) {
        Debug.Log((x, y));

        /* offset between internal and external boards is 7
           BoardGame Board origin (0, 0) begins at top-left
           Unity board origin (0, 0) begins at middle center */
        if (board.mark(player, x + 7, y + 7)) {
            switch (player) {
                case p1:
                    /* set the position of p1_piece to be at position (x, y)
                       var keyword => not strict typing, like Python
                       right now, var => value of a GameObject
                       .transform => class that manipulates position, scale for parents, children
                       Instantiate() => second argument will become the parent of the first argument */
                    var g1 = Instantiate(p1_piece, piece_parent.transform);
                    g1.transform.position = new Vector2(x, y);
                    break;
                case p2:
                    var g2 = Instantiate(p2_piece, piece_parent.transform);
                    g2.transform.position = new Vector2(x, y);
                    break;
            }

            // Swap players
            current = !current;
        }
        else
        {
            Debug.LogError("Cannot mark there, player " + (Convert.ToInt32(player) + 1) + ".");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // create something to place things on a board
        // board will always be the same size
        cam = Camera.main;
        board = new Board(15, 15, 5);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "It is player " + (Convert.ToInt32(current) + 1) + "'s turn.";

        // check if the game is over
        if (board.gameOver())
        {
            if (board.p1Win())
                text.text = "Player 1 won!";
            else if (board.p2Win())
                text.text = "Player 2 won!";
            else
                text.text = "Game is tied";

            // prevents the game from further updating, thus rejecting mouse input
            return;
        }

        // get the left mouse click and its position
        if (Input.GetMouseButtonDown(0)) {

            // world position of the cursor, round coords to ints
            Vector2 v = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int v_int = Vector2Int.RoundToInt(v);

            // get the .x and .y attributes 
            PlacePiece(current, v_int.x, v_int.y);
            text.text = "It is player " + (Convert.ToInt32(current) + 1) + "'s turn.";
        }
    }
}
