using System;

/// <summary>
/// Base wafer log object. Only used for data storage.
/// </summary>
/// <remarks>
/// Do not use for manipulating log steps.
/// Initializations and Fields/Properties only.
/// </remarks>
[Serializable]
public class WaferStep
{
    public double Time;
    public string Tool;
    public byte Position;
    public string Message;

    public WaferStep(string tool, byte position)
    {
        Position = position;
        Tool = tool;
        Time = DateTime.UtcNow.ToEpoch();
        Message = "";
    }

    public WaferStep(string tool, string message)
    {
        Position = 0;
        Tool = tool;
        Time = DateTime.UtcNow.ToEpoch();
        Message = message;
    }

    public override string ToString()
    {
        return Message.Equals("")
            ? string.Format("{0:F0} Machine {1}, Position {2}", Time, Tool, Position)
            : string.Format("{0:F0} Machine {1}, {2}", Time, Tool, Message);
    }

    public static implicit operator string(WaferStep step)
    {
        return step.ToString();
    }
}