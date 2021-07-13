﻿using App.Models.Entities;
using App.Infrastructures.Dbcontexts;
using App.Mapper;
using App.Repositories.BaseRepository;
using App.Repositories.Interface;
using App.Services;
using App.Services.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using App.Repositories;

namespace App.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            //DI
            services.AddSingleton(mapper);
            services.AddTransient<DBInitializer>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<UserManager<User>, UserManager<User>>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IDiscountService, DiscountService>();
            services.AddTransient<IDiscountRepository, DiscountRepository>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ILangRepository, LangRepository>();
            services.AddTransient<ILangService, LangService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ICategoryDetailsRepository, CategoryDetailsRepository>();
            services.AddTransient<IProductDetailRepository, ProductDetailRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IBillRepository, BillRepository>();
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<IBillDetailRepository, BillDetailRepository>();
            services.AddTransient<IBillDetailService, BillDetailService>();
            services.AddTransient<IStorageService, FileStorageService>();
        }
    }
}