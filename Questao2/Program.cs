using Newtonsoft.Json;
using Questao2;
using System.Text;
public class Program
{

    private const string URL_API = "https://jsonmock.hackerrank.com/api/football_matches";
    private const string URI1 = "?year={0}&team1={1}&page={2}";
    private const string URI2 = "?year={0}&team2={1}&page={2}";

    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        var task = Task.Run(() => getTotalGoals(team, year));
        task.Wait();
        return task.Result;
    }

    private static async Task<int> getTotalGoals(string team, int year)
    {
        var totalGoalsTeam1 = await GetTotalGoalsPerZone(team, year, URI1, 1);
        var totalGoalsTeam2 = await GetTotalGoalsPerZone(team, year, URI2, 1);
        return totalGoalsTeam1 + totalGoalsTeam2;
    }

    private static async Task<int> GetTotalGoalsPerZone(string team, int year, string uri, int page)
    {
        using (HttpClient client = new HttpClient())
        {
            byte[] bytes = Encoding.Default.GetBytes(team);
            team = Encoding.UTF8.GetString(bytes);

            client.BaseAddress = new Uri(URL_API);
            var response = await client.GetAsync(string.Format(uri, year, team, page));
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var obj =  JsonConvert.DeserializeObject<GoalsEntity>(jsonString);
                if (obj != null)
                {
                    var totalGoals = 0;
                    var totalPages = obj.total_pages;
                    foreach(var game in obj.data)
                    {
                        if (uri.Contains("team1"))
                        {
                            totalGoals += int.Parse(game.team1goals);
                        } else {
                            totalGoals += int.Parse(game.team2goals);
                        }
                    }
                    while (totalPages > page) 
                    {
                        return totalGoals + await GetTotalGoalsPerZone(team, year, uri, ++page);
                    }
                    return totalGoals;
                    
                }
            }
            return 0;
        }

    }
}