using Google.Cloud.Firestore;

namespace NavigusWebApp.Shared.Models
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
