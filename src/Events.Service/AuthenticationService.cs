﻿using Events.DAO;
using Events.Domain;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;


namespace Events.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private IAuthenticationDAO  _authDao;

        public AuthenticationService(IAuthenticationDAO authDAO)
        {
            _authDao = authDAO;

        }

        public async Task<User> postUser(User userObj)
        {
            Logger.Info($"EventsService - Trying to create user with the name of {userObj.Username}.");
            userObj.Password = BC.HashPassword(userObj.Password);

            var User = await _authDao.postUser(userObj);
            return User;
        }
    }
}
