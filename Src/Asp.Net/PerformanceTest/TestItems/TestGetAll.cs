﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using SqlSugar;
using Dapper.Contrib.Extensions;

namespace PerformanceTest.TestItems
{
    public class TestGetAll  
    {
        public void Init()
        {
            Console.WriteLine("测试一次读取100万条数据的速度");
            var eachCount = 1;

            Console.WriteLine("开启预热");
            Dapper(1);
            SqlSugar(1);
            Console.WriteLine("预热完毕");

            for (int i = 0; i < 10; i++)
            {
                //dapper
               Dapper(eachCount);

                //sqlSugar
              SqlSugar(eachCount); 
            }
 
        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = new SqlSugarClient(new ConnectionConfig() { InitKeyType=InitKeyType.SystemTable, ConnectionString= Config.connectionString, DbType=DbType.SqlServer }))
                {
                   var list2 = conn.Queryable<Test>().ToList();
                }
            });
        }

        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(Config.connectionString))
                {
                  var list = conn.GetAll<Test>();
                }
            });
        }
    }
}