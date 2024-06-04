using System.ComponentModel.DataAnnotations;

namespace PlaziatIdentity
{

    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public UserState State { get; set; } = 0;
        public UserType Type { get; set; } = 0;
    }

    public enum UserType
    {
        Admin = 666,
        User = 0,
    }

    public enum UserState
    {
        Pending = 0,
        Active = 1,
        Inactive = 2,
        Banned = 3,
    }

}
