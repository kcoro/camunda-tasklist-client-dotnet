// Install Zeebe Client - dotnet add package zb-client --version 1.2.0
// Install Newtonsoft JSON library - dotnet add package Newtonsoft.Json --version x.x.x.x
// Install GraphQL library - dotnet add package GraphQL.Client --version 4.0.2
// Install GraphQL Serializer Newtonsoft - dotnet add package GraphQL.Client.Serializer.Newtonsoft --version 4.0.2
using Zeebe.Client;
using Zeebe.Client.Impl.Builder;
using Zeebe.Client.Impl.Worker;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace CamundaTasklistClient;
public class Client {
    static async void Main(String[] args) {
        // var zeebeClient = CamundaCloudClientBuilder
        // .Builder()
        //     .UseClientId("CLIENT_ID")
        //     .UseClientSecret("CLIENT_SECRET")
        //     .UseContactPoint("ZEEBE_ADDRESS")
        // .Build();

        var zeebeClient = CamundaCloudClientBuilder
            .Builder()
                .FromEnv()
            .Build();

        var topology = await zeebeClient.TopologyRequest().Send();

        string zeebeAddress = Environment.GetEnvironmentVariable("ZEEBE_ADDRESS");

        var graphQLClient = new GraphQLHttpClient(zeebeAddress + "/graphql", new NewtonsoftJsonSerializer());
        bool run = true;

        while(run) {
            Console.WriteLine("Query Tasks - enter 1\nClaim Task - enter 2\nComplete Task - enter 3\nExit - enter 4");
            string userInput = Console.ReadLine();

            switch (userInput) {
                case "1":
                    queryTasks(graphQLClient);
                    break;
                case "2":
                    Console.WriteLine("Enter TaskID: ");
                    string taskId = Console.ReadLine();
                    Console.WriteLine("Enter assignee: ");
                    string assignee = Console.ReadLine();
                    Console.WriteLine(claimTask(taskId, assignee, graphQLClient));
                    break;
                case "3":
                    completeTask(graphQLClient);
                    break;
                case "4":
                    run = false;
                    break;
                default:
                    Console.WriteLine("Invalid input, try again.");
                    break;
            }
        }
    }

    // Query Tasks
    public static async Task queryTasks(GraphQLHttpClient graphQLClient) {
        var tasksRequest = new GraphQLRequest {
            Query = @"
            {
                tasks {
                    id
                    name
                    assignee {
                        id
                    }
                }
            }"
        };

        var graphQLResponse = await graphQLClient.SendQueryAsync<ResponseType>(tasksRequest);
        Console.WriteLine(graphQLResponse.Data.Tasks);
    }

    // Claim Task
    public static string claimTask(string taskId, string assignee, GraphQLHttpClient graphQLClient) {
        string claimQuery = @"mutation {
            setAssignee(taskEntityId: " + '"' + taskId + '"' + ", assignee: " + '"' + assignee + '"' + @") {
                id
                name
                assignee {
                    id
                }
            }
        }";

        return "";
    }

    // Complete Task
    public static string completeTask(GraphQLHttpClient graphQLClient) {

        return "";
    }
}

public class ResponseType 
{
    public TasksType Tasks { get; set; }
}

public class TasksType {
    public string Id {get; set;}
    public string Name {get; set;}
    public AssigneeType Assignee {get; set;} 
}

public class AssigneeType {
    public string Id {get; set;}
}
