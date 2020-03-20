using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("addproduct")]
        public IActionResult AddProduct([FromBody]Product product)
        {
            try
            {
                _productService.Create(product);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var product = _productService.GetAll();
                return Ok(product);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("findproduct")]
        public IActionResult GetbyName([FromBody]Product product)
        {
            try
            {
                var _product = _productService.GetByName(product.Name);



                return Ok(new
                {
                    Name = _product.Name,
                    Price = _product.Price,
                    Amount = _product.Amount
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("gettoorder")]
        public IActionResult GettoOrder()
        {
            try
            {
                var products = _productService.GettoOrder();
                return Ok(products);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut]
        public IActionResult Update([FromBody]Product product)
        {
            try
            {
                _productService.Update(product);
                return Ok("produkt zaktualizowany");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.Delete(id);
                return Ok("produkt usunięty");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
