using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BasketService.Api.Core.Application.Repositories;


namespace BasketService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
// [Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository basketRepository, IIdentityService identityService, IEventBus eventBus,
        ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Basket Service is Up and Running");
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket request)
    {
        return Ok(await _basketRepository.UpdateBasketAsync(request));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem request)
    {
        var userId = _identityService.GetUserName();

        var basket = await _basketRepository.GetBasketAsync("cenker");

        if (basket == null)
        {
            basket = new CustomerBasket("cenker");
        }

        basket.Items.Add(request);

        await _basketRepository.UpdateBasketAsync(basket);

        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;

        var basket = await _basketRepository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = _identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City,
            basketCheckout.Street, basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode,
            basketCheckout.CardNumber, basketCheckout.CardHolderName, basketCheckout.CardExpiration,
            basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer,
            new CustomerBasket { BuyerId = "", /*Items*/ });

        try
        {
            //listen itself to clean the basket
            //it islistened by OrderApi to start to 
            _eventBus.Publish(eventMessage);
        }
        catch (Exception)
        {
            _logger.LogError("Error publishing integration Event");
        }

        return Accepted();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}