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

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (
        DotNet.build (fun c ->
            { c with
                Configuration = DotNet.Release })
    ))



Target.create "Test" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (fun proj ->
        // CreateProcess.fromRawCommandLine "dotnet" ("test --logger \"trx;LogFileName=testresults.trx\"")
        // |> CreateProcess.withWorkingDirectory (Path.getDirectory proj)
        // |> CreateProcess.ensureExitCode
        // |> Proc.run
        // |> ignore))

        DotNet.test
            (fun p ->
                { p with
                    Configuration = DotNet.BuildConfiguration.Release }
                |> Coverlet.withDotNetTestOptions (fun p ->
                    { p with

                        Output = "TestResults/coverage.xml"
                        Include = [ "Morgemil.*", "*" ]
                        Exclude = [ "*.Tests?", "*" ]
                        OutputFormat = [ Coverlet.OutputFormat.OpenCover ] }))
            proj))


Target.create "Report" (fun _ ->
    !! "**/coverage.xml"
    |> Seq.toList
    |> ReportGenerator.generateReports (fun p ->
        { p with
            ToolType = ToolType.CreateLocalTool()
            ReportTypes = [ ReportGenerator.ReportType.Cobertura; ReportGenerator.ReportType.Html ]
            TargetDir = "./coveragereport/" }))

Target.create "Test with coverage" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (fun proj ->
        CreateProcess.fromRawCommandLine
            "dotnet"
            ("test /p:AltCover=true /p:AltCoverAssemblyFilter=\"Morgemil*\" /p:AltCoverAssemblyExcludeFilter=\"xunit*\" /p:AltCoverReport=\"./coverage.xml\" --logger \"trx;LogFileName=testresults.trx\"")
        |> CreateProcess.withWorkingDirectory (Path.getDirectory proj)
        |> CreateProcess.ensureExitCode
        |> Proc.run
        |> ignore))

let report (_: TargetParameter) =
    CreateProcess.fromRawCommandLine
        "dotnet"
        "reportgenerator -reports:\"**/coverage.xml\" -targetdir:\"coveragereport\" -reporttypes:\"HTML;Cobertura\" -assemblyfilters:\"+Morgemil.*.dll;-Morgemil.*.Tests\""
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

// Target.create "Report" report

Target.create "Report with Coverage" report

Target.create "All" ignore

let dependencies =
    [ "Clean" ==> "Build"
      "Build" ==> "Test" ==> "Report"
      "Build" ==> "Test with Coverage" ==> "Report with Coverage" ]

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
