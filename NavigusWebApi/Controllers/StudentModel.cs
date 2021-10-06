using Google.Cloud.Firestore;

namespace NavigusWebApi.Controllers
{
    [FirestoreData]
    public class StudentModel
    {
        [FirestoreProperty]
        public string Uid { get; set; }
        [FirestoreProperty]
        public StudentCourseDetailsModel[] EnrolledCourses { get; set; }
    }
}
