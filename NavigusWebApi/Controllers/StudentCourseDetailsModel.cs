using Google.Cloud.Firestore;

namespace NavigusWebApi.Controllers
{
    [FirestoreData]
    public class StudentCourseDetailsModel
    {
        [FirestoreProperty]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public string EnrolledDate { get; set; }
        [FirestoreProperty]
        public DateTime QuizStartedTime { get; set; }
        [FirestoreProperty]
        public List<int> Attempted { get; set; }
        [FirestoreProperty]
        public List<int> CorrectAnswersIndex { get; set; }
        [FirestoreProperty]
        public int PointsObtained { get; set; }
        [FirestoreProperty]
        public int XpObtained { get; set; }
          
    }
}
