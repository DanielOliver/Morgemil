namespace Morgemil.Models

[<RecordSerialization>]
type Scenario =
  { BasePath: string
    Version: string
    Date: System.DateTime
    Name: string
    Description: string
  }

