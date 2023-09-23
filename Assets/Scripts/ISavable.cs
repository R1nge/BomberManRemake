using System.Threading.Tasks;

public interface ISavable
{
    Task Save();
    Task Load();
}