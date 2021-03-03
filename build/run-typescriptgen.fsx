#load "library.fsx" // TODO: Extract to Practices.Build
#load "build.fsx"

open Fake.Core.TargetOperators

Build.configure {
    BuildDir = __SOURCE_DIRECTORY__
    TestsConnectionString = ""
    TestsAxiomaServiceUrl = ""
}

"Clean"
    ==> "Build-Backend"
    ==> "Generate-Api-Client"
    |> Library.Run

#quit