using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class QuizModel
    {
        [FirestoreProperty]
        public uint PassingMarks { get; set; }

        [FirestoreProperty]
        QuestionModel[] Questions { get; set; }

        /// <summary>
        /// Duration in Milliseconds after which course expires for particular student
        /// </summary>
        [FirestoreProperty]
        public long Duration { get; set; }
    }
}
