using System;

namespace NaughtyKid.MyEnum
{
    //打印机状态 
    [Flags]
    internal enum PrinterStatus
    {
        PrinterStatusBusy = 0x00000200,
        PrinterStatusDoorOpen = 0x00400000,
        PrinterStatusError = 0x00000002,
        PrinterStatusInitializing = 0x00008000,
        PrinterStatusIoActive = 0x00000100,
        PrinterStatusManualFeed = 0x00000020,
        PrinterStatusNoToner = 0x00040000,
        PrinterStatusNotAvailable = 0x00001000,
        PrinterStatusOffline = 0x00000080,
        PrinterStatusOutOfMemory = 0x00200000,
        PrinterStatusOutputBinFull = 0x00000800,
        PrinterStatusPagePunt = 0x00080000,
        PrinterStatusPaperJam = 0x00000008,
        PrinterStatusPaperOut = 0x00000010,
        PrinterStatusPaperProblem = 0x00000040,
        PrinterStatusPaused = 0x00000001,
        PrinterStatusPendingDeletion = 0x00000004,
        PrinterStatusPrinting = 0x00000400,
        PrinterStatusProcessing = 0x00004000,
        PrinterStatusTonerLow = 0x00020000,
        PrinterStatusUserIntervention = 0x00100000,
        PrinterStatusWaiting = 0x20000000,
        PrinterStatusWarmingUp = 0x00010000
    }
}
