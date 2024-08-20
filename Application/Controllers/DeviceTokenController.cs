using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;
using dotnetbase.Application.Wrappers;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceTokenController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly YaboUtilsService _yaboValidationService;


        public DeviceTokenController(DatabaseContext context, YaboUtilsService yaboValidationService)
        {
            _context = context;
            _yaboValidationService = yaboValidationService;

        }



        // GET: api/Charge/5
        [HttpGet("{email}")]
        public async Task<ActionResult> GetUserDeviceToken(string email)
        {
            if (_context.Charges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Charges'  is null." });
            }
            var deviceToken = await _context.UserDeviceTokens.FirstOrDefaultAsync(x => x.Email == email);

            if (deviceToken == null)
            {
                return NotFound(new
                {
                    mesage = "Device token not found"
                });
            }


            return Ok(new Wrappers.ApiResponse<UserDeviceToken>(deviceToken));

        }



        // POST: api/Charge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("devicetoken")]
        public async Task<ActionResult> PostUserDeviceToken(UserDeviceTokenDto userDeviceToken)
        {
            if (_context.UserDeviceTokens == null)
            {
                return Problem("Entity set 'DatabaseContext.Charges'  is null.");
            }


            UserDeviceToken? dbToken = await _context.UserDeviceTokens.FirstOrDefaultAsync(x => x.Email == userDeviceToken.Email);

            if (dbToken == null)
            {
                dbToken = new UserDeviceToken
                {
                    Email = userDeviceToken.Email,
                    DeviceToken = userDeviceToken.DeviceToken,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.UserDeviceTokens.Add(dbToken);
            }
            else
            {
                dbToken.DeviceToken = userDeviceToken.DeviceToken;
                dbToken.UpdatedAt = DateTime.Now;
                _context.UserDeviceTokens.Update(dbToken);
            }


            await _context.SaveChangesAsync();

            return Ok(new Wrappers.ApiResponse<UserDeviceToken>(dbToken));

        }


        [HttpPost("test")]
        public async Task<ActionResult> TestUserDeviceToken()
        {
            var userToken = await _context.UserDeviceTokens.FirstOrDefaultAsync(x => x.Email == "gilclefes@gmail.com");
            if (userToken == null)
            {
                return NotFound(new
                {
                    mesage = "Device token not found"
                });
            }
            var response = await _yaboValidationService.SendNotification("Test from API", "gilclefes@gmail.com");
            return Ok();
        }


    }
}
