; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Project 1.27"
#define MyAppVersion "1.2.0.0"
#define MyAppPublisher "Project 1.27 Inc."
#define MyAppURL "https://github.com/TwosHusbandS/Project-127/"
#define MyAppExeName "Project 127 Launcher.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{2E862DB9-ABA7-4F67-A954-4DF9D0349CAA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=.
OutputBaseFilename=Project_127_Installer_V_1_2_0_0
SetupIconFile=..\Artwork\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\bin\x64\Release\UglyFiles\Antlr3.Runtime.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\cef.pak"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\cef_100_percent.pak"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\cef_200_percent.pak"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\cef_extensions.pak"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CefSharp.BrowserSubprocess.Core.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CefSharp.BrowserSubprocess.exe"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CefSharp.Core.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CefSharp.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CefSharp.Wpf.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\chrome_elf.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\CredentialManagement.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\d3dcompiler_47.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\devtools_resources.pak"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\ExpressionEvaluator.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\GameOverlay.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\GSF.Core.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\icudtl.dat"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\libcef.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\libEGL.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\libGLESv2.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\LICENSE"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\JUMPSCRIPT_LICENSE"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\Microsoft.Bcl.AsyncInterfaces.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\Microsoft.Bcl.AsyncInterfaces.xml"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\Project 1.27.exe"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\Project 1.27.exe.manifest"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\P127_Jumpscript.exe"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\SharpDX.Direct2D1.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\SharpDX.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\SharpDX.DXGI.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\snapshot_blob.bin"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\v8_context_snapshot.bin"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\UglyFiles\WpfAnimatedGif.dll"; DestDir: "{app}\UglyFiles"; Flags: ignoreversion
Source: "..\bin\x64\Release\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\LICENSE_JUMPSCRIPT"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\Project 127 Launcher.exe"; DestDir: "{app}"; Flags: ignoreversion

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

