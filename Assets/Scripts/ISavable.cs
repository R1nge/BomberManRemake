using Cysharp.Threading.Tasks;

public interface ISavable
{
    UniTask Save();
    UniTask Load();
}