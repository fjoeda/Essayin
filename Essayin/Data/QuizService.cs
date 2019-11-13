using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Essayin.Data
{
    public class QuizService
    {
        MongoClient guruClient = new MongoClient("mongodb+srv://essayinuser:essayinpass@cluster0-ngjns.mongodb.net/gurudb?retryWrites=true&w=majority");
        MongoClient muridClient = new MongoClient("mongodb+srv://essayinuser:essayinpass@cluster0-ngjns.mongodb.net/muriddb?retryWrites=true&w=majority");
        public QuizService()
        {
            
        }

        #region Create User
        public void CreateNewGuruUser(string email, string nama)
        {
            var document = DocumentBuilder.BuildGuruUser(email, nama);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<BsonDocument>("guru");
            collection.InsertOne(document);
        }

        public void CreateNewStudentUser(string email, string nama)
        {
            var document = DocumentBuilder.BuildSiswaUser(email, nama);
            var db = muridClient.GetDatabase("muriddb");
            var collection = db.GetCollection<BsonDocument>("siswa");
            collection.InsertOne(document);
        }
        #endregion

        #region Check if something exist
        public bool CheckIfGuruExist(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<BsonDocument>("guru");
            var count = collection.Find(filter).CountDocuments();
            if (count > 0)
                return true;
            else
                return false;
        }

        public bool CheckIfSiswaExist(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = muridClient.GetDatabase("muriddb");
            var collection = db.GetCollection<BsonDocument>("siswa");
            var count = collection.Find(filter).CountDocuments();
            if (count > 0)
                return true;
            else
                return false;
        }

        public bool CheckIfSoalExist(string id)
        {
            try
            {
                List<QuestionItemModel> result = new List<QuestionItemModel>();
                var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId == id);
                var db = guruClient.GetDatabase("gurudb");
                var collection = db.GetCollection<Guru>("guru");
                var guruObj = collection.Find(filter).First();
                if (guruObj.Quizzes.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

        #region and update data
        public void UpdateSoalList(string email,List<Quiz> quizzes)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<BsonDocument>("guru");
            var update = Builders<BsonDocument>.Update.Set("Quizzes", quizzes);
            collection.UpdateOne(filter, update);
        }

        public void AddResultList(string email, SiswaResult result)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = muridClient.GetDatabase("muriddb");
            var collection = db.GetCollection<BsonDocument>("siswa");
            var siswaObj = BsonSerializer.Deserialize<Siswa>(collection.Find(filter).First());
            var currentResults = siswaObj.Results;
            currentResults.Add(result);
            var update = Builders<BsonDocument>.Update.Set("Results", currentResults);
            collection.UpdateOne(filter, update);
        }

        #endregion

        #region Get Data
        public List<QuestionItemModel> GetSoalFromId(string id, out string namakuis)
        {
            namakuis = "";
            List<QuestionItemModel> result = new List<QuestionItemModel>();
            var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId==id);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<Guru>("guru");
            var guruObj = collection.Find(filter).First();
            foreach (var item in guruObj.Quizzes)
            {
                if (item.QuizId.Equals(id))
                {
                    namakuis = item.QuizName;
                    result = item.QuizItems.ToList();
                    break;
                }
            }
            return result;
        }

        public List<QuizResults> GetQuizResultsFromId(string id)
        {
            List<QuizResults> result = new List<QuizResults>();
            var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId == id);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<Guru>("guru");
            var guruObj = collection.Find(filter).First();
            foreach (var item in guruObj.Quizzes)
            {
                if (item.QuizId.Equals(id))
                {
                    result = item.QuizResults.ToList();
                    break;
                }
            }
            return result;
        }

        public List<Quiz> GetQuizListFromGuru(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<BsonDocument>("guru");
            var guruObj = BsonSerializer.Deserialize<Guru>(collection.Find(filter).First());
            return guruObj.Quizzes.ToList()  ;
        }

        public List<SiswaResult> GetSiswaResults(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = muridClient.GetDatabase("muriddb");
            var collection = db.GetCollection<BsonDocument>("siswa");
            var siswaObj = BsonSerializer.Deserialize<Siswa>(collection.Find(filter).First());
            return siswaObj.Results.ToList();
        }
        #endregion

        #region Other Function
        public string GenerateQuizCode()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }
        #endregion
    }

    public class DocumentBuilder
    {

        public static BsonDocument BuildSoalListBson(string id, List<QuestionItemModel> questions)
        {
            Quiz quiz = new Quiz
            {
                QuizId = id,
                QuizItems = questions
            };
            return quiz.ToBsonDocument();
        }

        public static BsonDocument BuildGuruUser(string email, string name)
        {
            Guru user = new Guru
            {
                Email = email,
                Nama = name
            };
            return user.ToBsonDocument();
        }

        public static BsonDocument BuildSiswaUser(string email, string name)
        {
            Siswa user = new Siswa
            {
                Email = email,
                Nama = name
            };
            return user.ToBsonDocument();
        }

    }
}
