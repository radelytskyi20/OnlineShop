using OnlineShop.Ui.Models.Security;

namespace OnlineShop.Ui.Abstractions.Interfaces
{
    public interface ILoginStatusManager
    {
        Task<bool> LogIn(string userName, string password);
        Task<bool> LogOut();
        LoginStatus LoginStatus { get; }
        event EventHandler LoginStatusHasChanged;
    }
}
