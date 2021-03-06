﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Keda.Durable.Scaler.Server.Protos;
using Keda.Durable.Scaler.Server.Repository;
using Keda.Durable.Scaler.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;

namespace Keda.Durable.Scaler.Server
{
    public class Startup
    {
        public const string ConnectionString = "CONNECTION_STRING";

        public const string TaskHub = "TASK_HUB";

        public const string MaxPollingIntervalMillisecond = "MAX_POLLING_INTERVAL";

        public const string CertPath = "CERT_PATH";

        public const string CertPass = "CERT_PASS";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IPerformanceMonitorRepository, PerformanceMonitorRepository>();
            // TODO add configuration settings for Durable Task
            services.AddSingleton<DurableTaskContext>(GetDurableTaskContext());
            services.AddSingleton<IKubernetesRepository, KubernetesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapGrpcService<ExternalScalerService>(); });
        }

        internal DurableTaskContext GetDurableTaskContext()
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            var configuration = builder.Build();
            var context = new DurableTaskContext();
            context.StorageAccount = configuration[ConnectionString];
            context.TaskHub = configuration[TaskHub];
            string maxPollingIntervalMillisecond = configuration[MaxPollingIntervalMillisecond];
            if (!string.IsNullOrEmpty(maxPollingIntervalMillisecond))
            {
                context.MaxPollingIntervalMillisecond = int.Parse(maxPollingIntervalMillisecond);
            }

            return context;
        }
    }
}