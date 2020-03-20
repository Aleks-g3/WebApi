using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        IOrderService orderService;
        IWalletService walletService;
        public OrdersController(IOrderService _orderService, IWalletService _walletService)
        {
            orderService = _orderService;
            walletService = _walletService;
        }
        [HttpPost("add")]
        public IActionResult Add([FromBody]Order order)
        {
           
            try
            {
                
                orderService.Create(order);
                walletService.Update(order.Sum);
                return Ok();
            }
            catch (AppException ex)
            {
                //return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("wallet")]
        public IActionResult Wallet([FromBody]Wallet wallet)
        {
            walletService.Create(0);
            return Ok();
        }

        [HttpGet("status")]
        public IActionResult GetByStatus()
        {
            try
            {
                var orders = orderService.GetByStatus();
                return Ok(orders);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update")]
        public IActionResult Update([FromBody]Order order)
        {
            try
            {
                orderService.Update(order.Id, order.Status);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("findid/{id}")]
        public IActionResult GetByUser(int id)
        {
            try
            {
                var order = orderService.GetByUser(id);
                return Ok(order);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
