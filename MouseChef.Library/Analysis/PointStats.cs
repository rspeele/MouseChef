﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MouseChef.Analysis
{
    public class PointStats : IStats
    {
        private readonly List<TimePoint> _points = new List<TimePoint>();
        private readonly double _defaultValue;
        /// <summary>
        /// Mutable total of the point values received. Used for mean value.
        /// </summary>
        private double _runningTotal;

        private double? _minValue;
        private double? _maxValue;

        // Computed variance paired with the # of points recorded at the time it was computed.
        // This pairing lets us know if we need to recompute the variance.
        private KeyValuePair<int, double> _cachedVariance;
        private KeyValuePair<int, double> _cachedMedian;

        public PointStats(string description, double defaultValue, double expectedMinimum, double expectedMaximum, double majorStep, double? minorStep = null)
        {
            Description = description;
            _defaultValue = defaultValue;
            ExpectedMinimum = expectedMinimum;
            MinorStep = minorStep;
            MajorStep = majorStep;
            ExpectedMaximum = expectedMaximum;
            _cachedMedian = _cachedVariance = new KeyValuePair<int, double>(0, defaultValue);
        }

        private void PreInsert(int index, TimePoint timePoint)
        {
            if (index == 0)
            {
                Start = timePoint.Time;
            }
            if (index == _points.Count)
            {
                End = timePoint.Time;
            }
            if (_minValue == null || timePoint.Value < _minValue.Value) _minValue = timePoint.Value;
            if (_maxValue == null || timePoint.Value > _maxValue.Value) _maxValue = timePoint.Value;
            _runningTotal += timePoint.Value;
        }

        public void AddPoint(TimeSpan time, double value) => AddPoint(new TimePoint(time, value));
        public void AddPoint(TimePoint point)
        {
            // Under typical usage we'll always be adding points at a later timespan anyway,
            // but just to be sure, we check from the end of the list to keep things sorted.
            int insertIndex;
            for (insertIndex = _points.Count - 1; insertIndex >= 0; --insertIndex)
            {
                if (_points[insertIndex].Time <= point.Time) break;
            }
            ++insertIndex;
            PreInsert(insertIndex, point);
            _points.Insert(insertIndex, point);
        }

        public string Description { get; }

        public TimeSpan Start { get; private set; } = TimeSpan.Zero;
        public TimeSpan End { get; private set; } = TimeSpan.Zero;

        public double MinValue => _minValue ?? _defaultValue;
        public double MaxValue => _maxValue ?? _defaultValue;

        public double MedianValue
        {
            get
            {
                if (_cachedMedian.Key == _points.Count) return _cachedMedian.Value;
                if (_points.Count <= 0)
                {
                    _cachedMedian = new KeyValuePair<int, double>(_points.Count, _defaultValue);
                    return _cachedMedian.Value;
                }
                var sorted = _points.OrderBy(p => p.Value);
                var middle = sorted.Take(_points.Count / 2 + 1).Last();
                _cachedMedian = new KeyValuePair<int, double>(_points.Count, middle.Value);
                return _cachedMedian.Value;
            }
        }

        public double MeanValue => _points.Count > 0 ? _runningTotal / _points.Count : _defaultValue;

        /// <summary>
        /// The average of the squared differences from the mean.
        /// </summary>
        public double Variance
        {
            get
            {
                if (_cachedVariance.Key == _points.Count) return _cachedVariance.Value;
                var mean = MeanValue;
                var totalSquares = 0.0;
                foreach (var point in _points)
                {
                    var delta = point.Value - mean;
                    totalSquares += delta * delta;
                }
                var variance = totalSquares / _points.Count;
                _cachedVariance = new KeyValuePair<int, double>(_points.Count, variance);
                return variance;
            }
        }

        public double StandardDeviation => Math.Sqrt(Variance);

        private TimePointInterval BinarySearchInterval(TimeSpan time)
        {
            // TODO: cache the most recent interval and optimize for lookups that still fall within it or its immediate neighbors

            if (_points.Count <= 0)
                return new TimePointInterval
                    ( new TimePoint(TimeSpan.Zero, _defaultValue)
                    , new TimePoint(TimeSpan.Zero, _defaultValue));
            var lowIndex = 0; // First index within range of the search.
            var highBound = _points.Count; // First index beyond the range of the search.
            int previousLowIndex;
            int previousHighBound;
            do
            {
                previousLowIndex = lowIndex;
                previousHighBound = highBound;
                var searchIndex = lowIndex + (highBound - lowIndex) / 2;
                if (_points[searchIndex].Time > time)
                {
                    // We need to narrow the search to the left side.
                    highBound = searchIndex;
                    if (highBound == 0)
                    {
                        // We've reached the far left side.
                        return new TimePointInterval(_points[0]);
                    }
                }
                else
                {
                    var neighborIndex = searchIndex + 1;
                    if (neighborIndex >= _points.Count)
                    {
                        // We've reached the far right side.
                        return new TimePointInterval(_points[searchIndex]);
                    }
                    if (_points[neighborIndex].Time >= time)
                    {
                        // We've found our interval.
                        return new TimePointInterval(_points[searchIndex], _points[neighborIndex]);
                    }
                    // Otherwise, we need to narrow the search to the right side.
                    lowIndex = searchIndex;
                }
            } while (lowIndex != previousLowIndex || highBound != previousHighBound);
            throw new ProgrammerFaultException
                ("Binary search reached a state where it re-iterated without changing bounds.");
        }

        public double ValueAt(TimeSpan time)
        {
            var interval = BinarySearchInterval(time);
            return interval.InterpolateValue(time);
        }

        public IEnumerable<TimePoint> DataPoints => _points.AsReadOnly();
        public double ExpectedMinimum { get; }
        public double? MinorStep { get; }
        public double MajorStep { get; }
        public double ExpectedMaximum { get; }
    }
}
