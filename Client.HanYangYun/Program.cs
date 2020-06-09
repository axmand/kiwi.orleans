﻿using Client.HanYangYun.Util;
using GrainInterface.CMS;
using GrainInterface.WMS;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;

namespace Client.HanYangYun
{
    class Program
    {
        static void Main(string[] args)
        {
            //启动服务
            Helper.Start();
            //启动集群链接
            Helper.DelayInvoke(6000, () =>{
                Helper.service.AddLogging();
                Helper.service.AddOrleansMultiClient(build => {
                    //silo 1
                    build.AddClient(opt => {
                        opt.ServiceId = "dev1";
                        opt.ClusterId = "App";
                        opt.SetServiceAssembly(typeof(IWMS).Assembly);
                        opt.Configure = (o =>
                        {
                            o.UseLocalhostClustering(gatewayPort: 30001);
                        });
                    });
                    //silo 2
                    build.AddClient(opt => {
                        opt.ServiceId = "dev2";
                        opt.ClusterId = "App";
                        opt.SetServiceAssembly(typeof(ICMS).Assembly);
                        opt.Configure = (o =>
                        {
                            o.UseLocalhostClustering(gatewayPort: 30002);
                        });
                    });
                });
                Helper.provider = Helper.service.BuildServiceProvider();
            });
            //阻止客户端退出
            Console.ReadKey();
        }
    }
}