using PlaziatTools;
    
namespace PlaziatIdentity
{
    public class UserService : IUserService
    {

        private UserDbManager db = new UserDbManager(); 

        public bool RegisterUser(string name, string pass)
        {
            if (!name.IsValidUsername()) return false;
            if (!pass.IsValidPassword()) return false;
            //if (!mail.IsValidMailAddr()) return false;
            Logger.Log("Valid credentials...");
            using (var context = db.GetContext())
            {
                if (context.Users.Any(user => user.Name == name))
                {
                    Logger.Log("User already exists");
                    return false;
                }
                Logger.Log("Registering user...");
                //if (context.Users.Any(user => user.Mail == mail)) return false;
                context.Users.Add(new User { Name = name, Password = PlaziatTools.Encryption.Hash(pass) });
                context.SaveChanges();
            }
            using (var context = db.GetContext())
            {
                if (context.Users.Any(user => user.Name == name))
                {
                    Logger.Log("Success");
                    return true;
                }
                Logger.Log("Failure",LogLevels.Error);
                return true;
            }
        }

        public string? IdentifyUser(string name, string pass)
        {
            if (!name.IsValidUsername()) return null;
            if (!pass.IsValidPassword()) return null;
            User? user;
            using (var context = db.GetContext())
            {
               user = context.Users.Where(x => x.Name == name).FirstOrDefault();
            }
            if (user == null)  RegisterUser(name, pass);
            using (var context = db.GetContext())
            {
                user = context.Users.Where(x => x.Name == name).FirstOrDefault();
            }
            if (user == null)
            {
                Logger.Log("User doesnt exists");
                return null;
            }
            if (PlaziatTools.Encryption.Hash(user.Password) != pass)
            {
                Logger.Log("Wrong password");
                return null;
            }
            Logger.Log("Success");
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
                value = PlaziatTools.Encryption.Encrypt(user.Name + '|' + DateTime.Now.AddMinutes(2).ToString());
            }

            public bool IsValid()
            {
                DateTime limit = DateTime.Parse(PlaziatTools.Encryption.Decrypt(value).Split('|')[1]);
                return DateTime.Now > limit;
            }
        }

    }

}
