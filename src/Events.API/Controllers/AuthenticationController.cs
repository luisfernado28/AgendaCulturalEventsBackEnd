﻿using Events.Domain;
using Events.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Controllers
{
    [ApiController]
    [Route("v1.0/auth")]
    public class AuthenticationController : ControllerBase
    {
        public IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService authService)
        {
            this._authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] User userObj)
        {
            var User = await _authService.postUser(userObj);
            return Ok(User);
        }
    }
}
