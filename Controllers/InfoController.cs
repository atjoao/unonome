using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

[Route("/-/{controller}")]
public class InfoController : Controller
{
    private readonly IEnumerable<EndpointDataSource> _endpointSources;

    public InfoController(
        IEnumerable<EndpointDataSource> endpointSources
    )
    {
        _endpointSources = endpointSources;
    }

    [HttpGet("endpoints")]
    public ActionResult ListAllEndpoints()
    {
        var endpoints = _endpointSources
            .SelectMany(static es => es.Endpoints)
            .OfType<RouteEndpoint>();
        var output = endpoints.Select(
            static e =>
            {
                var controller = e.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();
                var action = controller != null
                    ? $"{controller.ControllerName}.{controller.ActionName}"
                    : null;
                var controllerMethod = controller != null
                    ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
                    : null;
                return new
                {
                    Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    Route = $"/{e.RoutePattern.RawText?.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            }
        );

        return Json(output);
    }
}