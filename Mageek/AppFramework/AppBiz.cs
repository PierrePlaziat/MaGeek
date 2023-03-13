using MaGeek.AppBusiness;
using MaGeek.AppData;

namespace MaGeek.AppFramework
{

    public class AppBiz
    {

        public SqliteDbManager DB { get; }
        public MageekImporter Importer { get; }
        public MageekUtils Utils { get; }

        public AppBiz()
        {
            DB = new SqliteDbManager();
            Importer = new MageekImporter();
            Utils = new MageekUtils();
        }

    }

}
