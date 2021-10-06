using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class StudentCourseDetailsModel
    {
        [FirestoreProperty]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public long EnrolledTimeStamp { get; set; }
        [FirestoreProperty]
        public long QuizStartedTimeStamp { get; set; }
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
