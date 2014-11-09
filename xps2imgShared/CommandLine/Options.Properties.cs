using System.ComponentModel;

using CommandLine;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        [Browsable(false)] public string PropNameSrcFile { get { return ReflectionUtils.GetPropertyName(() => SrcFile); } }
        [Browsable(false)] public string PropNameOutDir { get { return ReflectionUtils.GetPropertyName(() => OutDir); } }
        [Browsable(false)] public string PropNamePostAction { get { return ReflectionUtils.GetPropertyName(() => PostAction); } }
        [Browsable(false)] public string PropNamePages { get { return ReflectionUtils.GetPropertyName(() => Pages); } }
        [Browsable(false)] public string PropNameFileType { get { return ReflectionUtils.GetPropertyName(() => FileType); } }
        [Browsable(false)] public string PropNameJpegQuality { get { return ReflectionUtils.GetPropertyName(() => JpegQuality); } }
        [Browsable(false)] public string PropNameTiffCompression { get { return ReflectionUtils.GetPropertyName(() => TiffCompression); } }
        [Browsable(false)] public string PropNameRequiredSize { get { return ReflectionUtils.GetPropertyName(() => RequiredSize); } }
        [Browsable(false)] public string PropNameDpi { get { return ReflectionUtils.GetPropertyName(() => Dpi); } }
        [Browsable(false)] public string PropNameImageName { get { return ReflectionUtils.GetPropertyName(() => ImageName); } }
        [Browsable(false)] public string PropNameFirstPageIndex { get { return ReflectionUtils.GetPropertyName(() => FirstPageIndex); } }
        [Browsable(false)] public string PropNamePrelimsPrefix { get { return ReflectionUtils.GetPropertyName(() => PrelimsPrefix); } }
        [Browsable(false)] public string PropNameProcessorsNumber { get { return ReflectionUtils.GetPropertyName(() => ProcessorsNumber); } }
        [Browsable(false)] public string PropNameProcessPriority { get { return ReflectionUtils.GetPropertyName(() => ProcessPriority); } }
        [Browsable(false)] public string PropNameCpuAffinity { get { return ReflectionUtils.GetPropertyName(() => CpuAffinity); } }
        [Browsable(false)] public string PropNameIgnoreExisting { get { return ReflectionUtils.GetPropertyName(() => IgnoreExisting); } }
        [Browsable(false)] public string PropNameIgnoreErrors { get { return ReflectionUtils.GetPropertyName(() => IgnoreErrors); } }
        [Browsable(false)] public string PropNameTest { get { return ReflectionUtils.GetPropertyName(() => Test); } }
    }
}
