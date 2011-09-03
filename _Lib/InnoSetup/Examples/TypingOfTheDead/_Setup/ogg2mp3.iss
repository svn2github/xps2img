#ifndef __OGG2MP3_ISS__
#define __OGG2MP3_ISS__

#define Ogg2Mp3Bat      "ogg2mp3.bat"
#define Ogg2Mp3BatPath  "{app}\" + Ogg2Mp3Bat

[Code]

const batchError = 'if errorlevel 1 exit' + #13#10;
const template =
    'cd "{app}\sound\%s"' + <Const_NewLine> +
    batchError +
    '"{app}\oggdec.exe" "*.ogg"' + <Const_NewLine> +
    batchError +
    'del /f "*.ogg"' + <Const_NewLine> + <Const_NewLine>;
const cls = 'cls' + <Const_NewLine>;

procedure CreateOgg2Mp3Bat;
    var soundFolders: array of string;
    var i: integer;
    var script: string;
begin

    soundFolders := [
        'bgm',
        'SE\COMM3',
        'SE\COMMON',
        'SE\COMMON2',
        'SE\DAMEGE_Ga',
        'SE\DAMEGE_Jms',
        'SE\DC_SE',
        'SE\ETC',
        'SE\STAGE1_SE',
        'SE\STAGE2_SE',
        'SE\STAGE3_SE',
        'SE\STAGE4_SE',
        'SE\STAGE5_SE',
        'SE\STAGE6_SE',
        'SE\START_COIN',
        'SE\TYPOD_Y',
        'voice\com',
        'voice\org',
        'voice\st1',
        'voice\st2',
        'voice\st3',
        'voice\st4',
        'voice\st5',
        'voice\st6'
    ];

    script := '@echo off' + <Const_NewLine> + <Const_NewLine>;
    
    for i := 0 to GetArrayLength(soundFolders) - 1 do
    begin
        script := script + Format(template, [ soundFolders[i] ]);
    end;
    script := script + cls;
    SaveStringToFile(
        ExpandConstant('<Ogg2Mp3BatPath>'),
        ExpandConstant(script),
        False
    );
end;

[/Code]

#endif
