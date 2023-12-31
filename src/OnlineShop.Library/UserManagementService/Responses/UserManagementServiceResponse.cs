namespace OnlineShop.Library.UserManagementService.Responses
{
    public class UserManagementServiceResponse<T>
    {
        public T Payload { get; set; } //сохраняется информация о всех полях объекта

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
