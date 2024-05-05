namespace Morgemil.Models

[<Record>]
[<ScenarioData>]
type Scenario =
    { BasePath: string
      Version: string
      Date: System.DateTime
      Name: string
      Description: string }
