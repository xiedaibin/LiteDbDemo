using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Demo1
{
    public class CurdTest
    {
        LDB _db;
        public CurdTest()
        {
            _db = new LDB();
        }

        public int Insert(string name = "Tom", int age = 21)
        {
            Person tom = new Person() { ID = 0, Name = name, Age = age };
            _db.Persons.Insert(tom);
            return tom.ID;
        }

        public void Update(int id, string name = "张三")
        {
            var person = _db.Persons.FindOne(p => p.ID == id);
            person.Name = name;
            _db.Persons.Update(person);
        }

        public void FindById(int id)
        {
            var person = _db.Persons.FindById(id);
            Console.WriteLine($"FindById {id}:{person?.Name}");
        }

        public void Delete(int id)
        {
            _db.Persons.DeleteMany(p => p.ID == id);
        }

        public Person? FirstOrDefault()
        {
            return _db.Persons.Find(p => p.Name == p.Name, limit: 1).FirstOrDefault();
        }

        /// <summary>
        /// 创建唯一索引
        /// </summary>
        public void EnsureIndex()
        {
            _db.Persons.EnsureIndex(p1 => p1.ID, true);
            _db.Persons.EnsureIndex(p1 => p1.Name, unique: false);
            //_db.Persons.DropIndex("Name");
        }

        /// <summary>
        /// 查询
        /// </summary>
        public void SelectByQuery()
        {
            // 查询 name 为 "John" 的文档
            var results = _db.Persons.Find(Query.And(Query.EQ(field: "Name", value: "张三"), Query.GT("Age", 20)));
            foreach (var doc in results)
            {
                //Console.WriteLine($"one person:{doc.ID},{doc.Name},{doc.Age}");
            }
        }

        public void ShowIndex()
        {
            // 获取系统集合 "_indexes"
            var sysIndexes = _db.GetLiteDatabase().GetCollection("$indexes");

            // 查询指定集合的索引信息
            var indexes = sysIndexes.Find(Query.EQ("collection", "Persons"));

            Console.WriteLine("索引信息：");
            foreach (var index in indexes)
            {
                Console.WriteLine($"collection:{index["collection"]},字段: {index["expression"]}, 唯一: {index["unique"]}, 名称: {index["name"]}");
            }
        }

        /// <summary>
        /// 事务测试
        /// </summary>
        public void TransactionTest(bool throwErr)
        {
            //注：问 事务中使用时会出现游标未关闭错误
            var person = _db.Persons.FindOne(p => p.ID == 20);
            if (_db.GetLiteDatabase().BeginTrans())
            {
                try
                {
                    int id = Insert(name: "事务新增" + throwErr, age: 23);
                    string updateName = "事务新增" + throwErr + "update";
                    if (person != null)
                    {
                        person.Name = updateName;
                        _db.Persons.Update(person);
                    }
                    if (throwErr)
                    {
                        throw new Exception("事务出错");
                    }
                    _db.GetLiteDatabase().Commit();
                    Console.WriteLine($"Transaction successed: 事务成功");
                }
                catch (Exception ex)
                {
                    _db.GetLiteDatabase().Rollback();
                    Console.WriteLine($"Transaction failed: {ex.Message}");
                }

            }
        }

        public void HandlerFile()
        {
            Console.WriteLine("文件处理:");
            // Get file storage with Int Id
            var storage = _db.GetLiteDatabase().GetStorage<int>();
            storage.Upload(123, "a.pdf");
            storage.Download(123, @"D:\a.pdf", overwritten: true);
        }

        /// <summary>
        /// 显示json
        /// </summary>
        /// <param name="person"></param>
        public void ShowJson(Person person)
        {
            var doc = BsonMapper.Global.ToDocument(person);
            // 将 BsonDocument 转换为 JSON 字符串
            var jsonString = LiteDB.JsonSerializer.Serialize(doc);
            Console.WriteLine($"person jsonString:{jsonString}");
        }

        public void Show()
        {
            var persons = _db.Persons.FindAll();
            foreach (var person in persons)
            {
                Console.WriteLine($"person:{person.ID},{person.Name},{person.Age}");
            }
        }

        public void Main()
        {
            Console.WriteLine("1.新增");
            int id = Insert();
            Show();
            Console.WriteLine("2.修改");
            Update(id);
            FindById(id);
            Show();
            //Console.WriteLine("3.删除");
            var persion = FirstOrDefault();

            if (persion != null)
            {
                //文档json
                ShowJson(persion);
                //Delete(persion!.ID);
            }
            Show();
            Console.WriteLine("4.建立索引");
            EnsureIndex();
            ShowIndex();
            SelectByQuery();

            //事务新增 异常
            TransactionTest(true);

            //事务新增 无异常
            TransactionTest(false);

            //文件处理
            HandlerFile();

            //ObjectId
            ShowObjectId();
        }

        public void ShowObjectId()
        {
            var id = ObjectId.NewObjectId();
            Console.WriteLine($"ObjectId.NewObjectId():{id}");
        }
    }
}
