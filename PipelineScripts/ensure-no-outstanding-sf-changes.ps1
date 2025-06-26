param(
    [Parameter(Mandatory = $true)][string]$IslnPath,
    [switch]$CheckDeviations,
    [switch]$ClearCachedModules
)

$intent_architect_user = $Env:INTENT_PACKAGER_USERNAME
$intent_architect_password = "";

if (-not $Env:INTENT_PACKAGER_PASSWORD) {
    $intent_architect_password = Read-Host '"INTENT_PACKAGER_PASSWORD" environment variable is not set, please enter your Intent Architect password' -MaskInput
    $Env:INTENT_PACKAGER_PASSWORD = [System.Convert]::ToBase64String([System.Security.Cryptography.ProtectedData]::Protect([System.Text.Encoding]::Unicode.GetBytes($intent_architect_password), $null, "CurrentUser"))
    Write-Host
    Write-Host "To avoid having to type in your password every time you can set the INTENT_PACKAGER_PASSWORD environment on your operating system to the following value:"
    Write-Host $Env:INTENT_PACKAGER_PASSWORD
    Write-Host -NoNewLine 'Press any key to continue...';
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    Write-Host
}
else {
    $intent_architect_password = [System.Text.Encoding]::Unicode.GetString([System.Security.Cryptography.ProtectedData]::Unprotect([System.Convert]::FromBase64String($Env:INTENT_PACKAGER_PASSWORD), $null, "CurrentUser"))
}

if ($ClearCachedModules) {
    $folder = [System.IO.Path]::GetDirectoryName($IslnPath)
    $cacheFolder = [System.IO.Path]::Combine($folder, ".intent")
    Remove-Item -Path $cacheFolder -Recurse -Force
}

$params = @(
    "ensure-no-outstanding-changes"
    "--check-deviations"
    "--continue-on-error"
    "--", # Prevents any subsequent parameters starting with '@' from being interpreted as a response file, see https://intentarchitect.com/redirect/xwTSFCW9
    $intent_architect_user
    $intent_architect_password
    $IslnPath
)

if (-not $CheckDeviations) {
    $params = $params.Where{ $_ -ne "--check-deviations" }
}

intent-cli $params
# dotnet run --project D:\Dev\Intent\Intent.IntentArchitect\IntentArchitect.ElectronClient\Intent.SoftwareFactory.CLI\Intent.SoftwareFactory.CLI.csproj /p:WarningLevel=0 /p:NoWarn="NU5104" -- $params

exit $LASTEXITCODE
