namespace Morgemil.GameEngine

open System
open SadConsole.Input


type ScenarioSelectorConsole(scenarios: string list, chooseScenario: (string -> unit)) =
    inherit SadConsole.Console(40, 40)

    do
        let cursor = base.Cursor
        cursor.IsVisible <- true

        scenarios
        |> Seq.iteri (fun index scenarioName ->
            cursor.Print(sprintf "%-5i | %s" index scenarioName).NewLine() |> ignore)


    override this.ProcessKeyboard(info: Keyboard) : bool =
        if info.KeysPressed.Count = 0 then
            false
        else
            chooseScenario (info.KeysPressed[0].Character.ToString())
            true

    override this.Update(timeElapsed: TimeSpan) = ()
