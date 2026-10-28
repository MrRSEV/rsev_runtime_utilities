namespace RSEV.Utilities.Session;

public interface ISessionManager
{
    SessionContext Create(string subject);
    bool IsValid(SessionContext context);
}
