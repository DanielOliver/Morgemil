dotnet reportgenerator -reports:"../../**/coverage.xml" -targetdir:"../coveragereport" -reporttypes:"HTML;Cobertura;Opencover" -assemblyfilters:"+Morgemil.*,-Morgemil.*.Tests"

dotnet test /p:AltCoverForce=true /p:AltCover=true

dotnet test /p:AltCoverForce=true /p:AltCover=true --logger "trx;LogFileName=testresults.trx"
