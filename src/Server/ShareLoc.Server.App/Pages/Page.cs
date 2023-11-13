using System.Reflection;

using DotLiquid;

namespace ShareLoc.Server.App.Pages;

public abstract class Page
{
	public virtual string Name => $"{GetType().Name}";

	private Template _template = null!;

	public void Init()
	{
		var liquidFile = $"Pages.{Name}";
		var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(liquidFile) ??
			throw new FileNotFoundException(liquidFile);

		using var sr = new StreamReader(stream);
		var content = sr.ReadToEnd();

		_template = Template.Parse(content);
	}

	public string Render(Hash parameters) => _template.Render(parameters);

	public abstract void MapEndpoints(WebApplication app);
}
