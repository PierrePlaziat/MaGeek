using MageekCore.Data;
using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;
using MageekCore.Service;

Console.WriteLine("MAGEEK TESTS");

MtgDbManager mtg = new();
CollectionDbManager collec = new(mtg);
MtgJsonManager json = new MtgJsonManager(mtg, collec);

await json.FetchPrices();

Console.WriteLine("Done");
Console.WriteLine("");
Console.WriteLine("- Press any key to exit -");
Console.ReadKey();
