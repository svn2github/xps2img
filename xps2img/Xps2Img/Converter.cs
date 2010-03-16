using System;
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

    public void Convert(int startPage, int endPage, ImageType imageType, ImageOptions imageOptions, int dpi, string outputDir, string baseImageName)
    {
      var xpsFileName = xpsDocument.Uri.OriginalString;
      
      var numberFormat = PageCount.GetNumberFormat();

      if(baseImageName == null)
      {
        baseImageName = Path.GetFileNameWithoutExtension(xpsFileName) + '-';
      }
      
      if (String.IsNullOrEmpty(outputDir))
      {
        outputDir = Path.Combine(Path.GetDirectoryName(xpsFileName), Path.GetFileNameWithoutExtension(xpsFileName));
      }

      Directory.CreateDirectory(outputDir);

      if (!ConverterState.HasPageCount)
      {
        ConverterState.SetLastAndTotalPages(endPage, PageCount);
      }
      
      var activeDir = outputDir;

      for (var docPageNumber = startPage; docPageNumber <= endPage; docPageNumber++)
      {
        ConverterState.ActivePage = docPageNumber;

        var fileName = Path.Combine(activeDir, baseImageName + String.Format(numberFormat, docPageNumber));
        
        Directory.CreateDirectory(activeDir);

        ImageWriter.Write(
          fileName,
          imageType,
          imageOptions,
          GetPageBitmap(documentPaginator, docPageNumber-1, dpi),
          fullFileName => { if (OnProgress != null) { ConverterState.ActivePageIndex++; OnProgress(new ProgressEventArgs(fullFileName, ConverterState)); } });
      }
    }

    private static RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, double dpi)
    {
      using(var page = documentPaginator.GetPage(pageNumber))
      {
        var ratio = dpi/96.0;

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