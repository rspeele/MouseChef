﻿using System.ComponentModel;
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

        public AnalyzerFactorMode FactorMode
        {
            get { return _factorMode; }
            set { _factorMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(Factor)); }
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}