using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Essayin.Data
{

    public class AnswerItem
    {
        public string CorrectAnswer { get; set; }
        public string Answer { get; set; }
    }

    public class Quiz
    {
        public string QuizId { get; set; }
        public string QuizName { get; set; }
        public IList<QuestionItemModel> QuizItems { get; set; } = new List<QuestionItemModel>();
        public IList<QuizResults> QuizResults { get; set; } = new List<QuizResults>();
    }

    public class CheckItem
    {
        public IList<AnswerItem> Items { get; set; }
    }

    public class QuizResults
    {
        public string StudentName { get; set; }
        public int CorrectAnswer { get; set; }
        public int QuestionItemTotal { get; set; }
        public double Score { get; set; }

    }

    public class SiswaResult
    {
        public string QuizId { get; set; }
        public string QuizName { get; set; }
        public int CorrectAnswer { get; set; }
        public int QuestionItemTotal { get; set; }
        public double Score { get; set; }
    }

    public class Guru
    {
        public object Id { get; set; }
        public string Nama { get; set; }
        public string Email { get; set;}
        public IList<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }

    public class Siswa
    {
        public object Id { get; set; }
        public string Nama { get; set; }
        public string Email { get; set; }
        public IList<SiswaResult> Results { get; set; } = new List<SiswaResult>();
    }
}
