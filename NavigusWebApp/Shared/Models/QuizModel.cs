using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class QuizModel
    {
        [FirestoreProperty]
        public uint PassingMarks { get; set; }

        [FirestoreProperty]
        public QuestionModel[] Questions { get; set; }

        /// <summary>
        /// Duration in Minutes after which quiz expires for particular student
        /// </summary>
        [FirestoreProperty]
        public long Duration { get; set; } = 30;
    }
}
