[Code]

// Update support.

function IsUpdate : Boolean;
begin
    Result := Utils_HasParam('update');
end;

var
  IsUserPortable: Boolean;

function IsPortable : Boolean;
begin
    Result := IsUserPortable or Utils_HasParam('portable');
end;

function IsInstallable : Boolean;
begin
    Result := not IsPortable;
end;

[/Code]
