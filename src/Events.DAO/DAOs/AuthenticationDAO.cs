﻿/*
 * Project: Agenda Cultural Back End Net Core
 * Author: Luis Fernando Choque (luisfernandochoquea@gmail.com)
 * -----
 * Copyright 2021 - 2022 Universidad Privada Boliviana La Paz, Luis Fernando Choque Arana
 */
using Events.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events.DAO
{
    public class AuthenticationDAO: IAuthenticationDAO
    {
        private readonly IMongoCollection<User> _users;

        public AuthenticationDAO(IAgendaCulturalDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<User> getUsers()
        {
            try
            {
                var filter = FilterDefinition<User>.Empty;
                var list = _users.FindAsync(filter).Result.ToList();
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public User getUserById(string userId)
        {
            try
            {
                var existinguser = _users.Find(userFind => userFind.Id == userId).FirstOrDefault();
                EventHandler(existinguser != null);
                return existinguser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public User postUser(User user)
        {
            try
            {
                _users.InsertOne(user);
                Logger.Info($"Authentication DAO - User Created at {System.DateTime.Now} with the name {user.Firstname}  {user.Lastname} with username {user.Username}");
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<User> patchUser(string userId, User user)
        {
            try
            {
                var existinguser = await _users.Find(userFind => userFind.Id == userId).FirstOrDefaultAsync();
                EventHandler(existinguser != null);
                user.Id = userId;
                var patchedUser = userPatcher(user, existinguser);
                await _users.ReplaceOneAsync(evnt => evnt.Id == userId, patchedUser);
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task deleteUser(string userId)
        {
            try
            {
                var existinguser = await _users.Find(userFind => userFind.Id == userId).FirstOrDefaultAsync();
                if (existinguser == null)
                    throw new KeyNotFoundException();
                DeleteResult deleteResult = await _users.DeleteOneAsync(evnt => evnt.Id == userId);
                EventHandler(deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0);
                Logger.Info($"Authentication DAO - User deleted at {System.DateTime.Now} with the name {existinguser.Firstname}  {existinguser.Lastname} with username {existinguser.Username}");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void EventHandler(bool flag)
        {
            if (!flag) { throw new KeyNotFoundException(); }
        }

        private User userPatcher(User updatedUser, User storedUser)
        {
            return new User
            {
                Id = storedUser.Id,
                Username = updatedUser.Username ?? storedUser.Username,
                Firstname = updatedUser.Firstname ?? storedUser.Firstname,
                Lastname = updatedUser.Lastname ?? storedUser.Lastname,
                Password = string.IsNullOrEmpty(updatedUser.Password) ? storedUser.Password : updatedUser.Password,
                Admin = updatedUser.Admin,
            };
        }

        public User getUsersByUserName(string userName)
        {
            try
            {
                var existingUser = _users.Find(userFind => userFind.Username == userName).FirstOrDefaultAsync();
                //EventHandler(existingEvent != null);
                return existingUser.Result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
