using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.ActiveMQ
{
    /// <summary>
    /// ActiveMQ连接管理
    /// </summary>
    public class ActiveMQManager
    {

        private static Dictionary<int, SimplePoolConnManager> ConnPools = new Dictionary<int, SimplePoolConnManager>();//全局连接池表
        private static object _connpoollock = new object();//连接池锁

        /// <summary>
        /// 内部连接池获取
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private SimplePoolConnManager GetPool(ActiveMQConnConfig config)
        {
            int hash = config.GetHashCode();
            if (ConnPools.ContainsKey(hash))
                return ConnPools[hash];
            else
            {
                lock (_connpoollock)
                {
                    if (!ConnPools.ContainsKey(hash))
                    {
                        var pool = new SimplePoolConnManager(new ConnectionFactory(config.ActiveMQHost), config);//创建连接池
                        ConnPools.Add(hash, pool);
                    }
                    return ConnPools[hash];
                }
            }
        }

        /// <summary>
        /// 从连接池中获取一个连接
        /// </summary>
        /// <returns></returns>
        public ActiveMQPoolConnection GetPoolConnection(string tcpConnectString)
        {
            return GetPool(new ActiveMQConnConfig(tcpConnectString) { MaxConnectCount = 2 }).GetPoolConnection();
        }
        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <returns></returns>
        public ActiveMQConnection CreateConnection(string tcpConnectString)
        {
            return new ActiveMQConnection(new ConnectionFactory(tcpConnectString).CreateConnection());
        }
        /// <summary>
        /// 创建一个会话
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public ISession CreateSession(IConnection conn)
        {
            return conn.CreateSession();
        }
    }
    /// <summary>
    /// 简易MQ连接池管理
    /// 备注：未管理连接池的回收工作，未来需要优化
    /// </summary>
    public class SimplePoolConnManager : IDisposable
    {
        private ConnectionFactory ConnectionFactory { get; set; }//连接创建工厂
        private List<ActiveMQPoolConnection> Connections = new List<ActiveMQPoolConnection>();//连接池
        private ActiveMQConnConfig Config { get; set; }//连接池配置信息
        private object _connlock = new object();//连接池内部锁标识

        public SimplePoolConnManager(ConnectionFactory connectionFactory, ActiveMQConnConfig config)
        {
            ConnectionFactory = connectionFactory;
            Config = config;
        }
        /// <summary>
        /// 从池用获取可用连接
        /// </summary>
        /// <returns></returns>
        public ActiveMQPoolConnection GetPoolConnection()
        {
            //连接池未满
            if (Connections.Count < Config.MaxConnectCount)
            {
                var conn = ConnectionFactory.CreateConnection();//放在锁外面是为了加快初始化速度
                bool ifadd = true;
                lock (_connlock)
                {
                    if (Connections.Count <= Config.MaxConnectCount)//重新检查连接池数量
                    {
                        var poolconn = new ActiveMQPoolConnection(this, conn);
                        Connections.Add(poolconn);
                        return poolconn;
                    }
                    else
                        ifadd = false;
                }
                if (ifadd == false)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            int radomcount = Random(Config.MaxConnectCount);//随机连接复用
            var c = Connections[radomcount];
            if (!ConnectionCheck(c))//连接损坏，则修复
            {
                lock (_connlock)
                {
                    c = Connections[radomcount];
                    if (!ConnectionCheck(c))
                    {
                        var conn = ConnectionFactory.CreateConnection();
                        var poolconn = new ActiveMQPoolConnection(this, conn);
                        Connections[radomcount] = poolconn;
                        c = Connections[radomcount];
                    }
                }
            }

            return c;
        }
        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="maxvalue"></param>
        /// <returns></returns>
        private int Random(int maxvalue)
        {
            //Environment.TickCount
            return new Random(Guid.NewGuid().GetHashCode()).Next(0, maxvalue);
        }
        /// <summary>
        /// 连接可用性检查
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool ConnectionCheck(ActiveMQPoolConnection c)
        {
            if (!c.Connection.IsStarted)
            {
                c.Connection.Start();
            }
            if (c.Connection is Apache.NMS.ActiveMQ.Connection)
            {
                var mqconn = c.Connection as Apache.NMS.ActiveMQ.Connection;
                if (mqconn.FirstFailureError != null || mqconn.TransportFailed == true)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 连接池资源释放
        /// </summary>
        public void Dispose()
        {
            if (Connections != null)
            {
                Connections.ForEach((c) =>
                {
                    c.Connection.Close();
                    c.Connection.Dispose();
                });
            }
        }
    }
    /// <summary>
    /// ActiveMQ配置
    /// </summary>
    public class ActiveMQConnConfig
    {
        /// <summary>
        /// ActiveMQ连接Host，就是连接字符串
        /// </summary>
        public string ActiveMQHost { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectCount { get; set; }

        public ActiveMQConnConfig(string activeMQHost)
        {
            ActiveMQHost = activeMQHost;
        }

        public override int GetHashCode()
        {
            return (ActiveMQHost + MaxConnectCount).GetHashCode();
        }
    }
    /// <summary>
    /// 连接资源定义类
    /// </summary>
    public class ActiveMQConnection : IDisposable
    {
        public IConnection Connection { get; set; }

        public ActiveMQConnection(IConnection connection)
        {
            Connection = connection;
        }

        public virtual void Dispose()
        {
            if (Connection != null)
            {
                if (Connection.IsStarted)
                    Connection.Stop();
                Connection.Close();
                Connection.Dispose();
            }
        }
    }
    /// <summary>
    /// 连接池连接外围二次封装
    /// </summary>
    public class ActiveMQPoolConnection : ActiveMQConnection
    {
        public SimplePoolConnManager PoolManager { get; set; }

        public DateTime LastTimeUsedFromPool = DateTime.Now;

        public ActiveMQPoolConnection(SimplePoolConnManager poolmanager, IConnection connection)
            : base(connection)
        {
            PoolManager = poolmanager;
            LastTimeUsedFromPool = DateTime.Now;
        }

        public override void Dispose()
        {

        }
    }
}
