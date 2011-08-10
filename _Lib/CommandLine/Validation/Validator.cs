namespace CommandLine.Validation
{
  internal abstract class Validator
  {
    public abstract void Validate(string value);
    
    public static bool IsTypeOf<T>(object value) where T: class
    {
      return value != null && typeof (T) == value.GetType();
    }   
  }
}