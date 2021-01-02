using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuGame
    {
        public int done = (1 << 9) - 1; //helper for flag
        public int[] ROW, COL, BOX; //flag
        public int[,] arr, solution; //sudoku
        public List<int[]> emptyCell;
        public int K;

        public SudokuGame()
        {
            ROW = new int[9];
            COL = new int[9];
            BOX = new int[9];
            arr = new int[9, 9];
            solution = new int[9, 9];
            K = 20;
        }

        public SudokuGame(int K)
        {
            this.K = K;
            ROW = new int[9];
            COL = new int[9];
            BOX = new int[9];
            arr = new int[9, 9];
            solution = new int[9, 9];
        }

        public int boxIndex(int r, int c)
        {
            return (r / 3 * 3) + (c / 3);
        }

        public void generate()
        {
            emptyCell = new List<int[]>();

            //other than box 0, 4, 8
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    int b = boxIndex(i, j);
                    if (b != 0 && b != 4 && b != 8)
                        emptyCell.Add(new int[] { i, j });
                }
            }

            for (int i = 0; i < 9; ++i)
                ROW[i] = COL[i] = BOX[i] = 0;

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    arr[i, j] = 0;
                    solution[i, j] = 0;
                }
            }

            fillDiagonal();
            solve(0);
            removeNumber(K);
        }

        public void setFlag(int i, int j, bool set)
        {
            if(set)
            {
                ROW[i] |= (1 << (solution[i, j] - 1));              //row r has x
                COL[j] |= (1 << (solution[i, j] - 1));              //col c has x
                BOX[boxIndex(i, j)] |= (1 << (solution[i, j] - 1)); //box b has x
            }
            else
            {
                ROW[i] &= ~(1 << (solution[i, j] - 1));             //remove x from row r
                COL[j] &= ~(1 << (solution[i, j] - 1));             //remove x from col c
                BOX[boxIndex(i, j)] &= ~(1 << (solution[i, j] - 1));//remove x from box b
            }
        }

        public void fillDiagonal()
        {
            for(int k = 0; k < 9; k += 3)
            {
                //randomly generate 9 numbers
                Random r = new Random();
                int[] gen = { 1, 2, 3, 4, 5, 6, 7, 8, 9};
                for(int i = 8; i >= 0; --i)
                {
                    int j = r.Next(0, i + 1);

                    int temp = gen[i];
                    gen[i] = gen[j];
                    gen[j] = temp;
                }

                for(int i = 0; i < 3; ++i)
                {
                    for(int j = 0; j < 3; ++j)
                    {
                        //randomly fill solution[i+k][j+k]
                        solution[i + k, j + k] = gen[i * 3 + j];
                        setFlag(i + k, j + k, true);
                    }
                }
            }
        }

        public bool solve(int index)
        {
            if (index == emptyCell.Count) return true;
            int r = emptyCell[index][0];
            int c = emptyCell[index][1];
            int b = boxIndex(r, c);

            int row = ROW[r];
            int col = COL[c];
            int box = BOX[b];

            int remainingNumber = done & ~(row | col | box);
            while (remainingNumber > 0) //try remaining number
            {
                int num = remainingNumber & -remainingNumber; //get last 1 bit
                remainingNumber -= num;

                //test number
                solution[r, c] = (int)Math.Log(num, 2) + 1;
                ROW[r] |= num;
                COL[c] |= num;
                BOX[b] |= num;

                if (solve(index + 1)) return true;

                //reset number
                ROW[r] = row;
                COL[c] = col;
                BOX[b] = box;
            }
            
            return false;
        }

        public void removeNumber(int K)
        {
            emptyCell = new List<int[]>();

            for (int i = 0; i < 9; ++i)
                for(int j = 0; j < 9; ++j)
                    arr[i, j] = solution[i, j];

            int[,] gen = new int[81, 2];
            for(int i = 0; i < 81; ++i)
            {
                gen[i, 0] = i/9;
                gen[i, 1] = i%9;
            }

            Random r = new Random();
            for(int i = 80; i >= 0; --i)
            {
                int j = r.Next(0, i + 1);

                int tempA = gen[i, 0];
                gen[i, 0] = gen[j, 0];
                gen[j, 0] = tempA;

                int tempB = gen[i, 1];
                gen[i, 1] = gen[j, 1];
                gen[j, 1] = tempB;
            }

            for(int index = 0; index < K; ++index)
            {
                int i = gen[index, 0];
                int j = gen[index, 1];
                arr[i, j] = 0;
                emptyCell.Add(new int[] { i, j });
                setFlag(i, j, false);
            }
        }
    }
}

