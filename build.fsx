#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
  !! "src/**/bin"
  ++ "src/**/obj"
  ++ "src/**/TestResults"
  ++ "coveragereport"
  ++ "artifacts"
  |> Shell.cleanDirs 

  !! "src/**/coverage.xml*"
  |> File.deleteAll
)

Target.create "Build" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter (DotNet.build (fun c ->
        { c with
            Configuration = DotNet.Release
        })
  )
)

Target.create "Test" (fun _ ->
  [ 
    "test"
    "src"
    "--configuration"
    "Release"
    "/p:AltCoverForce=true"
    "/p:AltCover=true"
    "--logger"
    "trx;LogFileName=testresults.trx"           
  ]
  |> CreateProcess.fromRawCommand "dotnet" 
  |> CreateProcess.ensureExitCode
  |> Proc.run
  |> ignore
)

Target.create "Report" (fun _ ->
  [ 
    "reportgenerator"
    "--configuration Release"
    "-reports:**/coverage.xml"
    "-targetdir:coveragereport";
    "-reporttypes:\"HTML;Cobertura\"";
    "-assemblyfilters:\"+Morgemil.*;-Morgemil.*.Tests\""     
  ]
  |> CreateProcess.fromRawCommand "dotnet"
  |> CreateProcess.ensureExitCode
  |> Proc.run
  |> ignore
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Report"
  ==> "All"

Target.runOrDefault "All"
