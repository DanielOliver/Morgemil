namespace Morgemil.GameEngine

open SadConsole.Input


type ScenarioSelectorConsole(scenarios: string list, chooseScenario: (string -> unit)) =
    inherit SadConsole.Console(40, 40)

    do
        let cursor = base.Cursor

        scenarios
        |> Seq.iteri (fun index scenarioName ->
            cursor.Print(sprintf "%-5i | %s" index scenarioName).NewLine() |> ignore)

    override this.ProcessKeyboard(info: Keyboard) : bool =
        if info.KeysPressed.Count = 0 then
            base.ProcessKeyboard(info)
        else
            chooseScenario (info.KeysPressed[0].Character.ToString())
            true
