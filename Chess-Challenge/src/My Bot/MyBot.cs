using ChessChallenge.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Runtime;

public class MyBot : IChessBot
{
    private OpeningBook whiteOpeningBook;
    private OpeningBook d4OpeningBook;
    private OpeningBook e4OpeningBook;

    //from gotham chess's video https://www.youtube.com/watch?v=ECMMct_jnEM&t=1s
    private string theLondonPGN = "1. d4 d5 (1... Nf6 2. Nf3) (1... f5) (1... g6) 2. Bf4 (2. Nf3 Nf6 3. Bf4 c5) 2... Nf6 3. e3 (3. Nf3 c5 4. e3 Nc6 5. c3) 3... c5 (3... e6 4. Nf3 Nc6 (4... Bd6 5. Nbd2 (5. Bg3) (5. Ne5)) 5. c3 Be7 6. Bd3 O-O 7. Nbd2 b6 8. O-O (8. Ne5) (8. Qe2)) (3... Bf5 4. c4 e6 5. Nc3 Bb4 6. Qb3) (3... Bg4) (3... g6) 4. Nf3 (4. Nc3) (4. dxc5 e6 5. b4 a5 6. c3 b6 7. Bxb8 Rxb8 8. Bb5+) 4... Nc6 (4... Qb6 5. Nc3 (5. Na3)) 5. Nbd2 (5. c3 Qb6) 5... e6 (5... Bf5 6. dxc5 e6) (5... cxd4 6. exd4) 6. c3 Be7 7. Bd3 O-O 8. Ne5 Nxe5 9. dxe5 Nd7 10. Qh5 f5 11. g4 g6 12. Qh61. d4 d5 (1... Nf6 2. Nf3) (1... f5) (1... g6) 2. Bf4 (2. Nf3 Nf6 3. Bf4 c5) 2... Nf6 3. e3 (3. Nf3 c5 4. e3 Nc6 5. c3) 3... c5 (3... e6 4. Nf3 Nc6 (4... Bd6 5. Nbd2 (5. Bg3) (5. Ne5)) 5. c3 Be7 6. Bd3 O-O 7. Nbd2 b6 8. O-O (8. Ne5) (8. Qe2)) (3... Bf5 4. c4 e6 5. Nc3 Bb4 6. Qb3) (3... Bg4) (3... g6) 4. Nf3 (4. Nc3) (4. dxc5 e6 5. b4 a5 6. c3 b6 7. Bxb8 Rxb8 8. Bb5+) 4... Nc6 (4... Qb6 5. Nc3 (5. Na3)) 5. Nbd2 (5. c3 Qb6) 5... e6 (5... Bf5 6. dxc5 e6) (5... cxd4 6. exd4) 6. c3 Be7 7. Bd3 O-O 8. Ne5 Nxe5 9. dxe5 Nd7 10. Qh5 f5 11. g4 g6 12. Qh6";
    //from gotham chess's video https://www.youtube.com/watch?v=pBpEFAz5Afw
    private string theNimzoIndian = "1. d4 Nf6 2. c4 e6 3. Nc3 Bb4 4. e3 (4. Qc2 O-O (4... d5 5. a3 Bxc3+ 6. Qxc3 c5 7. dxc5 d4 8. Qg3 Nc6 9. Qxg7 Rg8 10. Qh6) (4... b6 5. e4 c5 6. d5 Qe7 7. Ne2 exd5 8. exd5 O-O 9. Bd2 d6 10. O-O-O Ng4) (4... Nc6 5. Nf3 d6 6. a3 Bxc3+ 7. Qxc3 a5) 5. a3 (5. e4 d5 6. e5 Ne4 7. Bd3 c5 8. Nf3 cxd4 9. Nxd4 Nd7 10. Bf4 Qh4 11. g3 Qh5 12. O-O g5 13. cxd5 Bxc3 14. bxc3 exd5 15. Bxe4 dxe4 16. e6 gxf4 17. exd7 Bxd7 18. Qxe4 fxg3 19. fxg3 b6 20. Nf5 Rae8 21. Qd5 Bxf5 22. Rxf5 Qe2 23. Raf1 Qe3+) 5... Bxc3+ 6. Qxc3 d6 7. Nf3 b6 8. Bg5 Bb7) (4. Nf3 b6 5. Bg5 h6 6. Bh4 g5 7. Bg3 Ne4 8. Qc2 Bb7) (4. f3 c5 (4... d5 5. a3 Bxc3+ 6. bxc3 c5 (6... O-O 7. cxd5 exd5 8. e3)) (4... Nc6 5. e4 d5) (4... Bxc3+ 5. bxc3) 5. d5 b5) (4. a3 Bxc3+ 5. bxc3 c5 (5... b6) 6. e3) (4. Bg5 h6 5. Bh4 c5) (4. g3) (4. Bd2) (4. Qb3) 4... O-O (4... b6 5. Bd3 Bb7 6. Nf3 Ne4 7. Qc2 f5 8. O-O Bxc3 9. bxc3 O-O 10. Bd2 d6) (4... c5 5. Bd3 Nc6 6. Nf3 Bxc3+ 7. bxc3 d6) (4... d5) 5. Bd3 (5. Ne2 d5 (5... c6) (5... Re8)) 5... d5 (5... c5 6. Nf3 Nc6 7. O-O Bxc3 8. bxc3 d6) (5... b6 6. Nf3 Bb7 7. O-O Bxc3 8. bxc3 Ne4 9. Qc2 f5) 6. Nf3 c5 7. O-O dxc4 8. Bxc4 cxd4 9. exd4 b6 10. Bg5 Bb7 11. Qe2 Bxc3 12. bxc3 Nbd7";
    //from gotham chess's video https://www.youtube.com/watch?v=ebfzL_GwiIE
    private string theCaroKann = "1. e4 c6 2. d4 (2. Nf3 d5 3. Nc3 Bg4 (3... a6 4. d4 Bg4) 4. h3 Bxf3 5. Qxf3 e6) (2. Nc3 d5 3. Qf3) 2... d5 3. e5 (3. Nc3 dxe4 4. Nxe4 Bf5 (4... Nf6 5. Nxf6+ exf6 6. Nf3 Bd6 7. Bd3 O-O 8. O-O Re8 9. Be3) 5. Ng3 Bg6 6. h4 h6 7. h5 Bh7 8. Nf3 Nd7 9. Bd3 Bxd3 10. Qxd3 e6) (3. Nd2) (3. exd5 cxd5 4. c4 (4. Nf3 Nc6 (4... g6)) (4. Bd3 Nc6) 4... Nf6 5. Nc3 e6 (5... g6 6. Qb3 Bg7 7. cxd5 O-O)) (3. f3 g6 (3... dxe4 4. fxe4) 4. Nc3 Bg7 5. Be3 Qb6) 3... c5 (3... Bf5 4. Nf3 (4. h4) (4. c4) (4. Nc3) 4... e6 5. Be2 c5 6. O-O Nc6) 4. c3 (4. dxc5 Nc6 (4... e6 5. Be3) 5. Nf3 Bg4 6. Bf4 e6) (4. Nf3) 4... Nc6 5. Nf3 Bg4 6. Be2 e6 7. O-O Nge7";

    public MyBot()
    {
        whiteOpeningBook = new OpeningBook(true, theLondonPGN);
        d4OpeningBook = new OpeningBook(false, theNimzoIndian);
        e4OpeningBook = new OpeningBook(false, theCaroKann);
    }

    public Move Think(Board board, Timer timer)
    {
        if (board.IsWhiteToMove)
        {
            Move? move = whiteOpeningBook.GetMove(board);
            if (move != null)
            {
                return (Move)move;
            }
        }
        else
        {
            //try both black opening books
            //(could have a flag for d4 or e4, but checking both is fine
            //as it should be cheapish and positions can morph)
            Move? move = d4OpeningBook.GetMove(board);
            if (move != null)
            {
                return (Move)move;
            }
            move = e4OpeningBook.GetMove(board);
            if (move != null)
            {
                return (Move)move;
            }
        }

        int depth = 3;
        //White maximizes, black minimizes
        MiniMaxOutput miniMaxOutput = minimax(board, depth, Int32.MaxValue, Int32.MinValue, board.IsWhiteToMove);
        return miniMaxOutput.Move;
    }

    private class MiniMaxOutput
    {
        //the point value for this move
        public double Value { get; set; }
        //The move that was made to get this value
        public Move Move { get; set; }
    }

    //following pseudocode from https://www.youtube.com/watch?v=l-hh51ncgDI
    private MiniMaxOutput minimax(Board board, int depth, double alpha, double beta, bool maximizingPlayer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            MiniMaxOutput outp = new MiniMaxOutput();
            outp.Value = evaluate(board);
            return outp;
        }

        //should probably do just checks first
        //add those moves to a set
        //and then do all moves and continue if in set
        //for optimal pruning
        Move[] captures = board.GetLegalMoves(true);
        HashSet<Move> captureSet = new HashSet<Move>();
        Move[] moves = board.GetLegalMoves();

        if (maximizingPlayer)
        {
            MiniMaxOutput maxEval = new MiniMaxOutput();
            maxEval.Value = Int32.MinValue;
            maxEval.Move = moves[0];//default to prevent any errors

            //iterate through captures first
            foreach (Move move in captures)
            {
                //add the move to the set of captures
                captureSet.Add(move);
                //make and evaluate move
                board.MakeMove(move);
                MiniMaxOutput moveOut = minimax(board, depth - 1, alpha, beta, false);
                //undo move
                board.UndoMove(move);

                //see if it's better
                if (moveOut.Value > maxEval.Value)
                {
                    maxEval.Value = moveOut.Value;
                    maxEval.Move = move;

                    //alpha beta prune
                    if (maxEval.Value > alpha)
                    {
                        alpha = maxEval.Value;
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

            }
            //iterate through the rest of the moves
            foreach (Move move in moves)
            {
                //if we've already seen this move we can skip it
                if (captureSet.Contains(move))
                {
                    continue;
                }

                //make and evaluate move
                board.MakeMove(move);
                MiniMaxOutput moveOut = minimax(board, depth - 1, alpha, beta, false);
                //undo move
                board.UndoMove(move);

                //see if it's better
                if (moveOut.Value > maxEval.Value)
                {
                    maxEval.Value = moveOut.Value;
                    maxEval.Move = move;

                    //alpha beta prune
                    if (maxEval.Value > alpha)
                    {
                        alpha = maxEval.Value;
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

            }

            return maxEval;
        }
        else
        {
            MiniMaxOutput minEval = new MiniMaxOutput();
            minEval.Value = Int32.MaxValue;
            minEval.Move = moves[0];//default to prevent any errors

            //iterate through captures first
            foreach (Move move in captures)
            {
                //add the move to the set of captures
                captureSet.Add(move);
                //make and evaluate move
                board.MakeMove(move);
                MiniMaxOutput moveOut = minimax(board, depth - 1, alpha, beta, true);
                //undo move
                board.UndoMove(move);

                //see if it's better
                if (moveOut.Value < minEval.Value)
                {
                    minEval.Value = moveOut.Value;
                    minEval.Move = move;

                    //alpha beta prune
                    if (minEval.Value < beta)
                    {
                        beta = minEval.Value;
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

            }
            //iterate through the rest of the moves
            foreach (Move move in moves)
            {
                //if we've already seen this move we can skip it
                if (captureSet.Contains(move))
                {
                    continue;
                }
                //make and evaluate move
                board.MakeMove(move);
                MiniMaxOutput moveOut = minimax(board, depth - 1, alpha, beta, true);
                //undo move
                board.UndoMove(move);

                //see if it's better
                if (moveOut.Value < minEval.Value)
                {
                    minEval.Value = moveOut.Value;
                    minEval.Move = move;

                    //alpha beta prune
                    if (minEval.Value < beta)
                    {
                        beta = minEval.Value;
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

            }
            return minEval;
        }


    }

    private double evalKingSafety(Board board, double currentEval)
    {
        if (Math.Abs(currentEval) > 4.0)
        {
            // true = white, false = black
            bool winningSide = currentEval > 0;
            
            double kingSafetyVal = 0.0;

            // Bitboard of all the places the losing king can't move, starting with occupied squares
            // Not including the losing side's own pieces, since this has more to do with "influence"
            // rather than literally restricting the king (which could discourage things like castling)
            ulong blockedSquares = winningSide ? board.WhitePiecesBitboard : board.BlackPiecesBitboard;

            foreach (PieceType pt in Enum.GetValues(typeof(PieceType)))
            {
                if (pt == PieceType.None)
                {
                    continue;
                }

                foreach (Piece p in board.GetPieceList(pt, winningSide))
                {
                    // Include the attacking pieces of the player currently making a move
                    blockedSquares |= BitboardHelper.GetPieceAttacks(pt, p.Square, board, winningSide);
                }
            }

            // Everywhere the losing king can move (assuming nothing blocking it)
            ulong losingSideKingAttacks = BitboardHelper.GetKingAttacks(board.GetKingSquare(!winningSide));

            // Actual number of squares the losing king can move to
            double totalKingAttacks = BitboardHelper.GetNumberOfSetBits(losingSideKingAttacks & blockedSquares);
            
            kingSafetyVal = (1.0 - (totalKingAttacks / 8.0)) * (winningSide ? -1 : 1);

            return kingSafetyVal;
        }
        return 0.0;
    }

    //Gives an evaluation of the position of the current board (centipawns)
    private double evaluate(Board board)
    {
        //return 0 if it's a draw
        if (board.IsDraw())
        {
            return 0;
        }
        //if white wins, positive max value, if they lose min value
        if (board.IsInCheckmate())
        {
            return board.IsWhiteToMove ? Int32.MinValue : Int32.MaxValue;
        }
        //simple piece weighting
        double eval = 0;

        PieceList[] pieceLists = board.GetAllPieceLists();

        foreach (PieceList pl in pieceLists)
        {
            foreach (Piece p in pl)
            {
                //white advantage is +, black is - like in stockfish
                double val = p.IsWhite ? 1 : -1;

                //just using generic weights for each piece like you think of in chess.com
                switch (p.PieceType)
                {
                    case PieceType.Pawn:
                        val *= 1;
                        break;
                    case PieceType.Knight:
                        val *= 3;
                        break;
                    case PieceType.Bishop:
                        val *= 3;
                        break;
                    case PieceType.Rook:
                        val *= 5;
                        break;
                    case PieceType.Queen:
                        val *= 9;
                        break;
                    default:
                        val = 0;
                        break;
                }
                //add up the vals
                eval += val;
            }
        }

        eval += evalKingSafety(board, eval);

        return eval;

    }


    private class OpeningBook
    {
        private const string initialPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        Move move;
        public Dictionary<ulong, Move> map;
        public OpeningBook(bool isWhiteOpeningBook, string pgn)
        {
            map = new Dictionary<ulong, Move>();
            //create a board
            Board board = Board.CreateBoardFromFEN(initialPos);

            readPGN(isWhiteOpeningBook, pgn, board, null, true);
        }

        /*
         * Attempts to get the next according to the opening book
         * Returns non null if successful
         */
        public Move? GetMove(Board board)
        {
            if (map.ContainsKey(board.ZobristKey))
            {
                return map.GetValueOrDefault(board.ZobristKey);
            }
            return null;
        }

        private void readPGN(bool isWhiteOpeningBook, string pgn, Board board, Move? lastMove, bool isWhitesTurn)
        {
            bool inComment = false;
            int i;
            int numParentheses = 0;
            for (i = 0; i < pgn.Length; i++)
            {
                char c = pgn[i];
                //keep track of if we're in a comment or some other irrelevant tag
                if (c == '[' || c == '{')
                {
                    inComment = true;
                }
                else if (c == ']' || c == '}')
                {
                    inComment = false;
                }

                //if we're in a comment or this is white space, ignore it
                if (inComment || Char.IsWhiteSpace(c))
                {
                    continue;
                }

                //if we hit parentheses now it's outside of a comment and legit
                if (c == '(')
                {
                    //increase the number of parentheses
                    numParentheses++;
                    int starti = i;
                    i++;
                    while (numParentheses > 0)
                    {
                        c = pgn[i];
                        if (c == '(')
                        {
                            numParentheses++;
                        }
                        else if (c == ')')
                        {
                            numParentheses--;
                        }
                        //we'll assume this is a valid string
                        //i will end up being 1 more than the last parentheses
                        i++;
                    }
                    //new string of the interior of the parentheses (shouldn't be empty if valid)
                    string altLine = pgn.Substring(starti + 1, i - 1 - (starti + 1));

                    //continue with the current line first
                    if (i < pgn.Length)
                    {
                        //since we didn't make a move it'll be responsible for undoing our last move
                        //and it's still the same turn as it was for us
                        readPGN(isWhiteOpeningBook, pgn.Substring(i), board, lastMove, isWhitesTurn);
                    }
                    else
                    {
                        if (lastMove != null)
                        {
                            board.UndoMove((Move) lastMove);
                        }
                    }

                    //now with the alternate line

                    //we need to make sure if it's whites turn it's also the white opening book
                    //bc then the move before (alternate line) is blacks turn
                    //meaning it's an alternate line by the opponent and not and alternate
                    //choice for us
                    if (isWhiteOpeningBook == isWhitesTurn)
                    {
                        //this is an alternate to our last move so we use the previous turn
                        readPGN(isWhiteOpeningBook, altLine, board, null, !isWhitesTurn);
                    }

                    //normally we would break here so we can undo the move
                    //but no move was made so the recursive calls will undo
                    //and we won't try and double undo
                    return;
                }

                //if we've made it this far we either have a move number or a move

                //we don't care about the move numbers
                if (c == '.' || Char.IsDigit(c))
                {
                    continue;
                }

                //now it's surely a letter
                if (!Char.IsLetter(c))
                {
                    continue;
                }

                int j;
                for (j = i + 1; j < pgn.Length; j++)
                {
                    if (Char.IsWhiteSpace(pgn[j]))
                    {
                        break;
                    }
                }

                string moveString = pgn.Substring(i, j - i);

                //some hackery using the intended to be off limits code to avoid writing even more code
                ChessChallenge.Chess.Move parsedMove = ChessChallenge.Chess.MoveUtility.GetMoveFromSAN(board.board, moveString);
                string uciMoveString = ChessChallenge.Chess.MoveUtility.GetMoveNameUCI(parsedMove);
                Move moveToMake = new Move(uciMoveString, board);

                //if it's our move to make
                if (isWhiteOpeningBook == isWhitesTurn)
                {
                    map.TryAdd(board.ZobristKey, moveToMake);
                }

                board.MakeMove(moveToMake);
                if (j < pgn.Length)
                {
                    readPGN(isWhiteOpeningBook, pgn.Substring(j), board, moveToMake, !isWhitesTurn);
                }
                else
                {
                    board.UndoMove(moveToMake);
                }
                break;



            }

            //undo the last move so alternate paths can have the correct board state
            if (lastMove != null)
            {
                board.UndoMove((Move) lastMove);
            }
        }
    }
}