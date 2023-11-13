using System.Net;

namespace ShareLoc.Client.BL.Types;

public readonly struct UnexpectedResponse : IUnexpectedError
{
	public string Name { get; }
	public HttpStatusCode StatusCode { get; }
	public string Content { get; }

	public UnexpectedResponse(HttpStatusCode statusCode, string content)
	{
		Name = "Unexpected Response";
		StatusCode = statusCode;
		Content = content;
	}
}
