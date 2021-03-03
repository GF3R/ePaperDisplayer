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


let configure build =
    let rootFolder = "../backend/"
    let binFolder = rootFolder + "/**/bin"
    let objFolder = rootFolder + "/**/obj"
    let proj = rootFolder + "/**/*.*proj"
    let tempFolder = rootFolder + "/temp"
    let appDir = tempFolder + "/App"
    let webDir = appDir + "/Web"
    let backendSolutionName = "EPaper.Web.Core"
    let backendStartupName = backendSolutionName + ".WebHost"
    let frontendDir = "/../frontend"




    Target.initEnvironment ()

    Target.create "Clean" (fun _ -> !!binFolder ++ objFolder |> Shell.cleanDirs)

    Target.create "Build" (fun _ -> !!proj |> Seq.iter (DotNet.build id))

    Target.create
        "Generate-Api-Client"
        (fun _ ->
            let configFile = "eplan.nswag"

            let variables =
                [ "swaggerDocument", tempFolder + "/swagger.json"
                  "startupAssembly", webDir + "/" + backendStartupName + ".dll"
                  "outputFile", frontendDir + "/src/app/core/api.ts" ]

            Library.NSwag(configFile, variables))

    Target.create "All" ignore

    "Clean" ==> "Build" ==> "All"

    Target.runOrDefault "All"
