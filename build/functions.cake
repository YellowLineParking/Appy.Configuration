public class ProjectTaskConfigurationManager
{
	readonly IDictionary<string, HashSet<string>> _taskConfigLookup = new Dictionary<string, HashSet<string>>()
	{
		{ "Clean"	, new HashSet<string>(new string[] { "App", "Package" }) },
		{ "Build"	, new HashSet<string>(new string[] { "App", "Package" }) },
		{ "Pack" , new HashSet<string>(new string[] { "App", "Package" }) },
		{ "Publish" , new HashSet<string>(new string[] { "App", "Package" }) },
	};

	bool CanDoTask(string taskName, string projectType) => _taskConfigLookup[taskName].Contains(projectType);

	public bool CanClean(ProjectConfigDescriptor projectConfig) => CanDoTask("Clean", projectConfig.Type);

    public bool CanBuild(ProjectConfigDescriptor projectConfig) => CanDoTask("Build", projectConfig.Type);

    public bool CanPack(ProjectConfigDescriptor projectConfig) => CanDoTask("Pack", projectConfig.Type);

    public bool CanPublish(ProjectConfigDescriptor projectConfig) => CanDoTask("Publish", projectConfig.Type);
}

public class ProjectConfigLoader
{
	static ProjectConfigContainerDescriptor MapToConfigContainer(IDictionary<string, IDictionary<string, string>> configLookup)
	{
		var container = new ProjectConfigContainerDescriptor();
		foreach(var projectPair in configLookup)
		{
			container.Projects.Add(
				ProjectConfigDescriptor.Create(
                    projectPair.Key,
                    projectPair.Value[nameof(ProjectConfigDescriptor.Type)]));
		}

		return container;
	}

	public ProjectConfigContainerDescriptor Load(ICakeContext context, string filePath)
	{
		var configLookup = context.DeserializeYamlFromFile<IDictionary<string, IDictionary<string, string>>>(filePath);

        return MapToConfigContainer(configLookup);
	}
}

public class ProjectConfigContainerDescriptor
{
	public ProjectConfigContainerDescriptor()
	{
		Projects = new List<ProjectConfigDescriptor>();
	}

	public IList<ProjectConfigDescriptor> Projects { get; }

	public void Add(ProjectConfigDescriptor projectConfig) => Projects.Add(projectConfig);
}

public class ProjectConfigDescriptor
{
	protected ProjectConfigDescriptor(string name, string type)
	{
		Name = name;
		Type = type;
	}

	public string Name { get; }

	public string Type { get; }

	public static ProjectConfigDescriptor Create(string name, string type) => new ProjectConfigDescriptor(name, type);
}