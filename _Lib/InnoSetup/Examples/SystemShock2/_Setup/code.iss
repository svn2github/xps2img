[Code]

const installCfgContent = 
	'install_path {app}'		+ <Const_NewLine> +
	'cd_path 8:'				+ <Const_NewLine> + // Any character. Required.
	'language english'			+ <Const_NewLine> +
	'resname_base {app}\res'	+ <Const_NewLine> +
	'load_path {app}'			+ <Const_NewLine> +
	'script_module_path {app}'	+ <Const_NewLine> +
	'movie_path {app}\cutscenes'+ <Const_NewLine>;

procedure SaveInstallCfg(installCfgPath: string);
begin
	SaveStringToFile(
		ExpandConstant(installCfgPath),
		ExpandConstant(installCfgContent),
		False
	);
end;

[/Code]
