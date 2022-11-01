using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Queries.GetOrderDetailById;

namespace OrderService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetailsById(Guid id)
    {
        var result = await _mediator.Send(new GetOrderDetailsQuery(id));

        return Ok(result);
    }
}