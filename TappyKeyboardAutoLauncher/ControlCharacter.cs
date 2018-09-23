using System;

enum ControlCharacter
{
    CRLF,
    TAB,
    INVALID
}

static class ControlCharacterMethods
{

    public static ControlCharacter FromString(String controlCharStr)
    {
        switch (controlCharStr.ToLower())
        {
            case "return":
                return ControlCharacter.CRLF;
                break;
            case "tab":
                return ControlCharacter.TAB;
                break;
            default:
                return ControlCharacter.INVALID;
                break;
        }
    }
}