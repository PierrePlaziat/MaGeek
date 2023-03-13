using MaGeek.AppBusiness;

namespace MaGeek.AppFramework
{

    public class AppBiz
    {

        public MageekDbHandler DB { get; }
        public MageekImporter Importer { get; }
        public MageekUtils Utils { get; }

        public AppBiz()
        {
            DB = new MageekDbHandler();
            DB.InitDb(DB.GetNewContext());
            Importer = new MageekImporter(DB.GetNewContext());
            Utils = new MageekUtils(DB.GetNewContext());
        }

    }

}
