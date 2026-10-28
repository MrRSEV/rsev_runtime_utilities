namespace RSEV.Utilities.Session;

public interface IAccessTokenProvider
{
    AccessToken Create(string subject, TimeSpan lifetime);
    bool Validate(AccessToken token);
}
