﻿using DotNetBlog.Core.Data;
using DotNetBlog.Core.Entity;
using DotNetBlog.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetBlog.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DotNetBlog.Core.Service
{
    public class AuthService
    {
        private BlogContext BlogContext { get; set; }
        private IHtmlLocalizer<AuthService> L { get; set; }

        public AuthService(BlogContext blogContext, IHtmlLocalizer<AuthService> localizer)
        {
            BlogContext = blogContext;
            L = localizer;
        }

        public async Task<OperationResult<string>> Login(string userName, string password)
        {
            password = Utilities.EncryptHelper.MD5(password);

            User userEntity = await BlogContext.Users.SingleOrDefaultAsync(t => t.UserName == userName && t.Password == password);

            if(userEntity == null)
            {
                return OperationResult<string>.Failure(L["Wrong username or password"].Value);
            }

            userEntity.LoginDate = DateTime.Now;

            string token = Utilities.EncryptHelper.MD5(Guid.NewGuid().ToString());
            var userTokenEntity = new UserToken
            {
                Token = token,
                UserID = userEntity.ID
            };

            BlogContext.Add(userTokenEntity);
            await BlogContext.SaveChangesAsync();

            BlogContext.RemoveUserTokenCache();

            return new OperationResult<string>(token);
        }

        public async Task LogOff(string token)
        {
            var entity = new UserToken
            {
                Token = token
            };
            BlogContext.Attach(entity).State = EntityState.Deleted;
            await BlogContext.SaveChangesAsync();

            BlogContext.RemoveUserTokenCache();
        }
    }
}
