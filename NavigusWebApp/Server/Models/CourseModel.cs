using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class CourseModel
    {
        [FirestoreProperty]
        public string CourseName { get; set; }

        [FirestoreDocumentId]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public QuizModel Quiz { get; set; }
        [FirestoreProperty]
        public  string Creator { get; set; }
    }
}
