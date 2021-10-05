using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class CourseModel
    {
        [FirestoreProperty]
        public string CourseName { get; set; }

        [FirestoreDocumentId]
        [FirestoreProperty]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public QuizModel Quiz { get; set; }
    }
}
