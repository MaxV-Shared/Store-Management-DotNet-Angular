﻿using App.DTO;
using App.Models.Entities;
using App.Models.Entities.Identities;
using App.Repositories.Interface;
using App.Services.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IMapper _mapper;
        public readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, IMapper mapper, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<UserUpdateRequest>> GetAllAsync(string filter)
        {
            var entities = await _userRepository.GetAllAsync(filter);
            var result = _mapper.Map<IEnumerable<UserUpdateRequest>>(entities);
            return result;
        }

        public async Task<UserUpdateRequest> GetUserById(string id)
        {
            var entity = await _userManager.FindByIdAsync(id);
            var result = _mapper.Map<UserUpdateRequest>(entity);
            return result;
        }
        public async Task<UserUpdateRequest> GetCurrentUser()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return null;
            }
            var user = await _userManager.FindByNameAsync(userName);
            var result = _mapper.Map<UserUpdateRequest>(user);

            return result;
        }
    }
}
