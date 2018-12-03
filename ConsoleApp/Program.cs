using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.Web.Script.Serialization;
using MongoDB;
using Castle.DynamicProxy;
using System.Net;
using System.IO.Compression;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net.Http;
using System.Windows.Forms;
using System.Net.Http.Headers;
using VoiceRSS_SDK;
using HtmlAgilityPack;
using System.Web;
using System.Reflection;
using AutoMapper;
using JT.Infrastructure.AppCommon;
using System.Drawing;
using System.Drawing.Imaging;
using RabbitMQ.Client;
using MassTransit;
using ConsoleApp.BusEvents;
using ConsoleApp.BusEvents.Message;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string phone = "13450272034";
            Console.WriteLine($"{phone.Substring(0, 3)}****{phone.Substring(4, 4)}");
            //MassTransitTTest();
            //RabbitMQProducerTest();
            //RabbitMQConsumerTest();
            //LoopStart();
            //PermissionTest();
            //CastleTest();
        }

        public static void MassTransitTTest()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq((config) =>
            {
                var host = config.Host(new Uri("rabbitmq://localhost//"), h =>
                {
                    h.Username("admin");
                    h.Password("admin0628");
                });

                config.ReceiveEndpoint(host, "test_queue", ep =>
                {
                    ep.Handler<BaseMessage>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Content}");
                    });
                });
            });
            bus.Start();
            bus.Publish(new BusEvents.Message.Message { Content = "Hi" });
        }

        #region rabbitMqTest
        public static void RabbitMQProducerTest()
        {
            // create connection
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "admin";
            factory.Password = "admin0628";
            factory.HostName = "localhost";
            IConnection conn = factory.CreateConnection();

            // open channel and config exchange, queue, bind
            var exchangeName = "exchangeName";
            var channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            var queueName = "queueName";
            var durable = false;    // 可持续
            var exclusive = false;  // 独占
            var autoDelete = false; // 自动删除
            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
            var routeKey = "routeKey";
            channel.QueueBind(queueName, exchangeName, routeKey);

            // publish message
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, world!");
            channel.BasicPublish(exchangeName, routeKey, null, messageBodyBytes);
        }
        public static void RabbitMQConsumerTest()
        {
            // create connection
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "admin";
            factory.Password = "admin0628";
            factory.HostName = "localhost";
            IConnection conn = factory.CreateConnection();

            // open channel and config exchange, queue, bind
            var exchangeName = "exchangeName";
            var channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            var queueName = "queueName";
            var durable = false;    // 可持续
            var exclusive = false;  // 独占
            var autoDelete = false; // 自动删除
            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
            var routeKey = "routeKey";
            channel.QueueBind(queueName, exchangeName, routeKey);

            // received message
            bool noAck = false;
            BasicGetResult result = channel.BasicGet(queueName, noAck);
            if (result == null)
            {
                // No message available at this time.
            }
            else
            {
                IBasicProperties props = result.BasicProperties;
                byte[] body = result.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);

                // acknowledge receipt of the message
                channel.BasicAck(result.DeliveryTag, false);
            }
        }
        #endregion

        #region loop
        public static void LoopStart()
        {
            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> stack = new List<int>();
            for (int i = 1; i < 10; i++)
            {
                Loop(arr, 0, i, 0, stack, true);
            }
        }
        public static void Loop(int[] arr, int index, int len, int result, List<int> stack, bool isSum)
        {
            if (len + index > arr.Length)
                return;
            var s = 0;
            for (int i = 0; i < len; i++)
            {
                s += (int)Math.Pow(10, len - i - 1) * arr[i + index];
            }
            if (!isSum) s = -s;
            result += s;
            stack.Add(s);
            if (index + len == arr.Length && result == 110)
            {
                var query = from e in stack select e > 0 ? "+" + e.ToString() : e.ToString();
                string strResult = string.Join("", query) + "=" + result.ToString();
                Console.WriteLine(strResult.Substring(1));
            }
            for (int i = 1; i < arr.Length - index - len; i++)
            {
                Loop(arr, index + len, i, result, stack, true);
                Loop(arr, index + len, i, result, stack, false);
            }
            stack.Remove(s);
        }
        #endregion

        #region PermissionTest
        public static void PermissionTest()
        {
            // 初始化
            PermissionEnum perm = PermissionEnum.Init;

            // 赋予 Insert, Delete, Update 的权限
            perm = perm | PermissionEnum.Insert;
            perm = perm.AddRight(PermissionEnum.Delete);
            perm = perm.AddRight(PermissionEnum.Update);
            Console.WriteLine((int)perm + " : " + perm);

            // 删除 Delete 的权限
            perm = perm & ~PermissionEnum.Delete;
            Console.WriteLine((int)perm + " : " + perm);

            // 判断是否有 Delete 的权限
            Console.WriteLine(perm.HasRight(PermissionEnum.Delete));
            Console.WriteLine((perm & PermissionEnum.Delete) == PermissionEnum.Delete);

            // 删除 Insert 的权限
            perm = perm.RemoveRight(PermissionEnum.Insert);
            Console.WriteLine((int)perm + " : " + perm);

            Console.WriteLine(PermissionManager.ConverEnum(3));

            var temp = TestEnum.f1;
            temp = temp.AddRight(TestEnum.f1);
            Console.WriteLine((int)temp + " : " + temp);
        }
        #endregion

        #region FileWatcher
        public static void FileWatcherTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "FileWatcherTemp");
            var watcher = new FileSystemWatcher(path);
            // 监听子目录
            watcher.IncludeSubdirectories = true;
            // 设置监视的更改类型为 LastWrite
            //watcher.NotifyFilter = NotifyFilters.LastWrite;

            // 该事件可能会被提交多次：如文本文件内容改变时，文件的LastWrite等其它属性也发生了改变
            // https://blogs.msdn.microsoft.com/oldnewthing/20140507-00/?p=1053/ 
            // https://weblogs.asp.net/ashben/31773 
            watcher.Changed += watcherHandler;
            watcher.Created += watcherHandler;
            watcher.Deleted += watcherHandler;
            watcher.Renamed += watcherHandler;
            watcher.EnableRaisingEvents = true;
        }
        private static DateTime lastRead = DateTime.MinValue;
        public static void watcherHandler(object sender, FileSystemEventArgs e)
        {
            var type = e.ChangeType.ToString();
            var fullPath = e.FullPath;
            var name = e.Name;

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                DateTime lastWriteTime = File.GetLastWriteTime(fullPath);
                if (lastWriteTime != lastRead)
                {
                    lastRead = lastWriteTime;
                }
            }
            Console.WriteLine(string.Format("type: {0}\r\nName: {1};\r\nPath: {2}", type, name, fullPath));
        }
        #endregion

        #region castle
        public static void CastleTest()
        {
            // class proxy
            //ProxyGenerator generator = new ProxyGenerator();
            //LogInterceptor interceptor = new LogInterceptor();
            //CastleM entity = generator.CreateClassProxy<CastleM>(interceptor);
            //entity.Id = 1;
            //entity.Name = "JT";
            //Console.WriteLine("The entity is: " + entity);
            //Console.WriteLine("Type of the entity: " + entity.GetType().FullName);

            // interface proxy with interface target
            ProxyGenerator generator2 = new ProxyGenerator();
            IStorageNode node = generator2.CreateInterfaceProxyWithTargetInterface<IStorageNode>(
                new StorageNode("master")
                , new DualNodeInterceptor(new StorageNode("slave"))
                , new LogInterceptor());
            node.Save("my message"); //应该调用master对象
            node.IsDead = true;
            node.Save("my message"); //应该调用slave对象
            node.Save("my message"); //应该调用master对象
        }
        public class LogInterceptor : IInterceptor
        {
            private void PreProceed(IInvocation invocation)
            {
                Console.Write("Intercepting: " + invocation.Method.Name + "(");
                if (invocation.Arguments != null && invocation.Arguments.Length > 0)
                    for (int i = 0; i < invocation.Arguments.Length; i++)
                    {
                        if (i != 0) Console.Write(", ");
                        Console.Write(invocation.Arguments[i] == null
                            ? "null"
                            : invocation.Arguments[i].GetType() == typeof(string)
                               ? "\"" + invocation.Arguments[i].ToString() + "\""
                               : invocation.Arguments[i].ToString());
                    }
                Console.WriteLine(")");
            }
            private void PostProceed(IInvocation invocation)
            {
            }
            public void Intercept(IInvocation invocation)
            {
                this.PreProceed(invocation);
                //Console.WriteLine(">>" + invocation.Method.Name);
                invocation.Proceed();
                //this.PostProceed(invocation);
            }
        }

        public class DualNodeInterceptor : IInterceptor
        {
            private IStorageNode _slave;
            public DualNodeInterceptor(IStorageNode slave)
            {
                this._slave = slave;
            }
            public void Intercept(IInvocation invocation)
            {
                IStorageNode master = invocation.InvocationTarget as IStorageNode;
                if (master.IsDead)
                {
                    IChangeProxyTarget cpt = invocation as IChangeProxyTarget;
                    //将被代理对象master更换为slave
                    cpt.ChangeProxyTarget(this._slave);
                    //测试中恢复master的状态，以便看到随后的调用仍然使用master这一效果
                    master.IsDead = false;
                }
                invocation.Proceed();
            }
        }
        #endregion
    }

    #region castle
    public class CastleM
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}; Name: {1};", this.Id, this.Name);
        }
    }

    public interface IStorageNode
    {
        bool IsDead { get; set; }
        void Save(string message);
    }
    public class StorageNode : IStorageNode
    {
        private string _name;
        public StorageNode(string name)
        {
            this._name = name;
        }
        public bool IsDead { get; set; }
        public void Save(string message)
        {
            Console.WriteLine(string.Format("\"{0}\" was saved to {1}", message, this._name));
        }
    }

    #endregion

    #region Permission
    /// <summary>
    /// 位运算，二进制
    /// </summary>
    [Flags]
    public enum PermissionEnum
    {
        /// <summary>
        /// 初始化
        /// 0000
        /// </summary>
        Init = 0,
        /// <summary>
        /// 插入
        /// 0001
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 删除
        /// 0010
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 更新
        /// 0100
        /// </summary>
        Update = 4,
        /// <summary>
        /// 查询
        /// 1000
        /// </summary>
        Query = 8
    }

    [Flags]
    public enum TestEnum
    {
        a0 = 1 << 0,
        b0 = 1 << 1,
        c0 = 1 << 2,
        d0 = 1 << 3,
        e0 = 1 << 4,
        f0 = 1 << 5,
        g0 = 1 << 6,
        h0 = 1 << 7,
        i0 = 1 << 8,
        j0 = 1 << 9,
        k0 = 1 << 10,
        l0 = 1 << 11,
        m0 = 1 << 12,
        n0 = 1 << 13,
        o0 = 1 << 14,
        p0 = 1 << 15,
        q0 = 1 << 16,
        r0 = 1 << 17,
        s0 = 1 << 18,
        t0 = 1 << 19,
        u0 = 1 << 20,
        v0 = 1 << 21,
        w0 = 1 << 22,
        x0 = 1 << 23,
        y0 = 1 << 24,
        z0 = 1 << 25,
        a1 = 1 << 26,
        b1 = 1 << 27,
        c1 = 1 << 28,
        d1 = 1 << 29,
        e1 = 1 << 30,
        f1 = 1 << 31,
    }
    public static class PermissionManager
    {
        public static PermissionEnum ConverEnum(string per)
        {
            PermissionEnum perm = (PermissionEnum)Enum.Parse(typeof(PermissionEnum), per);
            return perm;
        }
        public static PermissionEnum ConverEnum(int per)
        {
            PermissionEnum perm = (PermissionEnum)Enum.Parse(typeof(PermissionEnum), per.ToString());
            return perm;
        }
        /// <summary>        
        /// 是否有T这个值        
        /// bool canRead = permissions.Has(PermissionTypes.Read);        
        /// </summary>        
        /// <typeparam name="T"></typeparam>        
        /// <param name="type"></param>        
        /// <param name="value"></param>        
        /// <returns></returns>        
        public static bool HasRight<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否T完全相等        
        /// </summary>        
        /// <typeparam name="T"></typeparam>        
        /// <param name="type"></param>        
        /// <param name="value"></param>        
        /// <returns></returns>        
        public static bool IsEquals<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>        
        /// 添加        
        /// </summary>        
        /// <typeparam name="T"></typeparam>        
        /// <param name="type"></param>        
        /// <param name="value"></param>        
        /// <returns></returns>        
        public static T AddRight<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// 移除        
        /// </summary>        
        /// <typeparam name="T"></typeparam>        
        /// <param name="type"></param>        
        /// <param name="value"></param>        
        /// <returns></returns>        
        public static T RemoveRight<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion
}
