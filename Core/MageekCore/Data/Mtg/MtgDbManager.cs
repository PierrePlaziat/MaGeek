namespace MageekCore.Data.Mtg
{

    public class MtgDbManager
    {

        public MtgDbManager()
        {
        }

        public async Task<MtgDbContext?> GetContext()
        {
            return new MtgDbContext(Folders.File_UpdatePrints);
        }

    }

}
