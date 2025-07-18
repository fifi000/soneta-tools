#nullable enable

using System.Data;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;


static readonly HttpClient Client = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(10),
};

static readonly string TempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))).FullName;

static string DownloadFolder => Directory.CreateDirectory(Path.Join(TempFolder, "Download")).FullName;

static string GetNameFromUrl(string url) => url.Split('\\', '/').Last();

static string[] GetZips(string version) =>
[
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net/Handel/Soneta_GreenMail24_od_wersji_2304.0.0_stary_podpis.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/CRM/Soneta.Email.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/CRM/Soneta.Outlook.AddIn.Setup.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/CRM/Soneta.Smsing.dll.2504.2.4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_e-Sklepy_konektor_od_wersji_2504_2_2.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_EDI_od_wersji_2504.2.3.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/Soneta_Faktura_RR_od_wersji_2412.2.2.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_GreenMail24_od_wersji_2410.1.1.0.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_Integrator_od_wersji_2410.1.1.2.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_Kurierzy_od_wersji_2504_2_2.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_Opłata_cukrowa_od_wersji_2412.0.0.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/soneta_WMS_konektor_od_wersji_2410_3_4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Handel/Soneta.eParagony_2504_0_0_1.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.CzasPracy2504.3.5.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.Edycja_Kalendarza_w_Pulpicie_Pracownika.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.ImportPłac2412.3.4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.KopiowaniePracownikow2504.2.4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.KosztyProjektow.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.PakietMobilnosciKierowcy.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.PracownicyEksportowi.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.PracownicyProkuratury.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.PracownicyUczelni2412.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.RozrachunkiFunduszyPozyczkowych.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.Symmetrical.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.Worksmile2504.2.4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Kadry_Place/Soneta.ZarządzanieOdzieżąRoboczą2504.2.4.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta_Analizy_MSExcel250435.zip",
    //"https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta_ElektroniczneWyciagiBankowe_2410.0.208.0_(2025 - 02 - 06).zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta.EksportyDekretowListPlac2412.4.6.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta.EksportyKsiegowe.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta.ImportyKsiegowe.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ksiegowosc/Soneta.JednostkiBudzetowe.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ogolnosystemowe/Soneta_ArchiwizatorAzure(Od wersji 2504).zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ogolnosystemowe/Soneta.PracaNaWieluBazach_Od_wersji_2412.2.2.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Ogolnosystemowe/SonetaBrother.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Workflow/Soneta.eDoreczenia_2412.2.4.7.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Workflow/Soneta.ePodpisKonektor_2504.0.2.6.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow-net8/Workflow/Soneta.IntegracjaOCR.dll.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow/enova_w_muzeum.zip",
    "https://download.enova.pl/instalatory/dodatki-do-modulow/Soneta.ZebraPrinter.zip",
    $"https://download.enova.pl/instalatory/net/Soneta.Standard.{version}.zip",
];

static async Task<string> DownloadExplorer(string version)
{
    string url = $"https://download.enova.pl/instalatory/net/enova365_{version}_instalator.exe";
    string fileName = GetNameFromUrl(url);
    string path = Path.Combine(DownloadFolder, fileName);

    var response = await Client.GetAsync(url);
    var content = await response.Content.ReadAsByteArrayAsync();
    await File.WriteAllBytesAsync(path, content);

    return path;
}

static async Task<string> InstallExplorer(string installerPath)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = installerPath,
            Arguments = "/S",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };

    process.Start();
    await process.WaitForExitAsync();

    WriteLine("Enter the explorer installation path:");
    string? installPath = ReadLine()?.Trim();

    if (string.IsNullOrEmpty(installPath) || !Directory.Exists(installPath))
    {
        return string.Empty;
    }

    return installPath;
}

static async Task<string> DownloadZip(string url)
{
    string fileName = GetNameFromUrl(url);
    string path = Path.Combine(DownloadFolder, fileName);

    var response = await Client.GetAsync(url);
    var content = await response.Content.ReadAsByteArrayAsync();
    await File.WriteAllBytesAsync(path, content);

    return path;
}

static string Unzip(string zipPath, bool removeZip = false)
{
    string folder = Path.Combine(DownloadFolder, Path.GetFileNameWithoutExtension(zipPath));
    using (var reader = ZipFile.OpenRead(zipPath))
    {
        reader.ExtractToDirectory(folder);
    }

    if (removeZip)
    {
        File.Delete(zipPath);
    }

    return folder;
}

static bool IsSonetaDll(string path)
{
    var info = FileVersionInfo.GetVersionInfo(path);
    string?[] names =
    [
        new FileInfo(path).Name,
        info.ProductName,
        info.CompanyName,
        info.InternalName,
        info.OriginalFilename,
        info.FileDescription,
    ];

    return names.Any(x => x?.ToLower().Contains("soneta") == true);
}

static string AggregateDlls(IEnumerable<string> paths, string output)
{
    string dlls = Directory.CreateDirectory(Path.Combine(output, "DLLs")).FullName;

    foreach (var path in paths)
    {
        if (!Directory.Exists(path))
        {
            continue;
        }
        foreach (var dll in Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories))
        {
            if (IsSonetaDll(dll))
            {
                File.Copy(dll, Path.Combine(dlls, Path.GetFileName(dll)), true);
            }
        }
        foreach (var pdb in Directory.EnumerateFiles(path, "*.pdb", SearchOption.AllDirectories))
        {
            File.Copy(pdb, Path.Combine(dlls, Path.GetFileName(pdb)), true);
        }
    }

    return dlls;
}

async static Task<string> DecompileIntoSolution(string path, string output, string version)
{
    string solutionDir = Directory.CreateDirectory(Path.Combine(output, $"Soneta - {version}")).FullName;

    var tasks = Directory.GetFiles(path, "*.dll").Select(x => Decompile(solutionDir, x));

    WriteLine("Decompiling...");
    await WhenAllChunked(tasks);
    WriteLine("Decompilation completed");

    WriteLine("Creating solution...");
    await CreateSolution(solutionDir);
    WriteLine("Solution created");

    WriteLine("Cleaning projects...");
    FixProjects(solutionDir);
    WriteLine("Projects cleaned"); ;

    return solutionDir;
}

async static Task Decompile(string solutionDir, string dll)
{
    await Process.Start("ilspycmd", $"-p -usepdb -o \"{Path.Join(solutionDir, Path.GetFileNameWithoutExtension(dll))}\" \"{dll}\"").WaitForExitAsync();
}

async static Task CreateSolution(string solutionDir)
{
    var originalDir = Directory.GetCurrentDirectory();
    Directory.SetCurrentDirectory(solutionDir);

    Process.Start("dotnet", "new sln -n Solution").WaitForExit();

    var projects = Directory.EnumerateFiles(".", "*.csproj", SearchOption.AllDirectories);

    foreach (var project in projects)
    {
        await Process.Start("dotnet", $"sln add \"{project}\"").WaitForExitAsync();
    }

    Directory.SetCurrentDirectory(originalDir);
}

async static Task WhenAllChunked(IEnumerable<Task> tasks, int size = 10)
{
    foreach (var chunk in tasks.Chunk(size))
    {
        await Task.WhenAll(chunk);
    }
}

static void FixProjects(string solution)
{
    var originalDir = Directory.GetCurrentDirectory();
    Directory.SetCurrentDirectory(solution);

    // fix internals visible to...
    // 
    foreach (var info in Directory.EnumerateFiles(".", "AssemblyInfo.cs", SearchOption.AllDirectories))
    {
        var content = File.ReadAllText(info);
        var newContent = Regex.Replace(content, @"^\[assembly: InternalsVisibleTo\(""([^,]+)(,.*)""\)\]", @"[assembly: InternalsVisibleTo(""$1"")]", RegexOptions.Multiline);

        if (content != newContent)
        {
            File.WriteAllText(info, newContent);
        }
    }

    // fix csprojs
    var projects = Directory.GetFiles(".", "*.csproj", SearchOption.AllDirectories);

    foreach (var project in projects)
    {
        var content = File.ReadAllText(project);
        var newContent = Regex.Replace(content, @"<Reference Include=""([^""]+)"">[\n\s]+<HintPath>([^<]+)</HintPath>[\n\s]+</Reference>", match =>
        {
            var value = match.Groups[1].Value;

            if (projects.Select(x => Path.GetFileNameWithoutExtension(x)).Any(x => x == value))
            {
                return $"<ProjectReference Include=\"..\\{value}\\{value}.csproj\"/>";
            }

            return match.Groups[0].Value;
        });

        if (content != newContent)
        {
            File.WriteAllText(project, newContent);
        }
    }


    Directory.SetCurrentDirectory(originalDir);
}

static void CopyDirectory(string sourceDir, string destDir)
{
    if (!Directory.Exists(destDir))
    {
        Directory.CreateDirectory(destDir);
    }

    foreach (var file in Directory.GetFiles(sourceDir))
    {
        var destFile = Path.Combine(destDir, Path.GetFileName(file));
        File.Copy(file, destFile, true);
    }

    foreach (var dir in Directory.GetDirectories(sourceDir))
    {
        var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
        CopyDirectory(dir, destSubDir);
    }
}

static void MoveOtherStuff(string solution, string installation, IEnumerable<string> otherFolders)
{
    // installation
    // 
    string[] folders =
    [
        "Aspx",
        "Dct",
        "Demo",
        "Patterns",
    ];

    foreach (var folder in folders)
    {
        CopyDirectory(Path.Combine(installation, folder), Path.Combine(solution, folder));
    }

    string[] files =
    [
        "appsettings.json"
    ];

    foreach (var file in files)
    {
        File.Copy(Path.Combine(installation, file), Path.Combine(solution, file), true);
    }

    // others
    // 

    var solutionFolders = Directory.GetDirectories(solution);

    foreach (var folder in otherFolders)
    {
        if (solutionFolders.FirstOrDefault(x => Path.GetDirectoryName(x) == Path.GetDirectoryName(folder)) is string solutionFolder)
        {
            foreach (var f in Directory.EnumerateFiles(folder).Where(x => !x.EndsWith(".dll")))
            {
                File.Copy(f, Path.Combine(solutionFolder, f));
            }

            foreach (var d in Directory.EnumerateDirectories(folder))
            {
                CopyDirectory(d, Path.Combine(solutionFolder, d));
            }
        }
    }
}

// 
// Main
// 

WriteLine(TempFolder);

if (Args.Count != 1)
{
    WriteLine("Usage: dupa.csx <version>");
    return;
}


string version = Args[0];

// download all zips and installer
WriteLine("Downloading files...");
var paths = await Task.WhenAll(GetZips(version).Select(async x =>
{
    var zipPath = await DownloadZip(x);
    return Unzip(zipPath, true);
}).Append(DownloadExplorer(version)));
WriteLine("Download and unzip completed");

string installPath = await InstallExplorer(paths.Last());

string output = Directory.CreateDirectory(Path.Combine(TempFolder, "output")).FullName;


// move dlls to one common folder
WriteLine("Aggregating DLLs...");
var zipPaths = paths.SkipLast(1);
string dlls = AggregateDlls(zipPaths.Append(installPath), output);
WriteLine($"Aggregated DLLs to {dlls}");


// create solution
string solution = await DecompileIntoSolution(dlls, output, version);

// move other folders
MoveOtherStuff(solution, installPath, Directory.GetDirectories(DownloadFolder));

WriteLine(TempFolder);