namespace Morgemil.Models

[<Record>]
[<ScenarioStaticData>]
type Scenario =
    { BasePath: string
      Version: string
      Date: System.DateTime
      Name: string
      Description: string }
