using System;
using System.Diagnostics;

namespace Xps2Img
{
  public class MemoryUsageChecker
  {
    private const int checkInterval = 10;

    private readonly long memoryLimit;

    private int checkCountdown = checkInterval;

    public MemoryUsageChecker(bool enabled, int mbMemoryLimit)
    {
      memoryLimit = mbMemoryLimit * 1024 * 1024;
      if(!enabled)
      {
        checkCountdown = Int32.MaxValue;
      }
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