# TESTS = Core.Tests Data.Tests Math.Tests Utility.Tests
TEST_DIRS := $(wildcard src/Morgemil.Data.Tests)

all: report
tools: 
	dotnet tool restore
.PHONY : clean
clean :
	-rm -rf coveragereport
	-del /q coveragereport 2>nul
	-rm -rf build
	-del /q build 2>nul
	dotnet clean src
build: clean
	dotnet build src --configuration Release
test: tools build
	dotnet test src /p:AltCoverForce=true /p:AltCover=true --logger "trx;LogFileName=testresults.trx"
$(TEST_DIRS):
	cd $@
	dotnet reportgenerator -reports:"**/coverage.xml" -targetdir:"coveragereport" -reporttypes:"HTML;Cobertura" -assemblyfilters:"+Morgemil.*;-Morgemil.*.Tests"
# .PHONY: report $(TEST_DIRS)
report: test
	dotnet reportgenerator -reports:"**/coverage.xml" -targetdir:"coveragereport" -reporttypes:"HTML;Cobertura" -assemblyfilters:"+Morgemil.*;-Morgemil.*.Tests"

