using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IFMPService _fmpService;
        public PortfolioController(UserManager<User> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository, IFMPService fmpService)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(user);
            var result = userPortfolio.Select(x => x.ToStockDto());
            return Ok(result); //TODO nie zwraca komentarzy do stocków
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string symbol)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepository.GetBySymbolAsync(symbol);


            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("This stock does not exists");
                }
                else
                {
                    await _stockRepository.CreateAsync(stock);
                }
            }

            var userPortfolio = await _portfolioRepository.GetUserPortfolio(user);

            if (userPortfolio.Any(x => x.Symbol.ToLower() == symbol.ToLower()))
            {
                return BadRequest("Cannot add same portfolio");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                UserId = user.Id,
            };

            await _portfolioRepository.CreateAsync(portfolioModel);

            if (portfolioModel == null)
            {
                return StatusCode(500, "Could not create");
            }

            return Created();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(string symbol)
        {
            var username = User.GetUsername(); //bierzemy username z Usera
            var user = await _userManager.FindByNameAsync(username); // pobieramy całego użytkownika po imieniu

            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var userPortfolio = await _portfolioRepository.GetUserPortfolio(user); //pobieramy portfolio znalezionego usera

            var stocks = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();
            if (stocks.Count() == 1)
            {
                await _portfolioRepository.DeleteAsync(user, symbol);
            }
            else
            {
                return BadRequest("Stock is not in your portfolio");
            }

            return NoContent();
        }
    }
}