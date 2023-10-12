namespace EvolveJourneyLog.Core.Repositories.Models;

public interface ISaveResponse { } //Improve in future - something like Either/Result datatypes modelled in C# can be quite nice.

public class SaveSuccess : ISaveResponse
{
    public int SaveId { get; }

    public SaveSuccess(int saveId)
    {
        SaveId = saveId;
    }
}

public class SaveFailure : ISaveResponse
{
    public SaveResult Result { get; }

    public SaveFailure(SaveResult result)
    {
        Result = result;
    }
}
