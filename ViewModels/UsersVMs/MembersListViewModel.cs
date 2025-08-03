namespace GYM_APP.ViewModels.UsersVMs
{
    public class MembersListViewModel
    {
        public List<UserIndexViewModel> Members { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalMembers { get; set; }
    }
}
