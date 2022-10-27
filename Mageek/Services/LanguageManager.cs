using System.Linq;

namespace MaGeek.Services
{
    public class LanguageManager
    {

        public string GetForeignLanguage()
        {
            var p = App.Database.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any())
            {
                return p.FirstOrDefault().ParamValue;
            }
            else
            {
                App.Database.Params.Add(new Entities.Param() { ParamValue = "French", ParamName = "ForeignLanguage" });
                App.Database.SaveChanges();
                return "French";
            }
        }

        public void SetForeignLanguage(string value)
        {
            var p = App.Database.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any()) App.Database.Params.Remove(p.FirstOrDefault());
            App.Database.Params.Add(new Entities.Param() { ParamValue = value, ParamName = "ForeignLanguage" });
            App.Database.SaveChanges();
            App.Restart();
        }
    }
}