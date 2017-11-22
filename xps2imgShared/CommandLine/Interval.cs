using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xps2Img.Shared.CommandLine
{
    [Serializable]
    public class Interval : IEquatable<Interval>
    {
        public const string ValidationRegex  = @"(^\s*$)|(^(\s*,)*\s*((\s*[1-9](\d{1,4})?\s*)|(\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*))(\s*(\s*,\s*)+\s*((\s*[1-9](\d{1,4})?\s*)|(\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*)))*(\s*,)*\s*$)";
        public const string ValidationRegex0 = @"(^\s*$)|(^(\s*,)*\s*((\s*[0-9](\d{1,4})?\s*)|(\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*))(\s*(\s*,\s*)+\s*((\s*[0-9](\d{1,4})?\s*)|(\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*)))*(\s*,)*\s*$)";

        public Interval(int point)
        {
            Begin = End = point;

            Normalize();
        }

        public Interval(int begin, int end = MaxValue, int minValue = MinValue, int maxValue = MaxValue)
        {
            ActualMinValue = minValue;
            ActualMaxValue = maxValue;

            Begin = begin;
            End = end;

            Normalize();
        }

        public Interval(string intervalString, int minValue = MinValue, int maxValue = MaxValue)
        {
            ActualMinValue = minValue;
            ActualMaxValue = maxValue;

            Func<string, int> toInt = s => int.Parse(s, CultureInfo.InvariantCulture);

            var set = intervalString.Split('-').Select(s => s.Trim()).ToArray();

            if (set.Length == 0 || set.Length > 2)
            {
                throw new FormatException();
            }

            if (set.Length == 1)
            {
                // X
                Begin = End = toInt(set[0]);
            }
            else
            if (String.IsNullOrEmpty(set[0]))
            {
                // -X
                Begin = minValue;
                End = toInt(set[1]);
            }
            else
            if (String.IsNullOrEmpty(set[1]))
            {
                // X-
                Begin = toInt(set[0]);
                End = maxValue;
            }
            else
            {
                // X-Y
                Begin = toInt(set[0]);
                End = toInt(set[1]);
            }

            Normalize();
        }

        private void Normalize()
        {
            // Normalize.
            if (Begin > End)
            {
                var _ = End;
                End = Begin;
                Begin = _;
            }
        }

        public const int MinValue = 1;
        public const int MaxValue = int.MaxValue - 2;

        public int ActualMinValue { get; private set; }
        public int ActualMaxValue { get; private set; }

        public int Begin { get; private set; }
        public int End { get; private set; }

        public bool HasOneValue { get { return Begin == End; } }
        public bool HasMinValue { get { return Begin == ActualMinValue; } }
        public bool HasMaxValue { get { return End == ActualMaxValue; } }
        public bool HasSequentialValues { get { return Begin == End - 1; } }
        
        public int Length
        {
            get
            {
                if (End >= MaxValue)
                {
                    throw new ArgumentException("Could not calculate sequence length: Interval.MaxValue has been found.");
                }
                return End - Begin + 1;
            }
        }

        public bool LessThan(int val)
        {
            return val >= Begin && (HasMaxValue || val >= End);
        }

        public void SetEndValue(int endValue)
        {
            if (Begin > endValue)
            {
                Begin = End = endValue;
            }

            if (End > endValue)
            {
                End = endValue;
            }

            ActualMaxValue = endValue;
        }

        public bool Equals(Interval other)
        {
            return other.Begin == Begin && other.End == End;
        }

        public override bool Equals(object obj)
        {
            var interval = obj as Interval;
            return obj == this || (interval != null && Equals(interval));
        }

        public override int GetHashCode()
        {
            return Begin * 31 + End;
        }

        public override string ToString()
        {
            if (HasOneValue)
            {
                return String.Format("{0}", Begin);
            }

            if (HasMaxValue)
            {
                return String.Format("{0}-", Begin);
            }

            return HasMinValue
                    ? String.Format("-{0}", End)
                    : String.Format(HasSequentialValues ? "{0},{1}" : "{0}-{1}", Begin, End);
        }

        public static List<Interval> Parse(string intervalString, int minValue = MinValue)
        {
            if (intervalString == null || String.IsNullOrEmpty(intervalString.Trim()))
            {
                return new List<Interval> { new Interval(minValue, minValue: minValue) };
            }

            return Optimize(
                        Adjust(intervalString).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(s => !String.IsNullOrEmpty(s.Trim()))
                            .Select(interval => new Interval(interval, minValue)).ToList(),
                        minValue);
        }

        public static string Adjust(string intervalString)
        {
            return Regex.Replace(intervalString, @"\s*([\-,]|\s+)\s*", m => String.IsNullOrEmpty(m.Value.Trim()) ? ",".PadRight(m.Value.Length) : m.Value);
        }

        private static List<Interval> Optimize(IList<Interval> intervals, int minValue = MinValue)
        {
            var intervalsOptimized = new List<Interval>();

            Func<Interval, Interval, bool> inside = (x, y) =>
            {
                if (x.Begin >= y.Begin && x.End <= y.End)
                {
                    intervals.Add(y);
                    return true;
                }
                return false;
            };

            Func<Interval, Interval, bool> less = (x, y) =>
            {
                if (
                    (x.Begin <= y.Begin && x.End >= y.Begin && x.End <= y.End) ||
                    (x.End + 1 == y.Begin && x.Begin <= y.Begin)
                )
                {
                    intervals.Add(new Interval(x.Begin, y.End, minValue));
                    return true;
                }
                return false;
            };

            Func<Interval, Interval, bool> more = (x, y) =>
            {
                if (x.End >= y.End && x.Begin >= y.Begin && x.Begin <= y.End)
                {
                    intervals.Add(new Interval(y.Begin, x.End, minValue));
                    return true;
                }
                return false;
            };

            for (var i = 0; i < intervals.Count; )
            {
                var x = intervals[i];

                var xRemoved = false;

                for (var j = i + 1; j < intervals.Count; j++)
                {
                    var y = intervals[j];

                    if (
                        inside(x, y) || inside(y, x) ||
                        less(x, y) || less(y, x) ||
                        more(x, y) || more(y, x)
                    )
                    {
                        xRemoved = true;
                        intervals.RemoveAt(i);
                        intervals.RemoveAt(i > j ? j : j - 1);
                        break;
                    }
                }

                if (!xRemoved)
                {
                    intervalsOptimized.Add(x);
                    i++;
                }
            }

            intervalsOptimized.Sort((x, y) => x.Begin < y.Begin ? -1 : 1);

            return intervalsOptimized;
        }
    }
}
