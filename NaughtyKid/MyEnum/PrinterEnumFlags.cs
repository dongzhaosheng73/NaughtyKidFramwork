using System;

namespace NaughtyKid.MyEnum
{
    [Flags]
    public enum PrinterEnumFlags
    {
        PrinterEnumDefault = 0x00000001,
        PrinterEnumLocal = 0x00000002,
        PrinterEnumConnections = 0x00000004,
        PrinterEnumFavorite = 0x00000004,
        PrinterEnumName = 0x00000008,
        PrinterEnumRemote = 0x00000010,
        PrinterEnumShared = 0x00000020,
        PrinterEnumNetwork = 0x00000040,
        PrinterEnumExpand = 0x00004000,
        PrinterEnumContainer = 0x00008000,
        PrinterEnumIconmask = 0x00ff0000,
        PrinterEnumIcon1 = 0x00010000,
        PrinterEnumIcon2 = 0x00020000,
        PrinterEnumIcon3 = 0x00040000,
        PrinterEnumIcon4 = 0x00080000,
        PrinterEnumIcon5 = 0x00100000,
        PrinterEnumIcon6 = 0x00200000,
        PrinterEnumIcon7 = 0x00400000,
        PrinterEnumIcon8 = 0x00800000,
        PrinterEnumHide = 0x01000000
    }
}
