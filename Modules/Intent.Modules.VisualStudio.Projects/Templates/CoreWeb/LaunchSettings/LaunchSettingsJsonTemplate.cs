using System;
using System.Collections.Generic;
using System.IO;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.LaunchSettings
{
    public class LaunchSettingsJsonTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.LaunchSettings";

        public LaunchSettingsJsonTemplate(IProject project, IApplicationEventDispatcher applicationEventDispatcher)
            : base(Identifier, project, null)
        {
            applicationEventDispatcher.Subscribe(LaunchProfileRegistrationEvent.EventId, Handle);
        }

        public IDictionary<string, Profile> Profiles { get; } = new Dictionary<string, Profile>();

        private void Handle(ApplicationEvent @event)
        {
            Profiles.Add(@event.GetValue(LaunchProfileRegistrationEvent.ProfileNameKey), new Profile
            {
                commandName = @event.GetValue(LaunchProfileRegistrationEvent.CommandNameKey),
                launchBrowser = bool.TryParse(@event.GetValue(LaunchProfileRegistrationEvent.LaunchBrowserKey), out var launchBrowser) && launchBrowser,
                launchUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.LaunchUrlKey),
                applicationUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.ApplicationUrl),
            });
        }

        public override string TransformText()
        {
            dynamic config;
            if (!File.Exists(GetMetadata().GetFullLocationPathWithFileName()))
            {
                var randomPort = new Random().Next(40000, 65535);
                var randomSslPort = new Random().Next(44300, 44399);
                config = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new LaunchSettingsJson()
                {
                    iisSettings = new IISSettings()
                    {
                        windowsAuthentication = false,
                        anonymousAuthentication = true,
                        iisExpress = new IISExpress()
                        {
                            applicationUrl = $"http://localhost:{randomPort}/",
                            sslPort = randomSslPort
                        }
                    },
                    profiles = new Dictionary<string, Profile>()
                    {
                        { "IIS Express", new Profile()
                            {
                                commandName = "IISExpress",
                                launchBrowser = true,
                                environmentVariables = new EnvironmentVariables()
                                {
                                    ASPNETCORE_ENVIRONMENT = "Development"
                                }
                            }
                        },
                        { Project.Name, new Profile()
                            {
                                commandName = "Project",
                                launchBrowser = true,
                                environmentVariables = new EnvironmentVariables()
                                {
                                    ASPNETCORE_ENVIRONMENT = "Development"
                                },
                                applicationUrl = $"http://localhost:{randomPort}/"
                            }
                        }
                    }
                }, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));
            }
            else
            {
                var existingFileContent = File.ReadAllText(GetMetadata().GetFullLocationPathWithFileName());
                config = JsonConvert.DeserializeObject(existingFileContent, new JsonSerializerSettings());
            }

            if (config.profiles == null)
            {
                config.profiles = JObject.FromObject(new Dictionary<string, string>());
            }

            foreach (var profile in Profiles)
            {
                if (config.profiles[profile.Key] == null)
                {
                    config.profiles[profile.Key] = JObject.FromObject(profile.Value, JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    }));
                }
            }

            return JsonConvert.SerializeObject(config, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "launchsettings",
                fileExtension: "json",
                relativeLocation: "Properties"
                );
        }
    }

    public class LaunchSettingsJson
    {
        public IISSettings iisSettings { get; set; }
        public Dictionary<string, Profile> profiles { get; set; }
    }

    public class IISSettings
    {
        public bool windowsAuthentication { get; set; }
        public bool anonymousAuthentication { get; set; }
        public IISExpress iisExpress { get; set; }
    }

    public class IISExpress
    {
        public string applicationUrl { get; set; }
        public int sslPort { get; set; }
    }

    public class Profile
    {
        public string commandName { get; set; }
        public bool launchBrowser { get; set; }
        public EnvironmentVariables environmentVariables { get; set; }
        public string applicationUrl { get; set; }
        public string launchUrl { get; set; }
    }

    public class EnvironmentVariables
    {
        public string ASPNETCORE_ENVIRONMENT { get; set; }
    }
}