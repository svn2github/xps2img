using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace Xps2Img
{
  public class Converter: IDisposable
  {
    private readonly XpsDocument xpsDocument;
    private readonly DocumentPaginator documentPaginator;

    public ConverterState ConverterState { get; set; }

    private Converter(string xpsFileName)
    {
      xpsDocument = new XpsDocument(xpsFileName, FileAccess.Read, CompressionOption.NotCompressed);
      // ReSharper disable PossibleNullReferenceException
      documentPaginator = xpsDocument.GetFixedDocumentSequence().DocumentPaginator;
      // ReSharper restore PossibleNullReferenceException
      ConverterState = new ConverterState();
    }
    
    public int PageCount { get { return documentPaginator.PageCount; } }

    public static Converter Create(string xpsFileName)
    {
      return new Converter(xpsFileName);
    }
    
    public class ProgressEventArgs: EventArgs
    {
      public ProgressEventArgs(string fullFileName, ConverterState converterState)
      {
        FullFileName = fullFileName;
        ConverterState = converterState;
      }

      public readonly string FullFileName;
      public readonly ConverterState ConverterState;
    }
    
    public delegate void ProgressDelegate(ProgressEventArgs args);
    
    public event ProgressDelegate OnProgress;

    public class Parameters
    {
      public bool Test { get; set; }
      public int StartPage { get; set; }
      public int EndPage { get; set; }
      public ImageType ImageType { get; set; }
      public ImageOptions ImageOptions { get; set; }
      public Size? RequiredSize { get; set; }
      public int Dpi { get; set; }
      public string OutputDir { get; set; }
      public string BaseImageName { get; set; }
      public int? MemoryLimit { get; set; }
    }

    public void Convert(Parameters parameters)
    {
      var xpsFileName = xpsDocument.Uri.OriginalString;
      
      var numberFormat = PageCount.GetNumberFormat();

      if(parameters.BaseImageName == null)
      {
        parameters.BaseImageName = Path.GetFileNameWithoutExtension(xpsFileName) + '-';
      }
      
      if (String.IsNullOrEmpty(parameters.OutputDir))
      {
        parameters.OutputDir = Path.Combine(Path.GetDirectoryName(xpsFileName), Path.GetFileNameWithoutExtension(xpsFileName));
      }

      if (!ConverterState.HasPageCount)
      {
        ConverterState.SetLastAndTotalPages(parameters.EndPage, PageCount);
      }

      var activeDir = parameters.OutputDir;
      if(!parameters.Test)
      {
        Directory.CreateDirectory(activeDir);
      }

      var memoryUsageCheck = new MemoryUsageChecker(parameters.MemoryLimit);

      for (var docPageNumber = parameters.StartPage; docPageNumber <= parameters.EndPage; docPageNumber++)
      {
        ConverterState.ActivePage = docPageNumber;
        
        memoryUsageCheck.Check();
        
        var fileName = Path.Combine(activeDir, parameters.BaseImageName + String.Format(numberFormat, docPageNumber));
        
        ImageWriter.Write(
          !parameters.Test,
          fileName,
          parameters.ImageType,
          parameters.ImageOptions,
          GetPageBitmap(documentPaginator, docPageNumber-1, parameters),
          fullFileName => { if (OnProgress != null) { ConverterState.ActivePageIndex++; OnProgress(new ProgressEventArgs(fullFileName, ConverterState)); } });          
      }
    }

    private static RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, Parameters parameters)
    {
      const double dpiConst = 96.0;

      double dpi = parameters.Dpi;

      var size = parameters.RequiredSize ?? new Size();

      Func<int, bool> isSizeDefined = requiredSize => requiredSize > 0;
      Action<int, double> calcDpi = (requiredSize, pageSize) => { if (isSizeDefined(requiredSize)) { dpi = (requiredSize / pageSize) * dpiConst; } };
     
      using (var page = documentPaginator.GetPage(pageNumber))
      {
        if (!size.IsEmpty)
        {
          var portrait = page.Size.Height >= page.Size.Width;

          if (portrait || !isSizeDefined(size.Width))
          {
            calcDpi(size.Height, page.Size.Height);
          }

          if (!portrait || !isSizeDefined(size.Height))
          {
            calcDpi(size.Width, page.Size.Width);
          }
        }

        var ratio = dpi / dpiConst;

        var bitmap = new RenderTargetBitmap((int) Math.Round(page.Size.Width*ratio),
                                            (int) Math.Round(page.Size.Height*ratio), dpi, dpi, PixelFormats.Pbgra32);
        bitmap.Render(page.Visual);
        
        return bitmap;
      }
    }
    
    public void Dispose()
    {
      xpsDocument.Close();
      GC.SuppressFinalize(this);
    }
  }
}