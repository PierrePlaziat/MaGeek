using PlaziatTools;

namespace MageekCore.Data.Mtg
{

    /// <summary>
    /// MtgSqlive Tooling for .net
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public class MtgDbManager
    {

        public MtgDbManager()
        {
        }

        public async Task<MtgDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new MtgDbContext(Folders.File_UpdatePrints);
        }

    }

}
