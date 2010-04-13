using System;
using System.Diagnostics;

namespace Xps2Img
{
  public class MemoryUsageChecker
  {
    private readonly int checkInterval;

    private readonly long memoryLimit;

    private int checkCountdown;

    public MemoryUsageChecker(int? mbMemoryLimit):
      this(mbMemoryLimit.HasValue, mbMemoryLimit ?? 0)
    {
    }
    
    public MemoryUsageChecker(bool enabled, int mbMemoryLimit)
    {
      if(!enabled)
      {
        checkInterval = int.MaxValue;
        memoryLimit = int.MaxValue;
      }
      else
      {
        checkInterval = (int)Math.Log(mbMemoryLimit, 2) + 1;
        memoryLimit = mbMemoryLimit * 1024 * 1024;
      }

      checkCountdown = checkInterval;
    }
      
    public void Check()
    {
      if (--checkCountdown != 0)
      {
        return;
      }
        
      if (Process.GetCurrentProcess().WorkingSet64 > memoryLimit)
      {
        throw new OutOfMemoryException();
      }
        
      checkCountdown = checkInterval;
    }
  }
}