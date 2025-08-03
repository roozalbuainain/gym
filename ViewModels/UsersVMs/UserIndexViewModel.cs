namespace GYM_APP.ViewModels.UsersVMs
{
    public class UserIndexViewModel
    {
        public int UsersId { get; set; }
        public string UsersName { get; set; } = string.Empty;
        public string UsersEmail { get; set; } = string.Empty;
        public string UsersRole { get; set; } = string.Empty;
        public string? UsersPhoneNumber { get; set; }
        public DateTime? UsersJoinedAt { get; set; }
    }
}
