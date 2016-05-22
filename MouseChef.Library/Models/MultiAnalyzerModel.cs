using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MouseChef.Analysis;

namespace MouseChef.Models
{
    public class MultiAnalyzerModel : INotifyPropertyChanged
    {
        private readonly List<AnalyzerModel> _analyzerModels;
        private Mouse _baseline;
        private Mouse _subject;

        public Mouse Baseline
        {
            get { return _baseline; }
            set { _baseline = value; OnPropertyChanged(); }
        }

        public Mouse Subject
        {
            get { return _subject; }
            set { _subject = value; OnPropertyChanged(); }
        }

        public MultiAnalyzerModel(IEnumerable<AnalyzerModel> analyzerModels)
        {
            _analyzerModels = analyzerModels.ToList();
        }

        public IEnumerable<AnalyzerModel> Analyzers => _analyzerModels;

        public List<Move> Update(IEnumerable<Move> allMoves)
        {
            var moves = allMoves.ToList();
            if (Baseline == null || Subject == null)
            {
                var mice = moves.Select(m => m.Mouse).Distinct().Take(2).ToList();
                if (Baseline == null && mice.Count >= 1)
                {
                    Baseline = mice[0];
                }
                if (Subject == null && mice.Count >= 2)
                {
                    Subject = mice[1];
                }
                if (Baseline == null || Subject == null)
                    return moves;
            }
            foreach (var analyzer in _analyzerModels)
            {
                var stats = analyzer.Analyzer.Analyze(Baseline, Subject, moves);
                analyzer.LatestStats = stats;
                var factor = analyzer.Factor();
                for (var i = 0; i < moves.Count; i++)
                {
                    if (moves[i].Mouse != Subject) continue;
                    moves[i] = analyzer.Analyzer.TransformMove(moves[i], factor);
                }
            }
            return moves;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}