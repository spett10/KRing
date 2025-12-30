namespace KRingCore.Core.Interfaces
{
    public interface IPasswordRule
    {
        int Apply(string password);
    }
}
