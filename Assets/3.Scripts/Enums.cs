public enum Languages { KR, EN }

public enum Chapters { Lobby, Class535, Dormitory, PostionClass, Library, TeachersRoom, Totorial }

public enum PuzzleColor { white, red, Orange, Violet, Skyblue, Pink, green, blue, black, grey, yellow, magenta, cyan }
//Orange,Violet,Skyblue,Pink

#region Puzzles
public enum PuzzleType
{
    None,
    Slot,
    Dial,
    Keypad,
    ColorButton
}

public enum InteractorType
{
    None,
    Single,
    Multiple
}

public enum Axis
{
    XAxis,
    YAxis,
    ZAxis
}

public enum InteractableType
{
    None,
    Knob,
    Push,
    Press
}


#endregion

#region Data
public enum DialogType { Story, Hint, None }

public enum ObjectState { Exist, Used, Inventory, None } // 월드에 존재, 기믹으로 사용되어 사라짐, 인벤토리로 이동 ,None
#endregion