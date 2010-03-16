using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Xps2Img
{
  [TypeConverter(typeof(ConverterStateTypeConverter))]
  public class ConverterState
  {
    public int ActivePage { get; set; }
    public int ActivePageIndex { get; set; }
    public int LastPage { get; set; }
    
    public int TotalPages { get; set; }
    
    public bool HasPageCount { get { return TotalPages != 0; } }

    private static readonly Encoding Encoding = Encoding.UTF8;
    
    public void SetLastAndTotalPages(int lastPage, int totalPages)
    {
      LastPage    = lastPage;
      TotalPages  = totalPages;
    }

    public string Serialize()
    {
      using (var stringWriter = new StringWriter())
      {
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { NewLineHandling = NewLineHandling.None }))
        {
          // ReSharper disable AssignNullToNotNullAttribute
          new XmlSerializer(typeof(ConverterState)).Serialize(xmlWriter, this);
          // ReSharper restore AssignNullToNotNullAttribute
        }
        return Convert.ToBase64String(Encoding.GetBytes(stringWriter.ToString()));
      }
    }

    public static ConverterState Deserialize(string xml)
    {
      using (var stringReader = new StringReader(Encoding.GetString(Convert.FromBase64String(xml))))
      {
        return (ConverterState)(new XmlSerializer(typeof(ConverterState)).Deserialize(stringReader));
      }
    }
    
    public static bool IsSerializedTo(string str)
    {
      // base64 for "<?xml"
      return !String.IsNullOrEmpty(str) && str.StartsWith("PD94bWw");
    }
    
    public double Percent { get { return (double)ActivePageIndex / TotalPages * 100; } }
    
    public override string ToString()
    {
      return String.Format(
              "ActivePage: {0}, ActivePageIndex: {1}, LastPage: {2}, TotalPages: {3}",
               ActivePage,      ActivePageIndex,      LastPage,      TotalPages);
    }
  }

  public class ConverterStateTypeConverter : TypeConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return CanConvertTo(destinationType) ? ((ConverterState)value).Serialize() : null;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return CanConvertFrom(value.GetType()) ? ConverterState.Deserialize((string) value) : null;
    }
  }

}