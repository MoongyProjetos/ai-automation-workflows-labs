// dotnet add package Azure.AI.Projects --version 2.1.0-beta.4
// dotnet add package Azure.Identity --version 1.21.0
// dotnet add package UglyToad.PdfPig --prerelease
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI;
using OpenAI.Responses;
using System.Text;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


#pragma warning disable OPENAI001

const string endpoint = "https://fd-moongy-ai.services.ai.azure.com/api/projects/rg-moongy-ai-proj";
const string agentName = "agente-parecer-tecnico";

string textoOriginal = File.ReadAllText("content.txt");
string resultado_adequacao = JsonSerializer.Serialize(textoOriginal);

 string userMessage = $"Resultado Adequação: {resultado_adequacao}";

AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new DefaultAzureCredential());


ProjectResponsesClient agentClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(agentName);

// Reuse the agent-scoped SDK pipeline for conversations as well as responses.
ProjectConversationsClient conversationClient = new(    agentClient.Pipeline,    new OpenAIClientOptions { Endpoint = agentClient.Endpoint });
ProjectConversation conversation = conversationClient.CreateProjectConversation();

ProjectResponsesClient responseClient =
    projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(
        agentName, defaultConversationId: conversation.Id);

ResponseResult response = responseClient.CreateResponse(userMessage);

Console.WriteLine(response.GetOutputText());



