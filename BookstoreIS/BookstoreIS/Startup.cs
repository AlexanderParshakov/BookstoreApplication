﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookstoreIS.Startup))]
namespace BookstoreIS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
