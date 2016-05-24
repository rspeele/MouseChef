using System.Collections.Generic;
using System.Linq;
using MouseChef.Analysis;

namespace MouseChef.Models
{
    public class MultiAnalyzerModel
    {
        private readonly List<AnalyzerModel> _analyzerModels;

        public Mouse Baseline { get; set; }
        public Mouse Subject { get; set; }

        public MultiAnalyzerModel(IEnumerable<AnalyzerModel> analyzerModels)
        {
            _analyzerModels = analyzerModels.ToList();
        }

        public void Reset()
        {
            Baseline = null;
            Subject = null;
        }

        public IEnumerable<AnalyzerModel> Analyzers => _analyzerModels;

        public List<Move> Update(IEnumerable<Move> allMoves)
        {
            var moves = allMoves.ToList();
            if (Baseline == null || Subject == null)
            {
                return moves;
            }
            foreach (var analyzer in _analyzerModels)
            {
                var stats = analyzer.Analyzer.Analyze(Baseline, Subject, moves);
                analyzer.LatestStats = stats;
                var factor = analyzer.Factor;
                for (var i = 0; i < moves.Count; i++)
                {
                    if (moves[i].Mouse != Subject) continue;
                    moves[i] = analyzer.Analyzer.TransformMove(moves[i], factor);
                }
            }
            return moves;
        }
    }
}