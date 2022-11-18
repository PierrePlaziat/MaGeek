using System.Linq;

namespace MaGeek
{

    public class LanguageManager
    {

        public string GetForeignLanguage()
        {
            var p = App.DB.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any())
            {
                return p.FirstOrDefault().ParamValue;
            }
            else
            {
                App.DB.Params.Add(new Entities.Param() { ParamValue = "French", ParamName = "ForeignLanguage" });
                App.DB.SafeSaveChanges();
                return "French";
            }
        }

        public void SetForeignLanguage(string value)
        {
            var p = App.DB.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any()) App.DB.Params.Remove(p.FirstOrDefault());
            App.DB.Params.Add(new Entities.Param() { ParamValue = value, ParamName = "ForeignLanguage" });
            App.DB.SafeSaveChanges();
            App.Restart();
        }

    }

}