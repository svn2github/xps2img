using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xps2Img.CommandLine
{
    public class Interval
    {
        public const string ValidationRegex  = @"(^\s*$)|(^(\s*,)*\s*((\s*[1-9](\d{1,4})?\s*)|(\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*))(\s*(\s*,\s*)+\s*((\s*[1-9](\d{1,4})?\s*)|(\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*[1-9](\d{1,4})?\s*)|(\s*[1-9](\d{1,4})?\s*-\s*)))*(\s*,)*\s*$)";
        public const string ValidationRegex0 = @"(^\s*$)|(^(\s*,)*\s*((\s*[0-9](\d{1,4})?\s*)|(\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*))(\s*(\s*,\s*)+\s*((\s*[0-9](\d{1,4})?\s*)|(\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*[0-9](\d{1,4})?\s*)|(\s*[0-9](\d{1,4})?\s*-\s*)))*(\s*,)*\s*$)";

        public Interval()
            : this(MinValue, MaxValue)
        {
        }

        public Interval(int point)
        {
            Begin = End = point;

            Normalize();
        }

        public Interval(int begin, int end)
        {
            Begin = begin;
            End = end;

            Normalize();
        }

        public Interval(string intervalString)
        {
            Func<string, int> toInt = s => int.Parse(s, CultureInfo.InvariantCulture);

            var set = intervalString.Split(new[] { '-' }).Select(s => s.Trim()).ToArray();

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
                Begin = MinValue;
                End = toInt(set[1]);
            }
            else
            if (String.IsNullOrEmpty(set[1]))
            {
                // X-
                Begin = toInt(set[0]);
                End = MaxValue;
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

        public int Begin { get; private set; }
        public int End { get; private set; }

        public bool HasOneValue { get { return Begin == End; } }
        public bool HasMinValue { get { return Begin == MinValue; } }
        public bool HasMaxValue { get { return End == MaxValue; } }
        public bool HasSequentialValues { get { return Begin == End - 1; } }
        
        public int Length
        {
            get
            {
                if (HasMaxValue)
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

        public static List<Interval> Parse(string intervalString)
        {
            if (intervalString == null || String.IsNullOrEmpty(intervalString.Trim()))
            {
                return new List<Interval> { new Interval() };
            }

            return Optimize(intervalString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !String.IsNullOrEmpty(s.Trim()))
                    .Select(interval => new Interval(interval)).ToList());
        }

        private static List<Interval> Optimize(IList<Interval> intervals)
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
                    intervals.Add(new Interval(x.Begin, y.End));
                    return true;
                }
                return false;
            };

            Func<Interval, Interval, bool> more = (x, y) =>
            {
                if (x.End >= y.End && x.Begin >= y.Begin && x.Begin <= y.End)
                {
                    intervals.Add(new Interval(y.Begin, x.End));
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
