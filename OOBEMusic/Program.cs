﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OOBEMusic
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());

            var service = new OOBEMusicPlayer(loggerFactory.CreateLogger<OOBEMusicPlayer>());

            ServiceBase.Run(service); // Lancement du service

        }
    }
}
