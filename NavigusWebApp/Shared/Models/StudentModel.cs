using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
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
