using System.Collections.Generic;

namespace MouseChef.Analysis
{
    public interface IAnalyzer
    {
        string Name { get; }
        string Description { get; }
        double DefaultFactor { get; }
        /// <summary>
        /// Generate statistics from a stream of moves.
        /// The values reported in the resulting statistics are factors that would transform the subject mouse
        /// to be in line with the baseline mouse.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="subject"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        IStats Analyze(Mouse baseline, Mouse subject, IEnumerable<Move> moves);
        /// <summary>
        /// Given a factor (entered manually, or pulled from statistics), transform a move of the subject mouse.
        /// This is how you apply corrections obtained from `Analyze`.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        Move TransformMove(Move move, double factor);
    }
}
