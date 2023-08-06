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
    |> Seq.iter (
        DotNet.test (fun p ->
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
