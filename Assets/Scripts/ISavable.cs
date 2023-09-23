using System.Threading.Tasks;

public interface ISavable
{
    //TODO: use playfabs c# api to be able to use Tasks
    void Save();
    Task Load();
}