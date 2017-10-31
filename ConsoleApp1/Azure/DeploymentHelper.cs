// Requires the following Azure NuGet packages and related dependencies:
// package id="Microsoft.Azure.Management.Authorization" version="2.0.0"
// package id="Microsoft.Azure.Management.ResourceManager" version="1.4.0-preview"
// package id="Microsoft.Rest.ClientRuntime.Azure.Authentication" version="2.2.8-preview"
  
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PortalGenerated
{
    /// <summary>
    /// This is a helper class for deploying an Azure Resource Manager template
    /// More info about template deployments can be found here https://go.microsoft.com/fwLink/?LinkID=733371
    /// </summary>
    public class DeploymentHelper
    {
        string subscriptionId = "your-subscription-id";
        string clientId = "your-service-principal-clientId";
        string clientSecret = "your-service-principal-client-secret";
        string resourceGroupName = "temp-sql-rg";
        string deploymentName = $"YardiRestore{Guid.NewGuid()}";
        string resourceGroupLocation = "South Central US"; // must be specified for creating a new resource group
        string pathToTemplateFile = "path-to-template.json-on-disk";
        string pathToParameterFile = "path-to-parameters.json-on-disk";
        string tenantId = "tenant-id";

        public async void Run()
        {
            // Try to obtain the service credentials
            var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret);

            // Read the template and parameter file contents
            JObject templateFileContents = GetJsonFileContents(pathToTemplateFile);
            JObject parameterFileContents = GetJsonFileContents(pathToParameterFile);

            // Create the resource manager client
            var resourceManagementClient = new ResourceManagementClient(serviceCreds);
            resourceManagementClient.SubscriptionId = subscriptionId;

            // Create or check that resource group exists
            await EnsureResourceGroupExistsAsync(resourceManagementClient, resourceGroupName, resourceGroupLocation);

            // Start a deployment
            await DeployTemplateAsync(resourceManagementClient, resourceGroupName, deploymentName, templateFileContents, parameterFileContents);
        }

        /// <summary>
        /// Reads a JSON file from the specified path
        /// </summary>
        /// <param name="pathToJson">The full path to the JSON file</param>
        /// <returns>The JSON file contents</returns>
        private JObject GetJsonFileContents(string pathToJson)
        {
            JObject templatefileContent = new JObject();
            using (StreamReader file = File.OpenText(pathToJson))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    templatefileContent = (JObject)JToken.ReadFrom(reader);
                    return templatefileContent;
                }
            }
        }

        /// <summary>
        /// Ensures that a resource group with the specified name exists. If it does not, will attempt to create one.
        /// </summary>
        /// <param name="client">The resource manager client.</param>
        /// <param name="rgName">The name of the resource group.</param>
        /// <param name="resourceGroupLocation">The resource group location. Required when creating a new resource group.</param>
        private static async Task EnsureResourceGroupExistsAsync(ResourceManagementClient client, string rgName, string resourceGroupLocation)
        {
			var exists = await client.ResourceGroups.CheckExistenceAsync(rgName);
            if (!exists)
            {
                Console.WriteLine(string.Format("Creating resource group '{0}' in location '{1}'", rgName, resourceGroupLocation));
                var resourceGroup = new ResourceGroupInner();
                resourceGroup.Location = resourceGroupLocation;
				await client.ResourceGroups.CreateOrUpdateAsync(rgName, resourceGroup);
            }
            else
            {
                Console.WriteLine(string.Format("Using existing resource group '{0}'", rgName));
            }
        }

        /// <summary>
        /// Starts a template deployment.
        /// </summary>
        /// <param name="client">The resource manager client.</param>
        /// <param name="rgName">The name of the resource group.</param>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="templateFileContents">The template file contents.</param>
        /// <param name="parameterFileContents">The parameter file contents.</param>
        private static async Task DeployTemplateAsync(ResourceManagementClient client, string rgName, string deploymentName, JObject templateFileContents, JObject parameterFileContents)
        {
            Console.WriteLine(string.Format("Starting template deployment '{0}' in resource group '{1}'", deploymentName, rgName));
            var deployment = new DeploymentInner(new DeploymentProperties()
			{
				Mode = DeploymentMode.Incremental,
				Template = templateFileContents,
				Parameters = parameterFileContents["parameters"].ToObject<JObject>()
			});

			var deploymentResult = await client.Deployments.CreateOrUpdateAsync(rgName, deploymentName, deployment);
            Console.WriteLine(string.Format("Deployment status: {0}", deploymentResult.Properties.ProvisioningState));
        }
    }
}