using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo1
{
    public class LDB
    {
        /// <summary>
        /// 核心的LiteDatabase数据库操作对象，这个对象私有只在类内使用。
        /// </summary>
        private LiteDatabase db;

        /// <summary>
        /// 所有的Person列表。
        /// </summary>
        public ILiteCollection<Person> Persons { get; set; }

        /// <summary>
        /// 创建一个LiteDB对象，有一个参数表明对应的文件路径。
        /// </summary>
        /// <param name="dbpath">对应数据库文件的路径，默认位置为程序目录下"mydata.db"</param>
        public LDB(string connectionString = "Filename=mydata.db;Connection=shared;")
        {
            db = new LiteDatabase(connectionString);
            Persons = db.GetCollection<Person>("Persons");
        }

        public LiteDatabase GetLiteDatabase()
        {
            return db;
        }
    }

    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }
}
