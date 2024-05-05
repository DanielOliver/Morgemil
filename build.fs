open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.DotNet.Testing
open Fake.Testing


let execContext = Context.FakeExecutionContext.Create false "build.fsx" []
Context.setExecutionContext (Context.RuntimeContext.Fake execContext)

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "src/**/TestResults"
    ++ "coveragereport"
    ++ "artifacts"
    |> Shell.cleanDirs

    !! "src/**/coverage.xml*" |> File.deleteAll)


// The below error happens on Ubuntu image in GitHub Actions if not using the MSBuildParams below. I don't know why.
//
// Starting task 'DotNet:build': /home/runner/work/Morgemil/Morgemil/src/Morgemil.Console/Morgemil.Console.fsproj
// > "/usr/bin/mono" --version (In: false, Out: true, Err: true)
// /home/runner/work/Morgemil/Morgemil> "dotnet"  msbuild --version --nologo (In: false, Out: true, Err: true)
// 17.9.8.16306
// /home/runner/work/Morgemil/Morgemil> "dotnet"  build /home/runner/work/Morgemil/Morgemil/src/Morgemil.Console/Morgemil.Console.fsproj --configuration Release /clp:ForceConsoleColor /bl:/tmp/tmpBwfi72.tmp.binlog (In: false, Out: false, Err: false)
// MSBuild version 17.9.8+b34f75857 for .NET
//
// Unhandled exception. Fake.Core.BuildFailedException: Target 'Build' failed.
//  ---> System.AggregateException: One or more errors occurred. (Value cannot be null. (Parameter 'format'))
//  ---> System.ArgumentNullException: Value cannot be null. (Parameter 'format')
//    at System.ArgumentNullException.Throw(String paramName)
//    at System.String.FormatHelper(IFormatProvider provider, String format, ReadOnlySpan`1 args)
//    at System.String.Format(String format, Object[] args)
//    at Microsoft.Build.Logging.StructuredLogger.BuildEventArgsReader.FormatResourceStringIgnoreCodeAndKeyword(String resource, String[] arguments) in C:\MSBuildStructuredLog\src\StructuredLogger\BinaryLogger\BuildEventArgsReader.Viewer.cs:line 198
// ...
// Error: Process completed with exit code 134.

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (
        DotNet.build (fun c ->
            if Environment.isLinux then
                { c with
                    NoLogo = true
                    MSBuildParams =
                        { c.MSBuildParams with
                            BinaryLoggers = None
                            DisableInternalBinLog = true }
                    Configuration = DotNet.Release }
            else
                { c with
                    Configuration = DotNet.Release })
    ))

Target.create "Test" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (
        DotNet.test (fun p ->
            if Environment.isLinux then
                { p with
                    NoLogo = true
                    MSBuildParams =
                        { p.MSBuildParams with
                            BinaryLoggers = None
                            DisableInternalBinLog = true }
                    Configuration = DotNet.BuildConfiguration.Release }
            else
                { p with
                    Configuration = DotNet.BuildConfiguration.Release }
            |> Coverlet.withDotNetTestOptions (fun p ->
                { p with

                    Output = "TestResults/coverage.xml"
                    Include = [ "Morgemil.*", "*" ]
                    Exclude = [ "*.Tests?", "*" ]
                    OutputFormat = [ Coverlet.OutputFormat.OpenCover ] }))
    ))

Target.create "Report" (fun _ ->
    !! "**/coverage.xml"
    |> Seq.toList
    |> ReportGenerator.generateReports (fun p ->
        { p with
            ToolType = ToolType.CreateLocalTool()
            ReportTypes = [ ReportGenerator.ReportType.Cobertura; ReportGenerator.ReportType.Html ]
            TargetDir = "./coveragereport/" }))

Target.create "All" ignore

let dependencies = [ "Clean" ==> "Build" ==> "Test" ==> "Report" ]

[<EntryPoint>]
let program argv =
    Target.runOrDefaultWithArguments (
        match argv.Length with
        | 0 -> "Report"
        | _ -> argv[0]
    )

    0


// nuget Fake.DotNet.Cli
// nuget Fake.IO.FileSystem
// nuget Fake.Core.Target
