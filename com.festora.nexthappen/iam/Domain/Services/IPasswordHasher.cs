namespace com.festora.nexthappen.iam.Domain.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
