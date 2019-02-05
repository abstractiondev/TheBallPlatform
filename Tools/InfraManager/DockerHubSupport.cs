using System;
using System.Linq;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Models;

namespace KubeTool
{
    public class DockerHubSupport
    {
        public static async Task<string> GetLatestTag(string repositoryName)
        {
            var dockerClientConfig = new RegistryClientConfiguration("registry.hub.docker.com");
            var client = dockerClientConfig.CreateClient(new AnonymousOAuthAuthenticationProvider());
            var repoInfo = await client.Tags.ListImageTagsAsync(repositoryName, new ListImageTagsParameters());
            var latestTag = repoInfo.Tags.Last(tag => Char.IsNumber(tag.FirstOrDefault()));
            //var numLatest = await client.Manifest.GetManifestAsync(repositoryName, latestTag);
            //var latestLatest = await client.Manifest.GetManifestAsync(repositoryName, "latest");
            return latestTag;
        }
    }
}