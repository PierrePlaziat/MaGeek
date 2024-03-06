using MageekCore;
using MageekCore.Data;

Console.WriteLine("MAGEEK TESTS");

MageekService mageek = new MageekService();
//var v = await mageek.Initialize();
await mageek.FetchPrices();

Console.WriteLine("Done");
Console.WriteLine("");
Console.WriteLine("- Press any key to exit -");
Console.ReadKey();
