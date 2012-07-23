[Code]

// Update support.

function HasParam(paramName: String) : Boolean;
var
    i: Integer;
    fullParamName: String;
begin
    fullParamName := '/' + paramName;
    Result := False;
    for i := 1 to ParamCount do
    begin
        Result := LowerCase(ParamStr(i)) = fullParamName;
        if Result then Exit;
    end;
end;

function IsUpdate : Boolean;
begin
    Result := HasParam('update');
end;

var
  IsUserPortable: Boolean;

function IsPortable : Boolean;
begin
    Result := IsUserPortable or HasParam('portable');
end;

function IsInstallable : Boolean;
begin
    Result := not IsPortable;
end;

[/Code]
