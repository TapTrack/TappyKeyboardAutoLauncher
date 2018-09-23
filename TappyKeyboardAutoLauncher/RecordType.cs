using System;

enum RecordType
{
    TEXT,
    URI,
    MIME,
    EXTERNAL,
    INVALID
}

static class RecordTypeMethods
{

    public static String ToString(this RecordType recordType)
    {
        switch (recordType)
        {
            case RecordType.TEXT:
                return "t";
                break;
            case RecordType.URI:
                return "u";
                break;
            case RecordType.EXTERNAL:
                return "e";
                break;
            case RecordType.MIME:
                return "m";
            default:
                return null;
                break;
        }
    }

    public static RecordType FromString(String recordTypeStr)
    {
        switch (recordTypeStr.ToLower())
        {
            case "t":
                return RecordType.TEXT;
                break;
            case "u":
                return RecordType.URI;
                break;
            case "e":
                return RecordType.EXTERNAL;
                break;
            case "m":
                return RecordType.MIME;
                break;
            default:
                return RecordType.INVALID;
                break;
        }
    }
}