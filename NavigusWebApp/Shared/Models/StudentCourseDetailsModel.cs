using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class StudentCourseDetailsModel
    {
        [FirestoreProperty]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public Timestamp EnrolledTimeStamp { get; set; }
        public string EnrollTime=>EnrolledTimeStamp.ToDateTime().ToString();
        [FirestoreProperty]
        public Timestamp QuizExpireTimeStamp { get; set; }
        public string QuizExpireTime => QuizExpireTimeStamp.ToDateTime().ToString();

        [FirestoreProperty]
        public bool QuizStarted { get; set; } = false;
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
