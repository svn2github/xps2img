using System;
using System.Runtime.Serialization;

namespace CommandLine.Validation
{
  public class ValidationException : Exception
  {
    public ValidationException()
    {
    }
    
    public ValidationException(string message)
      : base(message)
    {
    }
    
    public ValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}