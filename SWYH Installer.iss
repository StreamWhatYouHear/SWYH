#define MyAppName "Stream What You Hear (SWYH)"
#define MyAppPublisher "Sebastien.warin.fr"
#define MyAppURL "http://www.streamwhatyouhear.com"
#define MyAppExeName "SWYH.exe"

; Updated by the build server
#define ShortVersion "1.0"
#define FullVersion "1.0-dev"

[Setup]
AppId={{5FBEA9D3-668E-4B88-BF6C-E1BCF441ECFD}
AppName={#MyAppName}
AppVersion={#ShortVersion}
AppVerName={#MyAppName} {#FullVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Stream What You Hear
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=SWYH\bin\Release
OutputBaseFilename=SWYH_{#FullVersion}
SetupIconFile=SWYH\Resources\Icons\swyh128.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "SWYH\bin\Release\SWYH.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\libmp3lame.32.dll"; DestDir: "{app}"; Flags: ignoreversion   
Source: "SWYH\bin\Release\libmp3lame.64.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\NAudio.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\NAudio.Lame.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\SWYH.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\UPnP.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\UPNP_AV.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\UPNPAV_RendererStack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SWYH\bin\Release\UPNPAVCDSML.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
