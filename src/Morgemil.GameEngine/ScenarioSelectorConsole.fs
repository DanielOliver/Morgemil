namespace Morgemil.GameEngine

open SadConsole.Input


type ScenarioSelectorConsole(scenarios: string list, chooseScenario: string -> unit) =
    inherit SadConsole.Console(40, 40)

    do
        let cursor = base.Cursor

        scenarios
        |> Seq.iteri (fun index scenarioName ->
            cursor.Print $"%-5i{index} | %s{scenarioName}".NewLine() |> ignore)

    override this.ProcessKeyboard(info: Keyboard) : bool =
        if info.KeysPressed.Count = 0 then
            base.ProcessKeyboard(info)
        else if info.IsKeyPressed Keys.D0 then
            chooseScenario (info.KeysPressed[0].Character.ToString())
            true
        else
            false
