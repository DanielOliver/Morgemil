open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

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
    |> Seq.iter (DotNet.build (fun c -> { c with Configuration = DotNet.Release })))

Target.create "Test" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (fun proj ->
        CreateProcess.fromRawCommandLine
            "dotnet"
            ("test /p:AltCover=true /p:AltCoverAssemblyExcludeFilter=\"xunit*\" /p:AltCoverReport=\"./coverage.xml\" --logger \"trx;LogFileName=testresults.trx\"")
        |> CreateProcess.withWorkingDirectory (Path.getDirectory proj)
        |> CreateProcess.ensureExitCode
        |> Proc.run
        |> ignore))

Target.create "Report" (fun _ ->
    CreateProcess.fromRawCommandLine
        "dotnet"
        "reportgenerator -reports:\"**/coverage.xml\" -targetdir:\"coveragereport\" -reporttypes:\"HTML;Cobertura\" -assemblyfilters:\"+Morgemil.*;-Morgemil.*.Tests\""
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore)

Target.create "All" ignore

let dependencies = [ "Clean" ==> "Build" ==> "Test" ==> "Report" ==> "All" ]

[<EntryPoint>]
let program argv =


    Target.runOrDefaultWithArguments (
        match argv.Length with
        | 0 -> "All"
        | _ -> argv[0]
    )

    0


// nuget Fake.DotNet.Cli
// nuget Fake.IO.FileSystem
// nuget Fake.Core.Target
