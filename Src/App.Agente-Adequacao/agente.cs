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
using UglyToad.PdfPig;


#pragma warning disable OPENAI001

const string endpoint = "https://fd-moongy-ai.services.ai.azure.com/api/projects/rg-moongy-ai-proj";
const string agentName = "agente-adequacao";
// const string userMessage = "Hi agente-adequacao";


 string url = "https://rgmoongyaia2de.blob.core.windows.net/curriculos/CV_Python_Developer_5_Anos.pdf";
HttpClient _httpClient = new HttpClient();
byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);

// var pdf = await _httpClient.GetAsync(url);


Console.WriteLine($"Arquivo baixado com sucesso! Tamanho: {fileBytes.Length} bytes.");


var descricao_oportunidade = @"About the job
We are growing. And we are looking for talented people.
At Decskill, we believe that technological excellence is driven by human talent.We are an IT consulting company with more than 10 years of consolidated experience in the market, focused on building long-term relationships with both our clients and our people. Today, we are a community of over 800 professionals, working from Lisbon, Porto, and Madrid, contributing to impactful technology initiatives.As part of the Astek Group, we combine a strong local culture with a global presence, being active in around 23 countries across 4 continents. This allows us to offer an international context, diverse challenges, and long-term career opportunities, while staying close to our teams.
We would like to meet a Senior Python Developer for a project with a hybrid working model:
Responsibilities:
Design, develop, and maintain Python applications
Write clean, efficient, and well-documented code
Collaborate with cross-functional teams (product, QA, DevOps)
Develop and integrate APIs and backend services
Participate in code reviews and technical discussions
Troubleshoot, debug, and optimize applications

Requirements:
Strong experience with Python
Experience with frameworks such as Django, Flask, or FastAPI
Solid understanding of RESTful APIs
Experience with databases (PostgreSQL, MySQL, or MongoDB)
Familiarity with Git and version control
Good problem-solving and communication skills

Are you looking for an environment that values curiosity and commitment? Here, your contribution has real impact and individual growth is taken seriously.
Find with us the right opportunity to grow!
What you can expect from us:
Long-term projects with national and international context 
Opportunities to grow technically and professionally
A people-first culture, focused on transparency and trust
Teams that value ownership, collaboration and stability

Join us Send us your CV in English to career@decskill.com with reference “DA / Python”.
Your next challenge might start here.
At Decskill, we are committed to equal opportunities and non-discrimination. We promote a culture of diversity and inclusion, where recruitment and career progression are based solely on talent, regardless of age, gender, ethnicity, race, nationality, or any other form of discrimination incompatible with human dignity.";




// string texto = Encoding.UTF8.GetString(fileBytes);



 var textoCompleto = new StringBuilder();

// Carrega o array de bytes diretamente em um MemoryStream
using (var stream = new MemoryStream(fileBytes))
using (var document = PdfDocument.Open(stream))
{
    foreach (var page in document.GetPages())
    {
        textoCompleto.AppendLine(page.Text);
    }
}

 string userMessage = $"Descricao Oportunidade: {descricao_oportunidade}; Currículo: {textoCompleto.ToString()}";

// Connect to your project using the endpoint from your project page.
// DefaultAzureCredential can use your Azure CLI identity; run `az login` for local authentication.
AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new DefaultAzureCredential());

// var clientSecret = "8x9dogibi0x20LBfHgIyZRcwNSE6va2b";
// var tenantId = "28969962-";
// var clientId = "1a2b886b-";

//Para utilizar como app fora do Portal do Azure
// var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
// AIProjectClient projectClient = new(
//     endpoint: new Uri(endpoint), 
//     tokenProvider: credential
// );


ProjectResponsesClient agentClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(agentName);

// Reuse the agent-scoped SDK pipeline for conversations as well as responses.
ProjectConversationsClient conversationClient = new(    agentClient.Pipeline,    new OpenAIClientOptions { Endpoint = agentClient.Endpoint });
ProjectConversation conversation = conversationClient.CreateProjectConversation();

ProjectResponsesClient responseClient =
    projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(
        agentName, defaultConversationId: conversation.Id);

ResponseResult response = responseClient.CreateResponse(userMessage);

Console.WriteLine(response.GetOutputText());



