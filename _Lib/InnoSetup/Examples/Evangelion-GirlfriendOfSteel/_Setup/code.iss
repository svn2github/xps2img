#ifndef __CODE_ISS__
#define __CODE_ISS__

#define EvaIniFile  '{app}\EXEC\EVANGELIONforWIN95.ini'

[Code]

const iniTemplate =
    '[EVANGELIONforWIN95]' + <Const_NewLine> +
    'MEMO=copyright 1997(C)GAINAX' + <Const_NewLine> +
    'DELETEPROGRAM={app}\unins000.exe' + <Const_NewLine> +
    'CDROMChashENABLE=0' + <Const_NewLine> +
    'EXEC={app}\exec,0,0' + <Const_NewLine> +
    'GRP={app}\grp,0,0' + <Const_NewLine> +
    'MIDI={app}\midi,0,0' + <Const_NewLine> +
    'FACE={app}\face,0,0' + <Const_NewLine> +
    'TEXTBMP={app}\textbmp,0,0' + <Const_NewLine> +
    'WAVE00={app}\wave00,0,0' + <Const_NewLine> +
    'WAVE1={app}\wave,1,0' + <Const_NewLine> +
    'WAVE2={app}\wave,2,0' + <Const_NewLine> +
    'WAVE3={app}\wave,3,0' + <Const_NewLine> +
    'WAVE4={app}\wave,4,0' + <Const_NewLine> +
    'BMP00={app}\bmp00,0,0' + <Const_NewLine> +
    'BMP1={app}\bmp,1,0' + <Const_NewLine> +
    'BMP2={app}\bmp,2,0' + <Const_NewLine> +
    'BMP3={app}\bmp,3,0' + <Const_NewLine> +
    'BMP4={app}\bmp,4,0' + <Const_NewLine>;
    
procedure CreateINI;
    var iniContent: string;
begin
    iniContent := iniTemplate;
    StringChangeEx(iniContent, '{app}', GetShortName(ExpandConstant('{app}')), True);
    SaveStringToFile(
        ExpandConstant('<EvaIniFile>'),
        iniContent,
        False
    );
end;

procedure SetLang(lang: string);
begin
    SetIniString(
        'EVANGELIONforWIN95',
        'TEXTBMP',
        ExpandConstant(Format('{app}\textbmp\%s,0,0', [lang])),
        ExpandConstant('<EvaIniFile>'));
end;

[/Code]

#endif

