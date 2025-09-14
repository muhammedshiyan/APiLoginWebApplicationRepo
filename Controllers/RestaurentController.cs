using APiLoginWebApplication.Data;
using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APiLoginWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurentController : Controller
    {
        private readonly AppDbContext _context;
        public RestaurentController(AppDbContext context) => _context = context;
        
        // GET api/Restaurent/menus
        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus()
       {
            var menuItems = await _context.MenuItems
                            .Select(m => new
                            {
                                m.Id,
                                m.Name,
                                m.Description,
                                m.Price,
                                m.Image,
                                m.Category,
                                m.Featured
                            }).ToListAsync(); // FIX: Ensure using Microsoft.EntityFrameworkCore and keep Select before ToListAsync

            return Ok(menuItems);
        }

        // GET api/Restaurent/testimonial
        [HttpGet("testimonials")]
        public async Task<IActionResult> GetTestimonials()
        {
            try {
                var testimonials = await _context.Testimonials
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Role,
                        t.Image,
                        t.Rating,
                        t.Review,
                        t.ReviewDate
                    }).ToListAsync(); // FIX: Ensure using Microsoft.EntityFrameworkCore and keep Select before ToListAsync
                return Ok(testimonials);

            }
            catch (Exception ex) {
                return Ok();
            }
        }
    }
}
        