using System.Collections.Generic;

/*  cube rep reference


    sticker rep - face indices
 
         ┌──┐
         │ 0│
      ┌──┼──┼──┬──┐
      │ 5│ 1│ 2│ 4│
      └──┼──┼──┴──┘
         │ 3│
         └──┘


    sticker rep - face sticker indices

      ┌──┬──┐
      │ 0│ 1│
      ├──┼──┤
      │ 2│ 3│
      └──┴──┘


    sticker rep - face colors

         ┌──┐
         │ 1│
      ┌──┼──┼──┬──┐
      │-3│ 2│ 3│-2│
      └──┼──┼──┴──┘
         │-1│
         └──┘


    OP rep - piece indices

           / \
          /   \
        / \ 0 / \
       /   \ /   \
      |\ 1 / \ 3 /|
      | \ /   \ / |
      |  |\ 2 /|  |
      |\ | \ / | /|
      | \|  |  |/ |
      |4 |\ | /| 6|
       \ | \|/ | /
        \|  |  |/
          \ 5 /
           \ /


    OP rep - orientation indices

    0 = none
    1 = ccw
    2 = cw

*/

namespace CubeStates
{
    class Cube222Fixed
    {
        int[][] stateOP;    // OP state representation
        int[] colUFR;       // UFR colors based on fixed DLB corner - for converting back to sticker rep

        #region constructors

        // default constructor - solved state
        public Cube222Fixed()
        {
            SetState(SolvedOP());
        }

        // constructor - given stickers state
        public Cube222Fixed(int[][] stateStickers)
        {
            SetStateFromStickers(stateStickers);
        }

        // constructor - given alg string
        public Cube222Fixed(string alg)
        {
            SetState(SolvedOP());
            DoAlg(alg);
        }

        #endregion

        #region private methods

        // convert stickers into primes relative to fixed corner
        int[][] StickersToPrimes(int[][] stateStickers)
        {
            int[][] statePrimes = new int[6][];
            for (int i = 0; i < 6; i++)
                statePrimes[i] = new int[4];

            // sticker primes
            Dictionary<int, int> stickerPrimes = new Dictionary<int, int>()
            {
                { colUFR[0]     ,   0 },
                { colUFR[1]     ,   1 },
                { colUFR[2]     ,   2 },
                { -colUFR[0]    ,   3 },
                { -colUFR[1]    ,   5 },
                { -colUFR[2]    ,   7 }
            };

            // create prime state
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 4; j++)
                    statePrimes[i][j] = stickerPrimes[stateStickers[i][j]];

            return statePrimes;
        }

        // convert primes state to OP representation
        int[][] PrimesToOP(int[][] statePrimes)
        {
            int[][] stateOP = new int[2][];
            for (int i = 0; i < 2; i++)
                stateOP[i] = new int[7];

            int[][] pieces = new int[7][];
            for (int i = 0; i < 7; i++)
                pieces[i] = new int[3];

            // prime sum piece IDs
            Dictionary<int, int> primeSumIDs = new Dictionary<int, int>()
            {
                { 12    ,   0 },
                { 8     ,   1 },
                { 3     ,   2 },
                { 7     ,   3 },
                { 11    ,   4 },
                { 6     ,   5 },
                { 10    ,   6 }
            };

            // separate into pieces
            pieces[0][0] = statePrimes[0][0]; // UBL
            pieces[0][1] = statePrimes[4][1];
            pieces[0][2] = statePrimes[5][0];

            pieces[1][0] = statePrimes[0][2]; // ULF
            pieces[1][1] = statePrimes[5][1];
            pieces[1][2] = statePrimes[1][0];

            pieces[2][0] = statePrimes[0][3]; // UFR
            pieces[2][1] = statePrimes[1][1];
            pieces[2][2] = statePrimes[2][0];

            pieces[3][0] = statePrimes[0][1]; // URB
            pieces[3][1] = statePrimes[2][1];
            pieces[3][2] = statePrimes[4][0];

            pieces[4][0] = statePrimes[3][0]; // DFL
            pieces[4][1] = statePrimes[1][2];
            pieces[4][2] = statePrimes[5][3];

            pieces[5][0] = statePrimes[3][1]; // DRF
            pieces[5][1] = statePrimes[2][2];
            pieces[5][2] = statePrimes[1][3];

            pieces[6][0] = statePrimes[3][3]; // DBR
            pieces[6][1] = statePrimes[4][2];
            pieces[6][2] = statePrimes[2][3];

            // create OP state
            int sum;
            int indexUD;
            for (int i = 0; i < 7; i++)
            {
                sum = 0;
                indexUD = 0;
                for( int j = 0; j < 3; j++)
                {
                    sum += pieces[i][j];
                    if (pieces[i][j] == 0 || pieces[i][j] == 3)
                        indexUD = j;
                }

                stateOP[0][i] = primeSumIDs[sum];
                stateOP[1][i] = indexUD;
            }

            return stateOP;
        }

        // create solved stickers state
        int[][] SolvedStickers()
        {
            int[][] stateStickers = new int[6][];
            for (int i = 0; i < 6; i++)
                stateStickers[i] = new int[4];

            // face colors
            Dictionary<int, int> faceCols = new Dictionary<int, int>()
            {
                { 0     ,   1 },
                { 1     ,   2 },
                { 2     ,   3 },
                { 3     ,   -1 },
                { 4     ,   -2 },
                { 5     ,   -3 },
            };

            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 4; j++)
                    stateStickers[i][j] = faceCols[i];

            return stateStickers;
        }

        // create solved OP state
        int[][] SolvedOP()
        {
            int[][] stateOP = new int[2][];
            for (int i = 0; i < 2; i++)
                stateOP[i] = new int[7];

            for( int i = 0; i < 7; i++ )
            {
                stateOP[0][i] = i;
                stateOP[1][i] = 0;
            }

            return stateOP;
        }

        // perform move on cube
        void TurnCube(int move)
        {
            switch (move)
            {
                case 0: // U
                    SwapPieces(0, 1);
                    SwapPieces(1, 2);
                    SwapPieces(2, 3);
                    break;
                case 1: // U'
                    SwapPieces(3, 2);
                    SwapPieces(2, 1);
                    SwapPieces(1, 0);
                    break;
                case 2: // U2
                    SwapPieces(0, 2);
                    SwapPieces(1, 3);
                    break;
                case 3: // F
                    SwapPieces(2, 1);
                    SwapPieces(1, 4);
                    SwapPieces(4, 5);

                    TwistPiece(2, 2);
                    TwistPiece(1, 1);
                    TwistPiece(4, 2);
                    TwistPiece(5, 1);
                    break;
                case 4: // F'
                    SwapPieces(5, 4);
                    SwapPieces(4, 1);
                    SwapPieces(1, 2);

                    TwistPiece(2, 2);
                    TwistPiece(1, 1);
                    TwistPiece(4, 2);
                    TwistPiece(5, 1);
                    break;
                case 5: // F2
                    SwapPieces(1, 5);
                    SwapPieces(2, 4);
                    break;
                case 6: // R
                    SwapPieces(3, 2);
                    SwapPieces(2, 5);
                    SwapPieces(5, 6);

                    TwistPiece(3, 2);
                    TwistPiece(2, 1);
                    TwistPiece(5, 2);
                    TwistPiece(6, 1);
                    break;
                case 7: // R'
                    SwapPieces(6, 5);
                    SwapPieces(5, 2);
                    SwapPieces(2, 3);

                    TwistPiece(3, 2);
                    TwistPiece(2, 1);
                    TwistPiece(5, 2);
                    TwistPiece(6, 1);
                    break;
                case 8: // R2
                    SwapPieces(2, 6);
                    SwapPieces(3, 5);
                    break;
            }
        }

        // swap two pieces
        void SwapPieces(int piece1 , int piece2)
        {
            int tempP = stateOP[0][piece1];
            int tempO = stateOP[1][piece1];

            stateOP[0][piece1] = stateOP[0][piece2];
            stateOP[1][piece1] = stateOP[1][piece2];

            stateOP[0][piece2] = tempP;
            stateOP[1][piece2] = tempO;
        }

        // twist a piece - ccw = 1, cw = 2
        void TwistPiece(int piece, int dir)
        {
            stateOP[1][piece] += dir;
            stateOP[1][piece] %= 3;
        }

        #endregion

        #region public methods

        // get string of OP state
        public override string ToString()
        {
            string strOP = "";

            for (int i = 0; i < 7; i++)
                strOP += stateOP[0][i].ToString();

            strOP += " ";

            for (int i = 0; i < 7; i++)
                strOP += stateOP[1][i].ToString();

            return strOP;
        }

        // get 2x7 OP state
        public int[][] GetOPState()
        {
            return stateOP;
        }

        // get piece at location
        public int GetPiece( int location )
        {
            return stateOP[0][location];
        }

        // get orientation of piece at location
        public int GetOrientation( int location )
        {
            return stateOP[1][location];
        }

        // get 6x4 sticker state
        public int[][] GetStickersState()
        {
            int[][] stateStickers = new int[6][];
            for (int i = 0; i < 6; i++)
                stateStickers[i] = new int[4];

            int[][] stickerPiece = new int[6][];
            for (int i = 0; i < 6; i++)
                stickerPiece[i] = new int[4];

            int[][] stickerOrientation = new int[6][];
            for (int i = 0; i < 6; i++)
                stickerOrientation[i] = new int[4];

            int[][] pieceCols = new int[8][];

            // the piece each sticker belongs to (7 = fixed corner)
            stickerPiece[0][0] = 0; // U
            stickerPiece[0][1] = 3;
            stickerPiece[0][2] = 1;
            stickerPiece[0][3] = 2;

            stickerPiece[1][0] = 1; // F
            stickerPiece[1][1] = 2;
            stickerPiece[1][2] = 4;
            stickerPiece[1][3] = 5;

            stickerPiece[2][0] = 2; // R
            stickerPiece[2][1] = 3;
            stickerPiece[2][2] = 5;
            stickerPiece[2][3] = 6;

            stickerPiece[3][0] = 4; // D
            stickerPiece[3][1] = 5;
            stickerPiece[3][2] = 7;
            stickerPiece[3][3] = 6;

            stickerPiece[4][0] = 3; // B
            stickerPiece[4][1] = 0;
            stickerPiece[4][2] = 6;
            stickerPiece[4][3] = 7;

            stickerPiece[5][0] = 0; // L
            stickerPiece[5][1] = 1;
            stickerPiece[5][2] = 7;
            stickerPiece[5][3] = 4;

            // the orientation associated with each sticker
            stickerOrientation[0][0] = 0; // U
            stickerOrientation[0][1] = 0;
            stickerOrientation[0][2] = 0;
            stickerOrientation[0][3] = 0;

            stickerOrientation[1][0] = 2; // F
            stickerOrientation[1][1] = 1;
            stickerOrientation[1][2] = 1;
            stickerOrientation[1][3] = 2;

            stickerOrientation[2][0] = 2; // R
            stickerOrientation[2][1] = 1;
            stickerOrientation[2][2] = 1;
            stickerOrientation[2][3] = 2;

            stickerOrientation[3][0] = 0; // D
            stickerOrientation[3][1] = 0;
            stickerOrientation[3][2] = 0;
            stickerOrientation[3][3] = 0;

            stickerOrientation[4][0] = 2; // B
            stickerOrientation[4][1] = 1;
            stickerOrientation[4][2] = 1;
            stickerOrientation[4][3] = 2;

            stickerOrientation[5][0] = 2; // L
            stickerOrientation[5][1] = 1;
            stickerOrientation[5][2] = 1;
            stickerOrientation[5][3] = 2;

            // colors associated with each piece
            pieceCols[0] = new int[3] { colUFR[0], -colUFR[1], -colUFR[2] };    // UBL
            pieceCols[1] = new int[3] { colUFR[0], -colUFR[2], colUFR[1] };     // ULF
            pieceCols[2] = new int[3] { colUFR[0], colUFR[1], colUFR[2] };      // UFR
            pieceCols[3] = new int[3] { colUFR[0], colUFR[2], -colUFR[1] };     // URB
            pieceCols[4] = new int[3] { -colUFR[0], colUFR[1], -colUFR[2] };    // DFL
            pieceCols[5] = new int[3] { -colUFR[0], colUFR[2], colUFR[1] };     // DRF
            pieceCols[6] = new int[3] { -colUFR[0], -colUFR[1], colUFR[2] };    // DBR
            pieceCols[7] = new int[3] { -colUFR[0], -colUFR[2], -colUFR[1] };   // DLB

            // create stickers state
            int piece;
            int sticker;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 4; j++ )
                {
                    if(stickerPiece[i][j] != 7) // not fixed DLB corner
                    {
                        piece = GetPiece(stickerPiece[i][j]);
                        sticker = (stickerOrientation[i][j] + 2 * GetOrientation(stickerPiece[i][j])) % 3;
                    }
                    else
                    {
                        piece = 7;
                        sticker = stickerOrientation[i][j];
                    }

                    stateStickers[i][j] = pieceCols[piece][sticker];
                }

            return stateStickers;
        }

        // get sticker at location
        public int GetSticker(int face, int sticker)
        {
            int[][] stateStickers = GetStickersState();

            return stateStickers[face][sticker];
        }

        // check if solved state
        public bool CheckSolved()
        {
            bool solved = true;

            for (int i = 0; i < 7; i++)
                if( stateOP[0][i] != i || stateOP[1][i] != 0 )
                {
                    solved = false;
                    break;
                }

            return solved;
        }

        // set OP state - from OP rep
        public void SetState(int[][] stateOPNew)
        {
            colUFR = new int[3] { 1, 2, 3 };
            stateOP = stateOPNew;
        }

        // set OP state - from sticker rep
        public void SetStateFromStickers(int[][] stateStickers)
        {
            colUFR = new int[3] { -stateStickers[3][2], -stateStickers[4][3], -stateStickers[5][2] };
            stateOP = PrimesToOP(StickersToPrimes(stateStickers));
        }

        // do move - move index
        public void DoMove(int move)
        {
            TurnCube(move);
        }

        // do move - string
        public void DoMove(string move)
        {
            Dictionary<string, int> moveList = new Dictionary<string, int>()
            {
                { "U"   ,   0 },
                { "U'"  ,   1 },
                { "U2"  ,   2 },
                { "F"   ,   3 },
                { "F'"  ,   4 },
                { "F2"  ,   5 },
                { "R"   ,   6 },
                { "R'"  ,   7 },
                { "R2"  ,   8 }
            };

            TurnCube(moveList[move]);
        }

        // do alg
        public void DoAlg(string alg)
        {
            string[] moves = alg.Trim().Split(' ');

            for (int i = 0; i < moves.Length; i++)
                DoMove(moves[i]);
        }

        #endregion
    }
}
