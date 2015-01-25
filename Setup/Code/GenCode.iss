[Code]

function AppLanguageName : String;
begin
  Result := 'English';
  case ExpandConstant('{language}') of
    #emit LanguageSelector
  end;
end;

[/Code]
