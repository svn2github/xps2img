[Code]

function BooleanToInteger(b: Boolean): Integer;
begin
  case b of
    True:  Result := 1;
    False: Result := 0;
  end;
end;

[/Code]
