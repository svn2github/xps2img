[Code]

function AppLanguageName : String;
begin
  Result := DefaultAppLanguageName;
  case ExpandConstant('{language}') of
    #emit LanguageSelector
  end;
end;

[/Code]
