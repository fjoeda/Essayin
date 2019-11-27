using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
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
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
                var db = guruClient.GetDatabase("gurudb");
                var collection = db.GetCollection<BsonDocument>("guru");
                var count = collection.Find(filter).CountDocuments();
                if (count > 0)
                    return true;
                else
                    return false;
            }catch(Exception e)
            {
                return false;
            }
            
        }

        public bool CheckIfSiswaExist(string email)
        {
            try
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
            catch (Exception)
            {

                return false;
            }
           
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

        public void AddResultList(string email, List<SiswaResult> results)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
            var db = muridClient.GetDatabase("muriddb");
            var collection = db.GetCollection<BsonDocument>("siswa");
            var update = Builders<BsonDocument>.Update.Set("Results", results);
            collection.UpdateOne(filter, update);
        }

        public void AddResultToQuiz(string id, QuizResults result)
        {
            var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId == id);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<Guru>("guru");
            var guruObj = collection.Find(filter).First();
            foreach (var item in guruObj.Quizzes)
            {
                if (item.QuizId.Equals(id))
                {
                    guruObj.Quizzes[guruObj.Quizzes.IndexOf(item)].QuizResults.Add(result); 
                    break;
                }
            }
            
            var update = Builders<Guru>.Update.Set("Quizzes", guruObj.Quizzes);
            collection.UpdateOne(filter, update);
        }

        #endregion

        #region Get Data

        

        public List<QuestionItemModel> GetSoalFromId(string id)
        {
            List<QuestionItemModel> result = new List<QuestionItemModel>();
            var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId == id);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<Guru>("guru");
            var guruObj = collection.Find(filter).First();
            foreach (var item in guruObj.Quizzes)
            {
                if (item.QuizId.Equals(id))
                {
                    result = item.QuizItems.ToList();
                    break;
                }
            }
            return result;
        }

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

        #region Delete

        public void DeleteSoal(string id)
        {
            var filter = Builders<Guru>.Filter.ElemMatch(x => x.Quizzes, a => a.QuizId == id);
            var db = guruClient.GetDatabase("gurudb");
            var collection = db.GetCollection<Guru>("guru");
            var guruObj = collection.Find(filter).First();
            foreach (var item in guruObj.Quizzes)
            {
                if (item.QuizId.Equals(id))
                {
                    guruObj.Quizzes.Remove(item);
                    break;
                }
            }

            var update = Builders<Guru>.Update.Set("Quizzes", guruObj.Quizzes);
            collection.UpdateOne(filter, update);
        }

        #endregion

        #region Other Function
        public string GenerateQuizCode()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }
        #endregion

        #region koreksi Skorin
        public SkorinResult KoreksiWithSkorin(List<string> JawabanBenar, List<string> JawabanSiswa)
        {
            var body = new SkorinBody { JawabanBenar = JawabanBenar, JawabanSiswa = JawabanSiswa };
            string url = "http://skorin.herokuapp.com/koreksi-satu/";
            var client = new RestClient();
            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(JsonConvert.SerializeObject(body));
            var response = client.Execute(request);
            var skorinObj = JsonConvert.DeserializeObject<SkorinResult>(response.Content);
            return skorinObj;
        }
        #endregion
    }
    class SkorinBody
    {
        [JsonProperty(PropertyName = "jawaban_benar")]
        public IList<string> JawabanBenar { get; set; }
        [JsonProperty(PropertyName = "jawaban_siswa")]
        public IList<string> JawabanSiswa { get; set; }
    }

    public class SkorinResult
    {
        [JsonProperty(PropertyName = "final_score")]
        public double Score { get; set; } = 0;

        [JsonProperty(PropertyName = "total_jawaban_benar")]
        public int TotalJawabanBenar { get; set; } = 0;

        [JsonProperty(PropertyName = "total_jawaban_salah")]
        public int TotalJawabanSalah { get; set; } = 0;

        [JsonProperty(PropertyName = "total_jawaban_setengah_benar")]
        public int TotalJawabanSetengahBenar { get; set; } = 0;

        public int TotalPertanyaan { get
            {
                return TotalJawabanBenar + TotalJawabanSalah + TotalJawabanSetengahBenar;
            } 
        }
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
                Id = ObjectId.GenerateNewId(),
                Email = email,
                Nama = name
            };
            return user.ToBsonDocument();
        }

        public static BsonDocument BuildSiswaUser(string email, string name)
        {
            Siswa user = new Siswa
            {
                Id = ObjectId.GenerateNewId(),
                Email = email,
                Nama = name
            };
            return user.ToBsonDocument();
        }

    }
}
