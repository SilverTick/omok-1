//  Made by Joyce Quach

using System;

/* ***************************************************** */
/* *                 Implementation                    * */
/* ***************************************************** */

namespace BoardGame
{
    public class Board
    {

        private int k;           // number of stones to win
        private bool p1;         // player 1
        private bool p2;         // player 2
        private bool w1;         // did player 1 win?
        private bool w2;         // did player 2 win?
        private bool done;       // ends the game
        private int[,] board;    // contains player 1 and 2's stones

        /* Initializes an M by N board that requires k stones in a row to win. 
           Returns a Board object. */
        public Board(int M, int N, int k)
        {
            this.k = k;
            this.p1 = true;
            this.p2 = false;
            this.w1 = false;
            this.w2 = false;
            this.done = false;
            this.board = new int[M, N];
        }

        /* Switches between players. */
        public bool Player(bool current)
        {
            if (current == this.p1)
                return this.p2;
            return this.p1;
        }

        /* Checks for a game over. */
        public bool GameOver()
        {
            return this.done || this.IsFull();
        }

        /* Check if players 1 and 2 won. */
        public bool P1Win() { return this.w1; }
        public bool P2Win() { return this.w2; }

        /* Marks a player's stone on the board. 
           Return values specify whether a player 
           successfully marked a position. */
        public bool Mark(bool player, int x, int y)
        {
            int M = this.board.GetLength(0);
            int N = this.board.GetLength(1);

            //  Invalid position
            if (x < 0 || x >= M || y < 0 || y >= N)
            {
                Console.WriteLine("Invalid position.");
                return false;
            }

            //  Can't mark any more positions
            else if (this.IsFull())
            {
                this.done = true;
                return false;
            }

            //  Stone was already marked by a player
            else if (this.IsMarked(x, y)) { return false; }

            //  Stone was not already marked by someone and check if they won
            else if (player == this.p1)
            {
                this.board[x, y] = 1;
                this.w1 = this.Victory(this.p1, x, y);
            }
            else
            {
                this.board[x, y] = 2;
                this.w2 = this.Victory(this.p2, x, y);
            }

            //  Check if the board wasn't made full after marking a stone
            return true;
        }

        /* Checks if a stone was already marked by someone.  */
        public bool IsMarked(int x, int y)
        {
            int M = this.board.GetLength(0);
            int N = this.board.GetLength(1);

            //  Check if (x, y) is valid
            if (x < 0 || x >= M || y < 0 || y >= N) { return false; }

            //  Did anyone mark this stone?
            // 0 : empty, 1 : player 1, 2 : player 2
            return this.board[x, y] != 0;
        }

        /* Checks if the Board can be filled or not. */
        private bool IsFull()
        {
            int M = this.board.GetLength(0);
            int N = this.board.GetLength(1);

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    //  One of the players marked (i, j)
                    if (this.board[i, j] != 0) { continue; }

                    //  No one marked (i, j)
                    else { return false; }
                }

            }

            //  Players marked every possible (i, j)
            return true;
        }

        /* Checks if someone has won based off of stone placement. 
           Are adjacent stones marked based on a 5-stone radius? */
        private bool Victory(bool player, int x, int y)
        {
            /* Traverse the array backwards as much as possible, 
               then go forwards to find the k-in-a-row. If you don't 
               find the k-in-a-row, then either reset parameters 
               or return false. */

            int M = this.board.GetLength(0);
            int N = this.board.GetLength(1);

            //  Checks if player 1 has a k-in-a-row streak at the marked spot
            if (player == this.p1)
            {
                this.w1 =
                    this.Traverse(this.p1, x, y, -1, -1, -1, N) ||    // Horizontal case
                    this.Traverse(this.p1, x, y, -1, M, -1, -1) ||    // Vertical case
                    this.Traverse(this.p1, x, y, -1, M, -1, N) ||    // Left diagonal case
                    this.Traverse(this.p1, x, y, -1, M, N, -1);      // Right diagonal case
                return this.w1;
            }

            //  Checks if player 2 has a k-in-a-row streak at the marked spot

            this.w2 =
                    this.Traverse(this.p2, x, y, -1, -1, -1, N) ||    // Horizontal case
                    this.Traverse(this.p2, x, y, -1, M, -1, -1) ||    // Vertical case
                    this.Traverse(this.p2, x, y, -1, M, -1, N) ||    // Left diagonal case
                    this.Traverse(this.p2, x, y, -1, M, N, -1);      // Right diagonal case
            return this.w2;
        }

        /* Traverses the array in a specified direction for a specific player. */
        private bool Traverse(
            bool player,
            int x,
            int y,
            int x_start,
            int x_stop,
            int y_start,
            int y_stop
            )
        {

            //  Initialization step
            int count = 0;
            int i = x;
            int j = y;

            for (
                ; // Go back as much as possible
                (i > x_start || i != x_start) &&
                (j > y_start || j != y_start) &&
                this.IsMarked(i, j)
                ;
                )
            {

                if (x_start == x_stop) { j--; }        // Horizontal case
                else if (y_start == y_stop) { i--; }        // Vertical case
                else if (x_start == y_start) { i--; j--; }   // Left diagonal case
                else { i--; j++; }   // Right diagonal case
            }

            /* i and j are already out of bounds, so increment them to let the next for loop execute */
            if (x_start == x_stop) { j++; }        // Horizontal case
            else if (y_start == y_stop) { i++; }        // Vertical case
            else if (x_start == y_start) { i++; j++; }   // Left diagonal case
            else { i++; j--; }   // Right diagonal case

            for (
                ; // Go forward as much as possible
                (i < x_stop || i != x_stop) &&
                (j < y_stop || j != y_stop) &&
                this.IsMarked(i, j)
                ; count++
                )
            {

                if (x_start == x_stop) { j++; }        // Horizontal case
                else if (y_start == y_stop) { i++; }        // Vertical case
                else if (x_start == y_start) { i++; j++; }   // Left diagonal case
                else { i++; j--; }   // Right diagonal case
            }

            // Did the player get at least k-in-a-row?
            if (count < this.k) { return false; }

            // One of the players won
            this.done = true;

            return true;
        }

        /* Unmarks a player's stone on the board. 
           Return values specify whether a player 
           successfully marked a position. */
        public void Unmark(int x, int y) {
            this.board[x, y] = 0;
            this.w1 = false;
            this.w2 = false;
            this.done = false;
        }
    }
}