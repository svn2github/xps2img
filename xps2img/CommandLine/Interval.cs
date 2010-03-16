using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xps2Img.CommandLine
{
  public class Interval
  {
    public Interval() : this(MinValue, MaxValue)
    {
    }
    
    public Interval(int begin, int end)
    {
      Begin = begin;
      End = end;
      
      Normalize();
    }
    
    public Interval(string intervalString)
    {
      var set = intervalString.Split(new[] { '-' });
      
      if(set.Length == 1)
      {
        // X
        Begin = End = Convert.ToInt32(set[0]);
      }
      else
      if(String.IsNullOrEmpty(set[0]))
      {
        // -X
        Begin = MinValue;
        End = Convert.ToInt32(set[1]);
      }
      else            
      if(String.IsNullOrEmpty(set[1]))
      {
        // X-
        Begin = Convert.ToInt32(set[0]);
        End = MaxValue;
      }
      else
      {
        // X-Y
        Begin = Convert.ToInt32(set[0]);
        End = Convert.ToInt32(set[1]);
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
    public const int MaxValue = int.MaxValue-2;

    public int Begin { get; set; }
    public int End { get; set; }

    public bool HasOneValue { get { return Begin == End; } }
    public bool HasMinValue { get { return Begin == MinValue; } }
    public bool HasMaxValue { get { return End == MaxValue; } }
    
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
    
    public void SetEndValue(int beginValue)
    {
      if (Begin > beginValue)
      {
        Begin = End = beginValue;
      }
      
      if(End > beginValue)
      {
        End = beginValue;
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

      if (HasMinValue)
      {
        return String.Format("-{0}", End);
      }
      
      return String.Format("{0}-{1}", Begin, End);
    }

    public const string ValidationRegex = @"^(,?((\d{1,5}-\d{1,5})|(\d{1,5}-?)|(-?\d{1,5})))+$";
    private static readonly Regex validationRegex = new Regex(ValidationRegex);

    public static List<Interval> Parse(string intervalString)
    {
      if(String.IsNullOrEmpty(intervalString))
      {
        return new List<Interval>{ new Interval() };
      }

      if (!validationRegex.IsMatch(intervalString))
      {
        throw new ArgumentException(@"UNEXPECTED: Interval format is invalid", "intervalString");
      }
      
      return Optimize(intervalString.Split(new[] {','}).Select(interval => new Interval(interval)).ToList());
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
        
        if(!xRemoved)
        {
          intervalsOptimized.Add(x);
          i++;
        }
      }

      intervalsOptimized.Sort((x, y) => x.Begin < y.Begin ? -1 : 1);

      return intervalsOptimized;
    }
  }
  
  public static class IntervalUtils
  {
    public static string ToString(this IEnumerable<Interval> intervals)
    {
      var sb = new StringBuilder(16);
      var first = true;
      foreach (var interval in intervals)
      {
        if (!first)
        {
          sb.Append(',');
        }
        sb.Append(interval);
        first = false;
      }
      return sb.ToString();
    }

    public static int GetTotalLength(this IEnumerable<Interval> intervals)
    {
      return intervals.Sum(interval => interval.Length);
    }
    
    public static void SetEndValue(this IEnumerable<Interval> intervals, int endValue)
    {
      foreach (var interval in intervals)
      {
        interval.SetEndValue(endValue);
      }
    }

    public static List<Interval> AdjustBeginValue(this IEnumerable<Interval> intervals, int beginValue)
    {
      return intervals
              .Where(interval => interval.End >= beginValue)
              .Select(interval => (interval.Begin >= beginValue) ? interval : new Interval(beginValue, interval.End))
              .ToList();
    }
  }
}
