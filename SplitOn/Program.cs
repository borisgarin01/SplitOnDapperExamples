using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SplitOn;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
                        .AddUserSecrets<Program>()
    .Build();

        //var workers = await GetWorkersAsync(config["WMSA_Connection"]);

        var stackSborkaItemsAsync = await GetStackSborkaItemsAsync(config["WMSA_Connection"]);

        //foreach (var worker in workers)
        //{
        //    Trace.WriteLine($"{worker.Code_Work}\t{worker.Name}{Environment.NewLine}{worker.TypeWork.Name}{Environment.NewLine}");
        //}

        Console.ReadKey();
    }

    private static async Task<IEnumerable<Worker>> GetWorkersAsync(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var workers = await connection.QueryAsync<Worker, TypeWork, Worker>("select workers.name, workers.code_work, workers.code_tw, type_work.name from workers inner join type_work on workers.code_tw=type_work.code_tw", (worker, typeWork) => { worker.TypeWork = typeWork; worker.Code_Tw = typeWork.Code_Tw; return worker; }, splitOn: "code_tw");

            return workers;
        }
    }

    private static async Task<IEnumerable<StackSborka>> GetStackSborkaItemsAsync(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            IEnumerable<StackSborka> stackSborkaItems = await connection.QueryAsync<StackSborka, PrikazItem, StackSborka>(@"select StackSborka.prikaz,  StackSborka.num_list, prikazy.prikaz, prikazy.num_nak,  prikazy.data_vip
    from StackSborka inner join prikazy on StackSborka.prikaz = prikazy.prikaz", (stackSborkaItem, prikaz) => { stackSborkaItem.PrikazForStackSborka = prikaz; stackSborkaItem.Prikaz = prikaz.Prikaz; return stackSborkaItem; }, splitOn: "prikaz");

            return stackSborkaItems;
        }
    }
}
