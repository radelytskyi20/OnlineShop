using OnlineShop.Ui.Models.Common;

namespace OnlineShop.Ui.Abstractions.Interfaces
{
    public interface IUsersManager
    {
        Task UpdateAsync(User user);
        event EventHandler? UserEntityHasChanged;
    }
}
