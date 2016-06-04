using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MouseChef.Analysis;

namespace MouseChef.Models
{
    public enum AnalyzerFactorMode
    {
        Mean,
        Median,
        Override,
    }
    public class AnalyzerModel : INotifyPropertyChanged
    {
        private IStats _latestStats;
        private double _overrideFactor;
        private AnalyzerFactorMode _factorMode = AnalyzerFactorMode.Mean;

        public AnalyzerModel(IAnalyzer analyzer)
        {
            Analyzer = analyzer;
            OverrideFactor = analyzer.DefaultFactor;
        }

        public IAnalyzer Analyzer { get; }

        public IStats LatestStats
        {
            get { return _latestStats; }
            set { _latestStats = value; OnPropertyChanged(); OnPropertyChanged(nameof(Factor)); }
        }

        public double OverrideFactor
        {
            get { return _overrideFactor; }
            set { _overrideFactor = value; OnPropertyChanged(); OnPropertyChanged(nameof(Factor)); }
        }

        public IEnumerable<AnalyzerFactorMode> FactorModes
            => (AnalyzerFactorMode[])Enum.GetValues(typeof(AnalyzerFactorMode));

        public AnalyzerFactorMode FactorMode
        {
            get { return _factorMode; }
            set
            {
                if (_factorMode == value) return;
                if (_factorMode != AnalyzerFactorMode.Override
                    && value == AnalyzerFactorMode.Override)
                {
                    OverrideFactor = Factor;
                }
                _factorMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Factor));
                OnPropertyChanged(nameof(IsOverridden));
            }
        }

        public double Factor
        {
            get
            {
                switch (FactorMode)
                {
                    case AnalyzerFactorMode.Mean:
                        return LatestStats?.MeanValue ?? Analyzer.DefaultFactor;
                    case AnalyzerFactorMode.Median:
                        return LatestStats?.MedianValue ?? Analyzer.DefaultFactor;
                    case AnalyzerFactorMode.Override:
                    default:
                        return OverrideFactor;
                }
            }
        }

        public bool IsOverridden => FactorMode == AnalyzerFactorMode.Override;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
