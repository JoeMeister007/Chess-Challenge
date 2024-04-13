using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Runtime;

public class MyBot : IChessBot
{

    public Move Think(Board board, Timer timer)
    {
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
        return eval;

    }

}