[Code]

const installCfgContent = 
	'install_path {app}'		+ <Const_NewLine> +
	'language english'			+ <Const_NewLine> +
	'resname_base {app}\res'	+ <Const_NewLine> +
	'load_path {app}'			+ <Const_NewLine> +
	'script_module_path {app}'	+ <Const_NewLine> +
	'movie_path {app}\movies'	+ <Const_NewLine>;

procedure SaveInstallCfg(installCfgPath: string);
begin
	SaveStringToFile(
		ExpandConstant(installCfgPath),
		ExpandConstant(installCfgContent),
		False
	);
end;

[/Code]
