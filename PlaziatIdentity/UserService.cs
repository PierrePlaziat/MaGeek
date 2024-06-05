using PlaziatCore;
    
namespace PlaziatIdentity
{
    public class UserService
    {

        private UserDbManager db = new UserDbManager();

        public bool RegisterUser(string name, string pass, string mail)
        {
            if (!name.IsValidUsername()) return false;
            if (!pass.IsValidPassword()) return false;
            if (!mail.IsValidMailAddr()) return false;
            using var context = db.GetContext();
            if (context.Users.Any(user => user.Name == name)) return false;
            if (context.Users.Any(user => user.Mail == mail)) return false;
            context.Users.Add(new User { Name = name, Password = PlaziatCore.Encryption.Hash(pass) });
            context.SaveChanges();
            return true;
        }

        public string? IdentifyUser(string name, string pass)
        {
            if (!name.IsValidUsername()) return null;
            if (!pass.IsValidPassword()) return null;
            using var context = db.GetContext();
            User? user = context.Users.Where(x => x.Name == name).FirstOrDefault();
            if (user == null) return null;
            if (PlaziatCore.Encryption.Hash(user.Password) != PlaziatCore.Encryption.Hash(pass)) return null;
            return new Token(user).Value;
        }

        public bool CheckToken(string token)
        {
            return new Token(token).IsValid();
        }

        class Token
        {

            string value;
            public string Value
            {
                get { return value; }
            }

            public Token(string value)
            {
                this.value = value;
            }

            public Token(User user)
            {
                value = PlaziatCore.Encryption.Encrypt(user.Name + '|' + DateTime.Now.AddMinutes(2).ToString());
            }

            public bool IsValid()
            {
                DateTime limit = DateTime.Parse(PlaziatCore.Encryption.Decrypt(value).Split('|')[1]);
                return DateTime.Now > limit;
            }
        }

    }

}
