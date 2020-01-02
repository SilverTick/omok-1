//  Made by Joyce Quach

using System;

/* ***************************************************** */
/* *                 Implementation                    * */
/* ***************************************************** */

namespace BoardGame
{
    public class Board
    {

        private int k;       // number of stones to win
        private bool p1;      // player 1
        private bool p2;      // player 2
        private bool w1;      // did player 1 win?
        private bool w2;      // did player 2 win?
        private bool done;    // ends the game
        private bool[,] m1;      // contains player 1's stones
        private bool[,] m2;      // contains player 2's stones

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
            this.m1 = new bool[M, N];
            this.m2 = new bool[M, N];
        }

        /* Switches between players. */
        public bool player(bool current)
        {
            if (current == this.p1)
                return this.p2;
            return this.p1;
        }

        /* Checks for a game over. */
        public bool gameOver()
        {
            return this.done || this.isFull();
        }

        /* Check if players 1 and 2 won. */
        public bool p1Win() { return this.w1; }
        public bool p2Win() { return this.w2; }

        /* Marks a player's stone on the board. 
           Return values specify whether a player 
           successfully marked a position. */
        public bool mark(bool player, int x, int y)
        {
            int M = this.m1.GetLength(0);
            int N = this.m1.GetLength(1);

            //  Invalid position
            if (x < 0 || x >= M || y < 0 || y >= N)
            {
                Console.WriteLine("Invalid position.");
                return false;
            }

            //  Can't mark any more positions
            else if (this.isFull())
            {
                this.done = true;
                return false;
            }

            //  Stone was already marked by a player
            else if (
                this.isMarked(this.p1, x, y) || // Marked by player 1
                this.isMarked(this.p2, x, y)    // Marked by player 2
                ) { return false; }

            //  Stone was not already marked by someone and check if they won
            else if (player == this.p1)
            {
                this.m1[x, y] = true;
                this.w1 = this.victory(this.p1, x, y);
            }
            else
            {
                this.m2[x, y] = true;
                this.w2 = this.victory(this.p2, x, y);
            }

            //  Check if the board wasn't made full after marking a stone
            return true;
        }

        /* Checks if a stone was already marked by someone.  */
        private bool isMarked(bool player, int x, int y)
        {
            int M = this.m1.GetLength(0);
            int N = this.m1.GetLength(1);

            //  Check if (x, y) is valid
            if (x < 0 || x >= M || y < 0 || y >= N) { return false; }

            //  Did player 1 mark this stone?
            if (player == this.p1) { return this.m1[x, y]; }

            //  Did player 2 mark this stone?
            return this.m2[x, y];
        }

        /* Checks if the Board can be filled or not. */
        private bool isFull()
        {
            int M = this.m1.GetLength(0);
            int N = this.m1.GetLength(1);

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    //  One of the players marked (i, j)
                    if (this.m1[i, j] || this.m2[i, j]) { continue; }

                    //  No one marked (i, j)
                    return false;
                }

            }

            //  Players marked every possible (i, j)
            return true;
        }

        /* Checks if someone has won based off of stone placement. 
           Are adjacent stones marked based on a 5-stone radius? */
        private bool victory(bool player, int x, int y)
        {
            /* Traverse the array backwards as much as possible, 
               then go forwards to find the k-in-a-row. If you don't 
               find the k-in-a-row, then either reset parameters 
               or return false. */

            int M = this.m1.GetLength(0);
            int N = this.m1.GetLength(1);

            //  Checks if player 1 has a k-in-a-row streak at the marked spot
            if (player == this.p1)
            {
                this.w1 =
                    this.traverse(this.p1, x, y, -1, -1, -1, N) ||    // Horizontal case
                    this.traverse(this.p1, x, y, -1, M, -1, -1) ||    // Vertical case
                    this.traverse(this.p1, x, y, -1, M, -1, N) ||    // Left diagonal case
                    this.traverse(this.p1, x, y, -1, M, N, -1);      // Right diagonal case
                return this.w1;
            }

            //  Checks if player 2 has a k-in-a-row streak at the marked spot

            this.w2 =
                    this.traverse(this.p2, x, y, -1, -1, -1, N) ||    // Horizontal case
                    this.traverse(this.p2, x, y, -1, M, -1, -1) ||    // Vertical case
                    this.traverse(this.p2, x, y, -1, M, -1, N) ||    // Left diagonal case
                    this.traverse(this.p2, x, y, -1, M, N, -1);      // Right diagonal case
            return this.w2;
        }

        /* Traverses the array in a specified direction for a specific player. */
        private bool traverse(
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
                this.isMarked(player, i, j)
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
                this.isMarked(player, i, j)
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

    }
}