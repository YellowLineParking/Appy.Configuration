namespace Appy.Configuration.IO;

public class StreamResult
{
    public StreamResult(string result, bool hasMore)
    {
        Result = result;
        HasMore = hasMore;
    }

    public string Result { get; }

    public bool HasMore { get; }
}