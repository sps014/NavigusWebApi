using Google.Cloud.Firestore;

namespace NavigusWebApp.Shared.Models
{
    [FirestoreData]
    public class StudentCourseDetailsModel
    {
        [FirestoreProperty]
        public string CourseId { get; set; }
        [FirestoreProperty]
        public string EnrolledTimeStamp { get; set; }
        public long EnrollTime => !string.IsNullOrWhiteSpace(EnrolledTimeStamp)? long.Parse(EnrolledTimeStamp):0;

        [FirestoreProperty]
        public string QuizExpireTimeStamp { get; set; }
        public long QuizExpireTime=> !string.IsNullOrWhiteSpace(QuizExpireTimeStamp)? long.Parse(QuizExpireTimeStamp) :0;

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
